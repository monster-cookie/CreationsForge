using System.Diagnostics;
using System.Numerics;
using Avalonia;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;
using Serilog;
using Silk.NET.OpenGL;

namespace CreationsForge.Views;

public class AssetPreviewOpenGlControl : OpenGlControlBase
{
    private const string DesktopVertexShaderSource = """
        #version 330 core
        layout(location = 0) in vec3 aPosition;
        layout(location = 1) in vec3 aColor;
        layout(location = 2) in vec3 aNormal;
        layout(location = 3) in vec2 aUv;

        uniform mat4 uMvp;
        uniform int uUseOverrideColor;
        uniform vec3 uOverrideColor;
        uniform vec3 uLightDirection;

        out vec3 vColor;
        out vec2 vUv;

        void main()
        {
            gl_Position = uMvp * vec4(aPosition, 1.0);
            gl_PointSize = 6.0;
            float diffuse = abs(dot(normalize(aNormal), normalize(uLightDirection)));
            float light = 0.45 + (diffuse * 0.55);
            vColor = uUseOverrideColor == 1 ? uOverrideColor : aColor * light;
            vUv = aUv;
        }
        """;

    private const string DesktopFragmentShaderSource = """
        #version 330 core
        in vec3 vColor;
        in vec2 vUv;

        uniform int uUseTexture;
        uniform sampler2D uTexture;

        out vec4 fragColor;

        void main()
        {
            fragColor = uUseTexture == 1 ? texture(uTexture, vUv) * vec4(vColor, 1.0) : vec4(vColor, 1.0);
        }
        """;

    private const string OpenGlesVertexShaderSource = """
        #version 300 es
        layout(location = 0) in vec3 aPosition;
        layout(location = 1) in vec3 aColor;
        layout(location = 2) in vec3 aNormal;
        layout(location = 3) in vec2 aUv;

        uniform mat4 uMvp;
        uniform int uUseOverrideColor;
        uniform vec3 uOverrideColor;
        uniform vec3 uLightDirection;

        out vec3 vColor;
        out vec2 vUv;

        void main()
        {
            gl_Position = uMvp * vec4(aPosition, 1.0);
            gl_PointSize = 6.0;
            float diffuse = abs(dot(normalize(aNormal), normalize(uLightDirection)));
            float light = 0.45 + (diffuse * 0.55);
            vColor = uUseOverrideColor == 1 ? uOverrideColor : aColor * light;
            vUv = aUv;
        }
        """;

    private const string OpenGlesFragmentShaderSource = """
        #version 300 es
        precision mediump float;

        in vec3 vColor;
        in vec2 vUv;

        uniform int uUseTexture;
        uniform sampler2D uTexture;

        out vec4 fragColor;

        void main()
        {
            fragColor = uUseTexture == 1 ? texture(uTexture, vUv) * vec4(vColor, 1.0) : vec4(vColor, 1.0);
        }
        """;

    private readonly IAssetPreviewRenderMeshFactory RenderMeshFactory;
    private readonly ILogger Logger;
    private AssetPreviewModelDTO? PreviewModelValue;
    private AssetPreviewRenderOptions RenderOptionsValue = new AssetPreviewRenderOptions();
    private GL? Gl;
    private uint VertexArrayObject;
    private uint VertexBufferObject;
    private uint ElementBufferObject;
    private uint LineElementBufferObject;
    private uint ShaderProgram;
    private int VertexCount;
    private int IndexCount;
    private int LineIndexCount;
    private AssetPreviewRenderMesh? CurrentRenderMesh;
    private AssetPreviewOpenGlBounds CurrentRenderBounds = AssetPreviewOpenGlBounds.Empty;
    private readonly List<uint> TextureObjects = new();
    private bool HasPendingMeshUpload = true;
    private bool IsRendererAvailable;
    private bool HasInitializationFailed;
    private bool HasInitialized;
    private bool IsOrbitEnabledValue;
    private AssetPreviewRenderMode RenderModeValue = AssetPreviewRenderMode.Solid;
    private AssetPreviewViewMode ViewModeValue = AssetPreviewViewMode.Isometric;
    private string? LastInitializationError;
    private long RenderCount;

    public AssetPreviewOpenGlControl(IAssetPreviewRenderMeshFactory renderMeshFactory, ILogger logger)
    {
        RenderMeshFactory = renderMeshFactory;
        Logger = logger.ForContext<AssetPreviewOpenGlControl>();
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
        MinHeight = 260;
        DiagnosticText = "OpenGL: waiting for attach";
    }

    public event EventHandler? DiagnosticsChanged;

    public string DiagnosticText { get; private set; }

    public AssetPreviewModelDTO? PreviewModel
    {
        get => PreviewModelValue;
        set
        {
            PreviewModelValue = value;
            HasPendingMeshUpload = true;
            SetDiagnostic("OpenGL: model changed, upload pending");
            RequestNextFrameRendering();
        }
    }

    public AssetPreviewRenderOptions RenderOptions
    {
        get => RenderOptionsValue;
        set
        {
            RenderOptionsValue = value;
            HasPendingMeshUpload = true;
            SetDiagnostic("OpenGL: render options changed, upload pending");
            RequestNextFrameRendering();
        }
    }

    public AssetPreviewViewMode ViewMode
    {
        get => ViewModeValue;
        set
        {
            if (ViewModeValue == value)
            {
                return;
            }

            ViewModeValue = value;
            RequestNextFrameRendering();
        }
    }

    public bool IsOrbitEnabled
    {
        get => IsOrbitEnabledValue;
        set
        {
            if (IsOrbitEnabledValue == value)
            {
                return;
            }

            IsOrbitEnabledValue = value;
            RequestNextFrameRendering();
        }
    }

    public AssetPreviewRenderMode RenderMode
    {
        get => RenderModeValue;
        set
        {
            if (RenderModeValue == value)
            {
                return;
            }

            RenderModeValue = value;
            RequestNextFrameRendering();
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SetDiagnostic($"OpenGL: attached ({Bounds.Width:N0}x{Bounds.Height:N0})");
        Logger.Information(
            "Asset preview OpenGL control attached with bounds {Width}x{Height}",
            Bounds.Width,
            Bounds.Height);
        RequestNextFrameRendering();
    }

    protected override void OnOpenGlInit(GlInterface gl)
    {
        try
        {
            SetDiagnostic("OpenGL: initializing");
            Gl = GL.GetApi(gl.GetProcAddress);
            LogOpenGlInfo("before shader compile");
            ShaderProgram = CreateShaderProgram();
            VertexArrayObject = Gl.GenVertexArray();
            VertexBufferObject = Gl.GenBuffer();
            ElementBufferObject = Gl.GenBuffer();
            LineElementBufferObject = Gl.GenBuffer();
            Gl.Enable(EnableCap.DepthTest);
            UploadMesh();
            IsRendererAvailable = true;
            HasInitialized = true;
            HasInitializationFailed = false;
            LastInitializationError = null;
            LogOpenGlInfo("after initialization");
            SetDiagnostic($"OpenGL: initialized, {IndexCount} indices uploaded");
            Logger.Information("Asset preview OpenGL renderer initialized");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Asset preview OpenGL initialization failed");
            IsRendererAvailable = false;
            HasInitialized = false;
            HasInitializationFailed = true;
            LastInitializationError = ex.Message;
            SetDiagnostic($"OpenGL init failed: {LastInitializationError}");
        }
    }

    protected override void OnOpenGlDeinit(GlInterface gl)
    {
        if (Gl is null)
        {
            return;
        }

        if (ElementBufferObject != 0)
        {
            Gl.DeleteBuffer(ElementBufferObject);
        }

        if (LineElementBufferObject != 0)
        {
            Gl.DeleteBuffer(LineElementBufferObject);
        }

        if (VertexBufferObject != 0)
        {
            Gl.DeleteBuffer(VertexBufferObject);
        }

        if (VertexArrayObject != 0)
        {
            Gl.DeleteVertexArray(VertexArrayObject);
        }

        if (ShaderProgram != 0)
        {
            Gl.DeleteProgram(ShaderProgram);
        }

        DeleteTextureObjects();

        Gl.Dispose();
        Gl = null;
        IsRendererAvailable = false;
        HasInitialized = false;
        SetDiagnostic("OpenGL: deinitialized");
    }

    protected override void OnOpenGlLost()
    {
        Logger.Warning("Asset preview OpenGL context was lost");
        Gl = null;
        VertexArrayObject = 0;
        VertexBufferObject = 0;
        ElementBufferObject = 0;
        LineElementBufferObject = 0;
        ShaderProgram = 0;
        TextureObjects.Clear();
        HasPendingMeshUpload = true;
        IsRendererAvailable = false;
        HasInitialized = false;
        SetDiagnostic("OpenGL: context lost");
    }

    protected override void OnOpenGlRender(GlInterface gl, int framebuffer)
    {
        if (Gl is null || !IsRendererAvailable)
        {
            if (HasInitializationFailed)
            {
                SetDiagnostic($"OpenGL init failed: {LastInitializationError}");
            }
            else if (!HasInitialized)
            {
                SetDiagnostic("OpenGL: waiting for initialization");
            }
            else
            {
                SetDiagnostic("OpenGL: render skipped, renderer unavailable");
            }

            return;
        }

        if (HasPendingMeshUpload)
        {
            UploadMesh();
        }

        var width = Math.Max(1, (uint)Bounds.Width);
        var height = Math.Max(1, (uint)Bounds.Height);
        Gl.Viewport(0, 0, width, height);
        Gl.ClearColor(0.02f, 0.035f, 0.05f, 1f);
        Gl.Clear((uint)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
        RenderCount++;

        if (VertexCount == 0 || (RenderMode != AssetPreviewRenderMode.Points && IndexCount == 0))
        {
            SetDiagnostic($"OpenGL: rendered clear only ({width}x{height}), no drawable geometry");
            return;
        }

        Gl.UseProgram(ShaderProgram);
        SetModelViewProjection(width, height);
        SetLightDirection();
        Gl.BindVertexArray(VertexArrayObject);
        if (RenderMode == AssetPreviewRenderMode.Points)
        {
            Gl.Disable(EnableCap.DepthTest);
            Gl.PointSize(5f);
            SetColorOverride(true, new Vector3(0.25f, 0.78f, 1f));
            Gl.DrawArrays(PrimitiveType.Points, 0, (uint)VertexCount);
            SetColorOverride(false, new Vector3());
            Gl.Enable(EnableCap.DepthTest);
        }
        else
        {
            SetColorOverride(false, new Vector3());
            Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, ElementBufferObject);
            DrawMeshParts();

            if (RenderMode == AssetPreviewRenderMode.Wireframe && LineIndexCount > 0)
            {
                Gl.Disable(EnableCap.DepthTest);
                SetColorOverride(true, new Vector3(1f, 0.88f, 0.25f));
                Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, LineElementBufferObject);
                unsafe
                {
                    Gl.DrawElements(PrimitiveType.Lines, (uint)LineIndexCount, DrawElementsType.UnsignedInt, null);
                }

                SetColorOverride(false, new Vector3());
                Gl.Enable(EnableCap.DepthTest);
            }
        }

        if (RenderCount == 1)
        {
            Logger.Information(
                "Asset preview OpenGL rendered frame {RenderCount} with bounds {Width}x{Height} and {IndexCount} indices",
                RenderCount,
                width,
                height,
                IndexCount);
        }

        SetDiagnostic($"OpenGL: rendered frame {RenderCount:N0} ({width}x{height}), {VertexCount} vertices, {IndexCount} indices, {ViewMode}, {RenderMode}");
        RequestNextFrameRendering();
    }

    private unsafe void UploadMesh()
    {
        if (Gl is null)
        {
            return;
        }

        var renderMesh = RenderMeshFactory.CreateRenderMesh(PreviewModel, RenderOptions);
        CurrentRenderMesh = renderMesh;
        var vertices = renderMesh.Vertices.ToArray();
        var indices = renderMesh.Indices.ToArray();
        var lineIndices = renderMesh.LineIndices.ToArray();
        CurrentRenderBounds = AssetPreviewOpenGlBounds.FromVertexBuffer(vertices);
        VertexCount = vertices.Length / 11;
        IndexCount = indices.Length;
        LineIndexCount = lineIndices.Length;

        Gl.BindVertexArray(VertexArrayObject);
        Gl.BindBuffer(BufferTargetARB.ArrayBuffer, VertexBufferObject);
        fixed (float* vertexPointer = vertices)
        {
            Gl.BufferData(
                BufferTargetARB.ArrayBuffer,
                (nuint)(vertices.Length * sizeof(float)),
                vertexPointer,
                BufferUsageARB.StaticDraw);
        }

        Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, ElementBufferObject);
        fixed (uint* indexPointer = indices)
        {
            Gl.BufferData(
                BufferTargetARB.ElementArrayBuffer,
                (nuint)(indices.Length * sizeof(uint)),
                indexPointer,
                BufferUsageARB.StaticDraw);
        }

        Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, LineElementBufferObject);
        fixed (uint* lineIndexPointer = lineIndices)
        {
            Gl.BufferData(
                BufferTargetARB.ElementArrayBuffer,
                (nuint)(lineIndices.Length * sizeof(uint)),
                lineIndexPointer,
                BufferUsageARB.StaticDraw);
        }

        var stride = 11 * sizeof(float);
        Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)stride, null);
        Gl.EnableVertexAttribArray(0);
        Gl.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, (uint)stride, (void*)(3 * sizeof(float)));
        Gl.EnableVertexAttribArray(1);
        Gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, (uint)stride, (void*)(6 * sizeof(float)));
        Gl.EnableVertexAttribArray(2);
        Gl.VertexAttribPointer(3, 2, VertexAttribPointerType.Float, false, (uint)stride, (void*)(9 * sizeof(float)));
        Gl.EnableVertexAttribArray(3);
        UploadTextures(renderMesh);
        HasPendingMeshUpload = false;
        SetDiagnostic($"OpenGL: uploaded {vertices.Length / 11:N0} vertices, {indices.Length:N0} indices, {lineIndices.Length:N0} line indices");
        Logger.Information(
            "Asset preview OpenGL uploaded {VertexCount} vertices, {IndexCount} indices, and {LineIndexCount} line indices",
            vertices.Length / 11,
            indices.Length,
            lineIndices.Length);
    }

    private unsafe void DrawMeshParts()
    {
        if (Gl is null || CurrentRenderMesh is null)
        {
            return;
        }

        if (CurrentRenderMesh.MeshParts.Count == 0)
        {
            SetTexture(null);
            Gl.DrawElements(PrimitiveType.Triangles, (uint)IndexCount, DrawElementsType.UnsignedInt, null);
            return;
        }

        foreach (var part in CurrentRenderMesh.MeshParts)
        {
            SetTexture(part.TextureIndex);
            Gl.DrawElements(
                PrimitiveType.Triangles,
                (uint)part.IndexCount,
                DrawElementsType.UnsignedInt,
                (void*)(part.IndexOffset * sizeof(uint)));
        }

        SetTexture(null);
    }

    private void SetTexture(int? textureIndex)
    {
        if (Gl is null)
        {
            return;
        }

        if (textureIndex is >= 0 && textureIndex.Value < TextureObjects.Count && TextureObjects[textureIndex.Value] != 0)
        {
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2D, TextureObjects[textureIndex.Value]);
            Gl.Uniform1(Gl.GetUniformLocation(ShaderProgram, "uTexture"), 0);
            Gl.Uniform1(Gl.GetUniformLocation(ShaderProgram, "uUseTexture"), 1);
            return;
        }

        Gl.BindTexture(TextureTarget.Texture2D, 0);
        Gl.Uniform1(Gl.GetUniformLocation(ShaderProgram, "uUseTexture"), 0);
    }

    private unsafe void UploadTextures(AssetPreviewRenderMesh renderMesh)
    {
        if (Gl is null)
        {
            return;
        }

        DeleteTextureObjects();
        foreach (var texture in renderMesh.Textures)
        {
            if (!DdsTexture.TryDecode(texture.Data, out var decodedTexture, out var failureReason))
            {
                Logger.Warning(
                    "Asset preview texture {TexturePath} could not be decoded: {FailureReason}",
                    texture.Path,
                    failureReason);
                TextureObjects.Add(0);
                continue;
            }

            var textureObject = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2D, textureObject);
            Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
            Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
            Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
            Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
            fixed (byte* pixelPointer = decodedTexture.Pixels)
            {
                Gl.TexImage2D(
                    TextureTarget.Texture2D,
                    0,
                    InternalFormat.Rgba,
                    decodedTexture.Width,
                    decodedTexture.Height,
                    0,
                    PixelFormat.Rgba,
                    PixelType.UnsignedByte,
                    pixelPointer);
            }

            TextureObjects.Add(textureObject);
            Logger.Information(
                "Asset preview uploaded texture {TexturePath} ({Width}x{Height})",
                texture.Path,
                decodedTexture.Width,
                decodedTexture.Height);
        }
    }

    private void DeleteTextureObjects()
    {
        if (Gl is null)
        {
            TextureObjects.Clear();
            return;
        }

        foreach (var textureObject in TextureObjects)
        {
            Gl.DeleteTexture(textureObject);
        }

        TextureObjects.Clear();
    }

    private uint CreateShaderProgram()
    {
        if (Gl is null)
        {
            return 0;
        }

        var shaderSources = GetShaderSources();
        Logger.Information("Asset preview OpenGL using {ShaderProfile} shader profile", shaderSources.Profile);
        var vertexShader = CompileShader(ShaderType.VertexShader, shaderSources.VertexShaderSource);
        var fragmentShader = CompileShader(ShaderType.FragmentShader, shaderSources.FragmentShaderSource);
        var shaderProgram = Gl.CreateProgram();
        Gl.AttachShader(shaderProgram, vertexShader);
        Gl.AttachShader(shaderProgram, fragmentShader);
        Gl.LinkProgram(shaderProgram);
        Gl.GetProgram(shaderProgram, ProgramPropertyARB.LinkStatus, out var linked);
        if (linked == 0)
        {
            var infoLog = Gl.GetProgramInfoLog(shaderProgram);
            Logger.Error("Asset preview OpenGL shader link failed: {InfoLog}", infoLog);
            throw new InvalidOperationException($"Asset preview shader link failed: {infoLog}");
        }

        Gl.DeleteShader(vertexShader);
        Gl.DeleteShader(fragmentShader);
        return shaderProgram;
    }

    private (string Profile, string VertexShaderSource, string FragmentShaderSource) GetShaderSources()
    {
        if (string.Equals(GlVersion.Type.ToString(), "OpenGLES", StringComparison.Ordinal))
        {
            return ("OpenGL ES 3.0", OpenGlesVertexShaderSource, OpenGlesFragmentShaderSource);
        }

        return ("Desktop OpenGL 3.3", DesktopVertexShaderSource, DesktopFragmentShaderSource);
    }

    private uint CompileShader(ShaderType shaderType, string source)
    {
        if (Gl is null)
        {
            return 0;
        }

        var shader = Gl.CreateShader(shaderType);
        Gl.ShaderSource(shader, source);
        Gl.CompileShader(shader);
        Gl.GetShader(shader, ShaderParameterName.CompileStatus, out var compiled);
        if (compiled == 0)
        {
            var infoLog = Gl.GetShaderInfoLog(shader);
            Logger.Error(
                "Asset preview OpenGL {ShaderType} compile failed: {InfoLog}",
                shaderType,
                infoLog);
            throw new InvalidOperationException($"Asset preview {shaderType} shader compile failed: {infoLog}");
        }

        return shader;
    }

    private void LogOpenGlInfo(string phase)
    {
        if (Gl is null)
        {
            return;
        }

        Logger.Information(
            "Asset preview OpenGL info {Phase}: vendor {Vendor}, renderer {Renderer}, version {Version}, Avalonia GL version {AvaloniaGlVersion}",
            phase,
            Gl.GetStringS(StringName.Vendor),
            Gl.GetStringS(StringName.Renderer),
            Gl.GetStringS(StringName.Version),
            GlVersion);
    }

    private unsafe void SetModelViewProjection(uint width, uint height)
    {
        if (Gl is null)
        {
            return;
        }

        var angle = IsOrbitEnabled ? RenderCount * 0.01f : 0f;
        var model = Matrix4x4.CreateRotationY(angle);
        var view = GetViewMatrix();
        var projection = GetProjectionMatrix(width, height);
        var mvp = Matrix4x4.Transpose(model * view * projection);
        var matrixValues = new[]
        {
            mvp.M11,
            mvp.M12,
            mvp.M13,
            mvp.M14,
            mvp.M21,
            mvp.M22,
            mvp.M23,
            mvp.M24,
            mvp.M31,
            mvp.M32,
            mvp.M33,
            mvp.M34,
            mvp.M41,
            mvp.M42,
            mvp.M43,
            mvp.M44
        };
        var location = Gl.GetUniformLocation(ShaderProgram, "uMvp");
        fixed (float* matrixPointer = matrixValues)
        {
            Gl.UniformMatrix4(location, 1, false, matrixPointer);
        }
    }

    private Matrix4x4 GetViewMatrix()
    {
        return ViewMode switch
        {
            AssetPreviewViewMode.Front => Matrix4x4.CreateLookAt(new Vector3(0f, 0.2f, 5.25f), Vector3.Zero, Vector3.UnitY),
            AssetPreviewViewMode.Back => Matrix4x4.CreateLookAt(new Vector3(0f, 0.2f, -5.25f), Vector3.Zero, Vector3.UnitY),
            AssetPreviewViewMode.Side => Matrix4x4.CreateLookAt(new Vector3(5.25f, 0.2f, 0f), Vector3.Zero, Vector3.UnitY),
            AssetPreviewViewMode.Top => Matrix4x4.CreateLookAt(new Vector3(0f, 4.5f, 0f), Vector3.Zero, -Vector3.UnitZ),
            _ => Matrix4x4.CreateLookAt(new Vector3(4.2f, 3.2f, 5.0f), new Vector3(0f, 0.15f, 0f), Vector3.UnitY)
        };
    }

    private Matrix4x4 GetProjectionMatrix(uint width, uint height)
    {
        var aspect = width / (float)height;
        if (ViewMode == AssetPreviewViewMode.Isometric || !CurrentRenderBounds.HasValue)
        {
            return Matrix4x4.CreatePerspectiveFieldOfView(
                MathF.PI / 4f,
                aspect,
                0.1f,
                100f);
        }

        var projectedSize = CurrentRenderBounds.GetProjectedSize(ViewMode);
        var halfHeight = MathF.Max(
            projectedSize.Height / 2f,
            projectedSize.Width / (2f * aspect));
        halfHeight = MathF.Max(halfHeight * 1.15f, 0.01f);
        return Matrix4x4.CreateOrthographic(
            halfHeight * 2f * aspect,
            halfHeight * 2f,
            0.1f,
            100f);
    }

    private void SetLightDirection()
    {
        if (Gl is null)
        {
            return;
        }

        Gl.Uniform3(Gl.GetUniformLocation(ShaderProgram, "uLightDirection"), 0.35f, 0.75f, 0.55f);
    }

    private void SetColorOverride(bool isEnabled, Vector3 color)
    {
        if (Gl is null)
        {
            return;
        }

        Gl.Uniform1(Gl.GetUniformLocation(ShaderProgram, "uUseOverrideColor"), isEnabled ? 1 : 0);
        Gl.Uniform3(Gl.GetUniformLocation(ShaderProgram, "uOverrideColor"), color.X, color.Y, color.Z);
    }

    private void SetDiagnostic(string diagnosticText)
    {
        if (string.Equals(DiagnosticText, diagnosticText, StringComparison.Ordinal))
        {
            return;
        }

        DiagnosticText = diagnosticText;
        DiagnosticsChanged?.Invoke(this, EventArgs.Empty);
    }

    private class DdsTexture
    {
        private const int DdsHeaderSize = 128;
        private const uint DdsMagic = 0x20534444;
        private const uint Dxt1FourCc = 0x31545844;
        private const uint Dxt3FourCc = 0x33545844;
        private const uint Dxt5FourCc = 0x35545844;

        public required uint Width { get; set; }

        public required uint Height { get; set; }

        public required byte[] Pixels { get; set; }

        public static bool TryDecode(byte[] data, out DdsTexture texture, out string failureReason)
        {
            texture = new DdsTexture
            {
                Width = 0,
                Height = 0,
                Pixels = []
            };
            if (data.Length < DdsHeaderSize)
            {
                failureReason = "DDS data is shorter than the header.";
                return false;
            }

            var magic = BitConverter.ToUInt32(data, 0);
            if (magic != DdsMagic)
            {
                failureReason = "Texture data is not a DDS file.";
                return false;
            }

            var height = BitConverter.ToUInt32(data, 12);
            var width = BitConverter.ToUInt32(data, 16);
            var fourCc = BitConverter.ToUInt32(data, 84);
            if (width == 0 || height == 0)
            {
                failureReason = "DDS texture has invalid dimensions.";
                return false;
            }

            var blockSize = fourCc == Dxt1FourCc ? 8 : 16;
            if (fourCc != Dxt1FourCc && fourCc != Dxt3FourCc && fourCc != Dxt5FourCc)
            {
                failureReason = $"DDS FourCC 0x{fourCc:X8} is not supported yet.";
                return false;
            }

            var blockCountX = (width + 3) / 4;
            var blockCountY = (height + 3) / 4;
            var requiredLength = DdsHeaderSize + (blockCountX * blockCountY * blockSize);
            if (data.Length < requiredLength)
            {
                failureReason = "DDS data is shorter than the first mip level.";
                return false;
            }

            var pixels = new byte[width * height * 4];
            var offset = DdsHeaderSize;
            for (uint blockY = 0; blockY < blockCountY; blockY++)
            {
                for (uint blockX = 0; blockX < blockCountX; blockX++)
                {
                    if (fourCc == Dxt1FourCc)
                    {
                        DecodeDxt1Block(data, offset, pixels, width, height, blockX, blockY);
                    }
                    else if (fourCc == Dxt3FourCc)
                    {
                        DecodeDxt3Block(data, offset, pixels, width, height, blockX, blockY);
                    }
                    else
                    {
                        DecodeDxt5Block(data, offset, pixels, width, height, blockX, blockY);
                    }

                    offset += blockSize;
                }
            }

            texture = new DdsTexture
            {
                Width = width,
                Height = height,
                Pixels = pixels
            };
            failureReason = string.Empty;
            return true;
        }

        private static void DecodeDxt1Block(byte[] data, int offset, byte[] pixels, uint width, uint height, uint blockX, uint blockY)
        {
            var colors = DecodeColorTable(data, offset, true);
            DecodeColorIndexes(data, offset + 4, pixels, width, height, blockX, blockY, colors, null);
        }

        private static void DecodeDxt3Block(byte[] data, int offset, byte[] pixels, uint width, uint height, uint blockX, uint blockY)
        {
            var alphaValues = new byte[16];
            for (var index = 0; index < 16; index++)
            {
                var packedAlpha = data[offset + (index / 2)];
                var alpha = index % 2 == 0 ? packedAlpha & 0x0F : packedAlpha >> 4;
                alphaValues[index] = (byte)(alpha * 17);
            }

            var colors = DecodeColorTable(data, offset + 8, false);
            DecodeColorIndexes(data, offset + 12, pixels, width, height, blockX, blockY, colors, alphaValues);
        }

        private static void DecodeDxt5Block(byte[] data, int offset, byte[] pixels, uint width, uint height, uint blockX, uint blockY)
        {
            var alphaValues = DecodeDxt5AlphaValues(data, offset);
            var colors = DecodeColorTable(data, offset + 8, false);
            DecodeColorIndexes(data, offset + 12, pixels, width, height, blockX, blockY, colors, alphaValues);
        }

        private static AssetPreviewTextureColor[] DecodeColorTable(byte[] data, int offset, bool allowDxt1Alpha)
        {
            var color0 = BitConverter.ToUInt16(data, offset);
            var color1 = BitConverter.ToUInt16(data, offset + 2);
            var colors = new AssetPreviewTextureColor[4];
            colors[0] = DecodeRgb565(color0, 255);
            colors[1] = DecodeRgb565(color1, 255);
            if (color0 > color1 || !allowDxt1Alpha)
            {
                colors[2] = Interpolate(colors[0], colors[1], 2, 1, 3);
                colors[3] = Interpolate(colors[0], colors[1], 1, 2, 3);
            }
            else
            {
                colors[2] = Interpolate(colors[0], colors[1], 1, 1, 2);
                colors[3] = new AssetPreviewTextureColor(0, 0, 0, 0);
            }

            return colors;
        }

        private static byte[] DecodeDxt5AlphaValues(byte[] data, int offset)
        {
            var alpha0 = data[offset];
            var alpha1 = data[offset + 1];
            var alphaTable = new byte[8];
            alphaTable[0] = alpha0;
            alphaTable[1] = alpha1;
            if (alpha0 > alpha1)
            {
                alphaTable[2] = (byte)(((6 * alpha0) + alpha1) / 7);
                alphaTable[3] = (byte)(((5 * alpha0) + (2 * alpha1)) / 7);
                alphaTable[4] = (byte)(((4 * alpha0) + (3 * alpha1)) / 7);
                alphaTable[5] = (byte)(((3 * alpha0) + (4 * alpha1)) / 7);
                alphaTable[6] = (byte)(((2 * alpha0) + (5 * alpha1)) / 7);
                alphaTable[7] = (byte)((alpha0 + (6 * alpha1)) / 7);
            }
            else
            {
                alphaTable[2] = (byte)(((4 * alpha0) + alpha1) / 5);
                alphaTable[3] = (byte)(((3 * alpha0) + (2 * alpha1)) / 5);
                alphaTable[4] = (byte)(((2 * alpha0) + (3 * alpha1)) / 5);
                alphaTable[5] = (byte)((alpha0 + (4 * alpha1)) / 5);
                alphaTable[6] = 0;
                alphaTable[7] = 255;
            }

            var alphaBits = 0UL;
            for (var index = 0; index < 6; index++)
            {
                alphaBits |= (ulong)data[offset + 2 + index] << (8 * index);
            }

            var alphaValues = new byte[16];
            for (var index = 0; index < 16; index++)
            {
                alphaValues[index] = alphaTable[(int)((alphaBits >> (3 * index)) & 0x07)];
            }

            return alphaValues;
        }

        private static void DecodeColorIndexes(
            byte[] data,
            int offset,
            byte[] pixels,
            uint width,
            uint height,
            uint blockX,
            uint blockY,
            IReadOnlyList<AssetPreviewTextureColor> colors,
            IReadOnlyList<byte>? alphaValues)
        {
            var indexes = BitConverter.ToUInt32(data, offset);
            for (var y = 0; y < 4; y++)
            {
                for (var x = 0; x < 4; x++)
                {
                    var pixelX = (blockX * 4) + (uint)x;
                    var pixelY = (blockY * 4) + (uint)y;
                    if (pixelX >= width || pixelY >= height)
                    {
                        continue;
                    }

                    var localIndex = (y * 4) + x;
                    var colorIndex = (indexes >> (2 * localIndex)) & 0x03;
                    var color = colors[(int)colorIndex];
                    var pixelOffset = (int)(((pixelY * width) + pixelX) * 4);
                    pixels[pixelOffset] = color.Red;
                    pixels[pixelOffset + 1] = color.Green;
                    pixels[pixelOffset + 2] = color.Blue;
                    pixels[pixelOffset + 3] = alphaValues == null ? color.Alpha : alphaValues[localIndex];
                }
            }
        }

        private static AssetPreviewTextureColor DecodeRgb565(ushort value, byte alpha)
        {
            var red = (value >> 11) & 0x1F;
            var green = (value >> 5) & 0x3F;
            var blue = value & 0x1F;
            return new AssetPreviewTextureColor(
                (byte)((red * 255) / 31),
                (byte)((green * 255) / 63),
                (byte)((blue * 255) / 31),
                alpha);
        }

        private static AssetPreviewTextureColor Interpolate(AssetPreviewTextureColor first, AssetPreviewTextureColor second, int firstWeight, int secondWeight, int denominator)
        {
            return new AssetPreviewTextureColor(
                (byte)(((first.Red * firstWeight) + (second.Red * secondWeight)) / denominator),
                (byte)(((first.Green * firstWeight) + (second.Green * secondWeight)) / denominator),
                (byte)(((first.Blue * firstWeight) + (second.Blue * secondWeight)) / denominator),
                (byte)(((first.Alpha * firstWeight) + (second.Alpha * secondWeight)) / denominator));
        }
    }

    private readonly struct AssetPreviewOpenGlBounds
    {
        public static readonly AssetPreviewOpenGlBounds Empty = new AssetPreviewOpenGlBounds();

        private AssetPreviewOpenGlBounds(float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
        {
            HasValue = true;
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
            MinZ = minZ;
            MaxZ = maxZ;
        }

        public bool HasValue { get; }

        private float MinX { get; }

        private float MaxX { get; }

        private float MinY { get; }

        private float MaxY { get; }

        private float MinZ { get; }

        private float MaxZ { get; }

        public static AssetPreviewOpenGlBounds FromVertexBuffer(IReadOnlyList<float> vertices)
        {
            if (vertices.Count < 11)
            {
                return Empty;
            }

            var minX = float.PositiveInfinity;
            var maxX = float.NegativeInfinity;
            var minY = float.PositiveInfinity;
            var maxY = float.NegativeInfinity;
            var minZ = float.PositiveInfinity;
            var maxZ = float.NegativeInfinity;
            for (var index = 0; index + 10 < vertices.Count; index += 11)
            {
                var x = vertices[index];
                var y = vertices[index + 1];
                var z = vertices[index + 2];
                minX = MathF.Min(minX, x);
                maxX = MathF.Max(maxX, x);
                minY = MathF.Min(minY, y);
                maxY = MathF.Max(maxY, y);
                minZ = MathF.Min(minZ, z);
                maxZ = MathF.Max(maxZ, z);
            }

            if (!IsFinite(minX) || !IsFinite(maxX) ||
                !IsFinite(minY) || !IsFinite(maxY) ||
                !IsFinite(minZ) || !IsFinite(maxZ))
            {
                return Empty;
            }

            return new AssetPreviewOpenGlBounds(minX, maxX, minY, maxY, minZ, maxZ);
        }

        public AssetPreviewProjectedSize GetProjectedSize(AssetPreviewViewMode viewMode)
        {
            return viewMode switch
            {
                AssetPreviewViewMode.Side => new AssetPreviewProjectedSize(MaxZ - MinZ, MaxY - MinY),
                AssetPreviewViewMode.Top => new AssetPreviewProjectedSize(MaxX - MinX, MaxZ - MinZ),
                _ => new AssetPreviewProjectedSize(MaxX - MinX, MaxY - MinY)
            };
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }
    }

    private readonly struct AssetPreviewProjectedSize
    {
        public AssetPreviewProjectedSize(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public float Width { get; }

        public float Height { get; }
    }

    private readonly struct AssetPreviewTextureColor
    {
        public AssetPreviewTextureColor(byte red, byte green, byte blue, byte alpha)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        public byte Red { get; }

        public byte Green { get; }

        public byte Blue { get; }

        public byte Alpha { get; }
    }
}
