using System.Diagnostics;
using System.Numerics;
using Avalonia;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using Serilog;
using Silk.NET.OpenGL;

namespace CreationsForge.Views;

public class AssetPreviewOpenGlControl : OpenGlControlBase
{
    private const string VertexShaderSource = """
        #version 330 core
        layout (location = 0) in vec3 aPosition;
        layout (location = 1) in vec3 aColor;

        uniform mat4 uMvp;

        out vec3 vColor;

        void main()
        {
            gl_Position = uMvp * vec4(aPosition, 1.0);
            vColor = aColor;
        }
        """;

    private const string FragmentShaderSource = """
        #version 330 core
        in vec3 vColor;
        out vec4 FragColor;

        void main()
        {
            FragColor = vec4(vColor, 1.0);
        }
        """;

    private readonly IAssetPreviewRenderMeshFactory RenderMeshFactory;
    private readonly ILogger Logger;
    private readonly Stopwatch Stopwatch = Stopwatch.StartNew();
    private AssetPreviewModelDTO? PreviewModelValue;
    private GL? Gl;
    private uint VertexArrayObject;
    private uint VertexBufferObject;
    private uint ElementBufferObject;
    private uint ShaderProgram;
    private int IndexCount;
    private bool HasPendingMeshUpload = true;
    private bool IsRendererAvailable;
    private bool HasInitializationFailed;
    private bool HasInitialized;
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
            ShaderProgram = CreateShaderProgram();
            VertexArrayObject = Gl.GenVertexArray();
            VertexBufferObject = Gl.GenBuffer();
            ElementBufferObject = Gl.GenBuffer();
            Gl.Enable(EnableCap.DepthTest);
            UploadMesh();
            IsRendererAvailable = true;
            HasInitialized = true;
            HasInitializationFailed = false;
            LastInitializationError = null;
            LogOpenGlInfo();
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
        ShaderProgram = 0;
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
        Gl.ClearColor(0.85f, 0.05f, 0.55f, 1f);
        Gl.Clear((uint)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
        RenderCount++;

        if (IndexCount == 0)
        {
            SetDiagnostic($"OpenGL: rendered clear only ({width}x{height}), no indices");
            return;
        }

        Gl.UseProgram(ShaderProgram);
        SetModelViewProjection(width, height);
        Gl.BindVertexArray(VertexArrayObject);
        unsafe
        {
            Gl.DrawElements(PrimitiveType.Triangles, (uint)IndexCount, DrawElementsType.UnsignedInt, null);
        }

        if (RenderCount == 1 || RenderCount % 120 == 0)
        {
            Logger.Information(
                "Asset preview OpenGL rendered frame {RenderCount} with bounds {Width}x{Height} and {IndexCount} indices",
                RenderCount,
                width,
                height,
                IndexCount);
        }

        SetDiagnostic($"OpenGL: rendered frame {RenderCount:N0} ({width}x{height}), {IndexCount} indices");
        RequestNextFrameRendering();
    }

    private unsafe void UploadMesh()
    {
        if (Gl is null)
        {
            return;
        }

        var renderMesh = RenderMeshFactory.CreateRenderMesh(PreviewModel);
        var vertices = renderMesh.Vertices.ToArray();
        var indices = renderMesh.Indices.ToArray();
        IndexCount = indices.Length;

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

        var stride = 6 * sizeof(float);
        Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)stride, null);
        Gl.EnableVertexAttribArray(0);
        Gl.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, (uint)stride, (void*)(3 * sizeof(float)));
        Gl.EnableVertexAttribArray(1);
        HasPendingMeshUpload = false;
        SetDiagnostic($"OpenGL: uploaded {vertices.Length / 6:N0} vertices and {indices.Length:N0} indices");
        Logger.Information(
            "Asset preview OpenGL uploaded {VertexCount} vertices and {IndexCount} indices",
            vertices.Length / 6,
            indices.Length);
    }

    private uint CreateShaderProgram()
    {
        if (Gl is null)
        {
            return 0;
        }

        var vertexShader = CompileShader(ShaderType.VertexShader, VertexShaderSource);
        var fragmentShader = CompileShader(ShaderType.FragmentShader, FragmentShaderSource);
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

    private void LogOpenGlInfo()
    {
        if (Gl is null)
        {
            return;
        }

        Logger.Information(
            "Asset preview OpenGL info: vendor {Vendor}, renderer {Renderer}, version {Version}",
            Gl.GetStringS(StringName.Vendor),
            Gl.GetStringS(StringName.Renderer),
            Gl.GetStringS(StringName.Version));
    }

    private unsafe void SetModelViewProjection(uint width, uint height)
    {
        if (Gl is null)
        {
            return;
        }

        var elapsedSeconds = (float)Stopwatch.Elapsed.TotalSeconds;
        var model = Matrix4x4.CreateRotationZ(elapsedSeconds * 0.35f);
        var view = Matrix4x4.CreateLookAt(new Vector3(0f, -3.2f, 2.1f), new Vector3(0f, 0f, 0.2f), Vector3.UnitZ);
        var projection = Matrix4x4.CreatePerspectiveFieldOfView(
            MathF.PI / 4f,
            width / (float)height,
            0.1f,
            100f);
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

    private void SetDiagnostic(string diagnosticText)
    {
        if (string.Equals(DiagnosticText, diagnosticText, StringComparison.Ordinal))
        {
            return;
        }

        DiagnosticText = diagnosticText;
        DiagnosticsChanged?.Invoke(this, EventArgs.Empty);
    }
}
