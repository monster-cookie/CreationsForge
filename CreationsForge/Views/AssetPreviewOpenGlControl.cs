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
        layout(location = 4) in float aAlpha;

        uniform mat4 uMvp;
        uniform int uUseOverrideColor;
        uniform vec3 uOverrideColor;
        uniform vec3 uLightDirection;

        out vec3 vColor;
        out vec3 vNormal;
        out vec2 vUv;
        out float vAlpha;
        out float vLight;

        void main()
        {
            gl_Position = uMvp * vec4(aPosition, 1.0);
            gl_PointSize = 6.0;
            float diffuse = abs(dot(normalize(aNormal), normalize(uLightDirection)));
            float light = 0.45 + (diffuse * 0.55);
            vColor = uUseOverrideColor == 1 ? uOverrideColor : aColor;
            vNormal = normalize(aNormal);
            vLight = light;
            vUv = aUv;
            vAlpha = aAlpha;
        }
        """;

    private const string DesktopFragmentShaderSource = """
        #version 330 core
        in vec3 vColor;
        in vec3 vNormal;
        in vec2 vUv;
        in float vAlpha;
        in float vLight;

        uniform int uUseTexture;
        uniform sampler2D uTexture;
        uniform vec4 uMaterialTint;
        uniform int uUseOverlayTexture;
        uniform sampler2D uOverlayTexture;
        uniform int uUseDecalOpacityTexture;
        uniform sampler2D uDecalOpacityTexture;
        uniform vec4 uDecalTint;
        uniform vec4 uDecalUvTransform;
        uniform int uUseAdditiveBlend;

        out vec4 fragColor;

        void main()
        {
            float previewLight = 0.85 + (vLight * 0.15);
            float facing = abs(normalize(vNormal).z);
            float highlight = pow(facing, 20.0) * 0.28;
            vec3 fill = vec3(0.08, 0.09, 0.10) * (0.6 + (vLight * 0.4));
            vec4 baseColor;
            if (uUseTexture == 1)
            {
                vec4 textureColor = texture(uTexture, vUv);
                // NifSkope's CE2 preview treats opacity as explicit material state, not ordinary base texture alpha.
                baseColor = vec4(min((textureColor.rgb * uMaterialTint.rgb * previewLight) + fill + vec3(highlight), vec3(1.0)), 1.0);
            }
            else
            {
                baseColor = vec4(vColor * vLight, 1.0);
            }

            if (uUseOverlayTexture == 1)
            {
                vec4 overlayColor = texture(uOverlayTexture, vUv);
                if (uUseAdditiveBlend == 1)
                {
                    baseColor.rgb = min(baseColor.rgb + (overlayColor.rgb * overlayColor.a), vec3(1.0));
                }
                else
                {
                    baseColor.rgb = mix(baseColor.rgb, overlayColor.rgb, overlayColor.a);
                }

                baseColor.a = max(baseColor.a, overlayColor.a);
            }

            if (uUseDecalOpacityTexture == 1)
            {
                vec2 decalUv = (vUv * uDecalUvTransform.xy) + uDecalUvTransform.zw;
                vec4 opacityColor = texture(uDecalOpacityTexture, decalUv);
                float opacity = max(max(opacityColor.r, opacityColor.g), opacityColor.b) * opacityColor.a * vAlpha * uDecalTint.a;
                baseColor = vec4(min(uDecalTint.rgb * previewLight, vec3(1.0)), clamp(opacity, 0.0, 1.0));
            }

            fragColor = baseColor;
        }
        """;

    private const string OpenGlesVertexShaderSource = """
        #version 300 es
        layout(location = 0) in vec3 aPosition;
        layout(location = 1) in vec3 aColor;
        layout(location = 2) in vec3 aNormal;
        layout(location = 3) in vec2 aUv;
        layout(location = 4) in float aAlpha;

        uniform mat4 uMvp;
        uniform int uUseOverrideColor;
        uniform vec3 uOverrideColor;
        uniform vec3 uLightDirection;

        out vec3 vColor;
        out vec3 vNormal;
        out vec2 vUv;
        out float vAlpha;
        out float vLight;

        void main()
        {
            gl_Position = uMvp * vec4(aPosition, 1.0);
            gl_PointSize = 6.0;
            float diffuse = abs(dot(normalize(aNormal), normalize(uLightDirection)));
            float light = 0.45 + (diffuse * 0.55);
            vColor = uUseOverrideColor == 1 ? uOverrideColor : aColor;
            vNormal = normalize(aNormal);
            vLight = light;
            vUv = aUv;
            vAlpha = aAlpha;
        }
        """;

    private const string OpenGlesFragmentShaderSource = """
        #version 300 es
        precision mediump float;

        in vec3 vColor;
        in vec3 vNormal;
        in vec2 vUv;
        in float vAlpha;
        in float vLight;

        uniform int uUseTexture;
        uniform sampler2D uTexture;
        uniform vec4 uMaterialTint;
        uniform int uUseOverlayTexture;
        uniform sampler2D uOverlayTexture;
        uniform int uUseDecalOpacityTexture;
        uniform sampler2D uDecalOpacityTexture;
        uniform vec4 uDecalTint;
        uniform vec4 uDecalUvTransform;
        uniform int uUseAdditiveBlend;

        out vec4 fragColor;

        void main()
        {
            float previewLight = 0.85 + (vLight * 0.15);
            float facing = abs(normalize(vNormal).z);
            float highlight = pow(facing, 20.0) * 0.28;
            vec3 fill = vec3(0.08, 0.09, 0.10) * (0.6 + (vLight * 0.4));
            vec4 baseColor;
            if (uUseTexture == 1)
            {
                vec4 textureColor = texture(uTexture, vUv);
                // NifSkope's CE2 preview treats opacity as explicit material state, not ordinary base texture alpha.
                baseColor = vec4(min((textureColor.rgb * uMaterialTint.rgb * previewLight) + fill + vec3(highlight), vec3(1.0)), 1.0);
            }
            else
            {
                baseColor = vec4(vColor * vLight, 1.0);
            }

            if (uUseOverlayTexture == 1)
            {
                vec4 overlayColor = texture(uOverlayTexture, vUv);
                if (uUseAdditiveBlend == 1)
                {
                    baseColor.rgb = min(baseColor.rgb + (overlayColor.rgb * overlayColor.a), vec3(1.0));
                }
                else
                {
                    baseColor.rgb = mix(baseColor.rgb, overlayColor.rgb, overlayColor.a);
                }

                baseColor.a = max(baseColor.a, overlayColor.a);
            }

            if (uUseDecalOpacityTexture == 1)
            {
                vec2 decalUv = (vUv * uDecalUvTransform.xy) + uDecalUvTransform.zw;
                vec4 opacityColor = texture(uDecalOpacityTexture, decalUv);
                float opacity = max(max(opacityColor.r, opacityColor.g), opacityColor.b) * opacityColor.a * vAlpha * uDecalTint.a;
                baseColor = vec4(min(uDecalTint.rgb * previewLight, vec3(1.0)), clamp(opacity, 0.0, 1.0));
            }

            fragColor = baseColor;
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
    private uint OrientationOverlayVertexArrayObject;
    private uint OrientationOverlayVertexBufferObject;
    private uint ShaderProgram;
    private int VertexCount;
    private int IndexCount;
    private int LineIndexCount;
    private int OrientationOverlayVertexCount;
    private AssetPreviewRenderMesh? CurrentRenderMesh;
    private AssetPreviewOpenGlBounds CurrentRenderBounds = AssetPreviewOpenGlBounds.Empty;
    private readonly List<uint> TextureObjects = new();
    private bool HasPendingMeshUpload = true;
    private bool IsRendererAvailable;
    private bool HasInitializationFailed;
    private bool HasInitialized;
    private AssetPreviewRenderMode RenderModeValue = AssetPreviewRenderMode.Solid;
    private string? LastInitializationError;
    private long RenderCount;
    private Quaternion CameraOrientation = Quaternion.Identity;
    private float CameraZoom = 1f;
    private Vector3 CameraTarget;
    private Vector3 DefaultCameraTarget;
    private Quaternion DefaultCameraOrientation = Quaternion.Identity;
    private float DefaultCameraZoom = 1f;

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
            ResetCamera();
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

    public void BeginOrbitDrag()
    {
        SetDiagnostic("OpenGL: orbit drag started");
    }

    public void OrbitByDragDelta(double deltaX, double deltaY)
    {
        var camera = GetCameraFrame();
        var yawRotation = Quaternion.CreateFromAxisAngle(camera.Up, -(float)deltaX * 0.01f);
        var pitchRotation = Quaternion.CreateFromAxisAngle(camera.Right, -(float)deltaY * 0.01f);
        CameraOrientation = Quaternion.Normalize(pitchRotation * yawRotation * CameraOrientation);
        RequestNextFrameRendering();
    }

    public void BeginPanDrag()
    {
        SetDiagnostic("OpenGL: pan drag started");
    }

    public void PanByDragDelta(double deltaX, double deltaY)
    {
        var camera = GetCameraFrame();
        var viewportHeight = Math.Max(1f, (float)Bounds.Height);
        var viewportWidth = Math.Max(1f, (float)Bounds.Width);
        var panScale = GetCurrentOrthographicHalfHeight(camera.View, viewportWidth / viewportHeight) * 2f / viewportHeight;
        CameraTarget -= camera.Right * ((float)deltaX * panScale);
        CameraTarget += camera.Up * ((float)deltaY * panScale);
        RequestNextFrameRendering();
    }

    public void ZoomByWheelDelta(double wheelDelta)
    {
        var zoomFactor = MathF.Pow(0.88f, (float)wheelDelta);
        CameraZoom = Math.Clamp(CameraZoom * zoomFactor, 0.08f, 20f);
        SetDiagnostic($"OpenGL: zoom {CameraZoom:0.00}");
        RequestNextFrameRendering();
    }

    public void ResetCamera()
    {
        CameraTarget = DefaultCameraTarget;
        CameraOrientation = DefaultCameraOrientation;
        CameraZoom = DefaultCameraZoom;
        SetDiagnostic("OpenGL: camera reset");
        RequestNextFrameRendering();
    }

    public void SnapCameraToPositiveX()
    {
        SnapCameraToDirection(Vector3.UnitX, "X+");
    }

    public void SnapCameraToNegativeX()
    {
        SnapCameraToDirection(-Vector3.UnitX, "X-");
    }

    public void SnapCameraToPositiveY()
    {
        SnapCameraToDirection(Vector3.UnitY, "Y+");
    }

    public void SnapCameraToNegativeY()
    {
        SnapCameraToDirection(-Vector3.UnitY, "Y-");
    }

    public void SnapCameraToPositiveZ()
    {
        SnapCameraToDirection(Vector3.UnitZ, "Z+");
    }

    public void SnapCameraToNegativeZ()
    {
        SnapCameraToDirection(-Vector3.UnitZ, "Z-");
    }

    public void EndCameraDrag()
    {
        SetDiagnostic("OpenGL: camera drag ended");
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
            OrientationOverlayVertexArrayObject = Gl.GenVertexArray();
            OrientationOverlayVertexBufferObject = Gl.GenBuffer();
            Gl.Enable(EnableCap.DepthTest);
            UploadOrientationOverlay();
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

        if (OrientationOverlayVertexBufferObject != 0)
        {
            Gl.DeleteBuffer(OrientationOverlayVertexBufferObject);
        }

        if (VertexBufferObject != 0)
        {
            Gl.DeleteBuffer(VertexBufferObject);
        }

        if (OrientationOverlayVertexArrayObject != 0)
        {
            Gl.DeleteVertexArray(OrientationOverlayVertexArrayObject);
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
        OrientationOverlayVertexArrayObject = 0;
        OrientationOverlayVertexBufferObject = 0;
        ShaderProgram = 0;
        OrientationOverlayVertexCount = 0;
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

        DrawOrientationOverlay(width, height);

        if (RenderCount == 1)
        {
            Logger.Information(
                "Asset preview OpenGL rendered frame {RenderCount} with bounds {Width}x{Height} and {IndexCount} indices",
                RenderCount,
                width,
                height,
                IndexCount);
        }

        var cameraDiagnostic = GetCameraFrame();
        SetDiagnostic($"OpenGL: rendered frame {RenderCount:N0} ({width}x{height}), {VertexCount} vertices, {IndexCount} indices, camera {cameraDiagnostic.Direction.X:0.00}/{cameraDiagnostic.Direction.Y:0.00}/{cameraDiagnostic.Direction.Z:0.00}/{CameraZoom:0.00}, {RenderMode}");
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
        ResetDefaultCamera();
        VertexCount = vertices.Length / 12;
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

        var stride = 12 * sizeof(float);
        Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)stride, null);
        Gl.EnableVertexAttribArray(0);
        Gl.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, (uint)stride, (void*)(3 * sizeof(float)));
        Gl.EnableVertexAttribArray(1);
        Gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, (uint)stride, (void*)(6 * sizeof(float)));
        Gl.EnableVertexAttribArray(2);
        Gl.VertexAttribPointer(3, 2, VertexAttribPointerType.Float, false, (uint)stride, (void*)(9 * sizeof(float)));
        Gl.EnableVertexAttribArray(3);
        Gl.VertexAttribPointer(4, 1, VertexAttribPointerType.Float, false, (uint)stride, (void*)(11 * sizeof(float)));
        Gl.EnableVertexAttribArray(4);
        UploadTextures(renderMesh);
        HasPendingMeshUpload = false;
        SetDiagnostic($"OpenGL: uploaded {vertices.Length / 12:N0} vertices, {indices.Length:N0} indices, {lineIndices.Length:N0} line indices");
        Logger.Information(
            "Asset preview OpenGL uploaded {VertexCount} vertices, {IndexCount} indices, and {LineIndexCount} line indices",
            vertices.Length / 12,
            indices.Length,
            lineIndices.Length);
    }

    private unsafe void UploadOrientationOverlay()
    {
        if (Gl is null)
        {
            return;
        }

        var vertices = CreateOrientationOverlayVertices();
        OrientationOverlayVertexCount = vertices.Length / 12;
        Gl.BindVertexArray(OrientationOverlayVertexArrayObject);
        Gl.BindBuffer(BufferTargetARB.ArrayBuffer, OrientationOverlayVertexBufferObject);
        fixed (float* vertexPointer = vertices)
        {
            Gl.BufferData(
                BufferTargetARB.ArrayBuffer,
                (nuint)(vertices.Length * sizeof(float)),
                vertexPointer,
                BufferUsageARB.StaticDraw);
        }

        var stride = 12 * sizeof(float);
        Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)stride, null);
        Gl.EnableVertexAttribArray(0);
        Gl.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, (uint)stride, (void*)(3 * sizeof(float)));
        Gl.EnableVertexAttribArray(1);
        Gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, (uint)stride, (void*)(6 * sizeof(float)));
        Gl.EnableVertexAttribArray(2);
        Gl.VertexAttribPointer(3, 2, VertexAttribPointerType.Float, false, (uint)stride, (void*)(9 * sizeof(float)));
        Gl.EnableVertexAttribArray(3);
        Gl.VertexAttribPointer(4, 1, VertexAttribPointerType.Float, false, (uint)stride, (void*)(11 * sizeof(float)));
        Gl.EnableVertexAttribArray(4);
    }

    private unsafe void DrawMeshParts()
    {
        if (Gl is null || CurrentRenderMesh is null)
        {
            return;
        }

        if (CurrentRenderMesh.MeshParts.Count == 0)
        {
            SetTextures(null, null, null, false, false, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 0f, 0f);
            Gl.DrawElements(PrimitiveType.Triangles, (uint)IndexCount, DrawElementsType.UnsignedInt, null);
            return;
        }

        foreach (var part in CurrentRenderMesh.MeshParts)
        {
            var useDecalOpacity = part.IsDecal && part.DecalOpacityTextureIndex.HasValue;
            SetBlendMode(useDecalOpacity, part.UseAdditiveBlend);
            SetTextures(
                part.TextureIndex,
                part.OverlayTextureIndex,
                part.DecalOpacityTextureIndex,
                useDecalOpacity,
                part.UseAdditiveBlend,
                part.MaterialTintRed,
                part.MaterialTintGreen,
                part.MaterialTintBlue,
                part.MaterialTintAlpha,
                part.DecalTintRed,
                part.DecalTintGreen,
                part.DecalTintBlue,
                part.DecalOpacity,
                part.DecalUvScaleU,
                part.DecalUvScaleV,
                part.DecalUvOffsetU,
                part.DecalUvOffsetV);
            Gl.DrawElements(
                PrimitiveType.Triangles,
                (uint)part.IndexCount,
                DrawElementsType.UnsignedInt,
                (void*)(part.IndexOffset * sizeof(uint)));
        }

        SetBlendMode(false, false);
        SetTextures(null, null, null, false, false, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 0f, 0f);
    }

    private void SetTextures(int? textureIndex, int? overlayTextureIndex, int? decalOpacityTextureIndex, bool useDecalOpacityTexture, bool useAdditiveBlend, float materialTintRed, float materialTintGreen, float materialTintBlue, float materialTintAlpha, float decalTintRed, float decalTintGreen, float decalTintBlue, float decalOpacity, float decalUvScaleU, float decalUvScaleV, float decalUvOffsetU, float decalUvOffsetV)
    {
        if (Gl is null)
        {
            return;
        }

        var hasPrimaryTexture = BindTexture(TextureUnit.Texture0, textureIndex);
        Gl.Uniform1(Gl.GetUniformLocation(ShaderProgram, "uTexture"), 0);
        Gl.Uniform1(Gl.GetUniformLocation(ShaderProgram, "uUseTexture"), hasPrimaryTexture ? 1 : 0);
        Gl.Uniform4(Gl.GetUniformLocation(ShaderProgram, "uMaterialTint"), materialTintRed, materialTintGreen, materialTintBlue, materialTintAlpha);

        var hasOverlayTexture = BindTexture(TextureUnit.Texture1, overlayTextureIndex);
        Gl.Uniform1(Gl.GetUniformLocation(ShaderProgram, "uOverlayTexture"), 1);
        Gl.Uniform1(Gl.GetUniformLocation(ShaderProgram, "uUseOverlayTexture"), hasOverlayTexture ? 1 : 0);

        var hasDecalOpacityTexture = BindTexture(TextureUnit.Texture2, decalOpacityTextureIndex);
        Gl.Uniform1(Gl.GetUniformLocation(ShaderProgram, "uDecalOpacityTexture"), 2);
        Gl.Uniform1(Gl.GetUniformLocation(ShaderProgram, "uUseDecalOpacityTexture"), useDecalOpacityTexture && hasDecalOpacityTexture ? 1 : 0);
        Gl.Uniform4(Gl.GetUniformLocation(ShaderProgram, "uDecalTint"), decalTintRed, decalTintGreen, decalTintBlue, decalOpacity);
        Gl.Uniform4(Gl.GetUniformLocation(ShaderProgram, "uDecalUvTransform"), decalUvScaleU, decalUvScaleV, decalUvOffsetU, decalUvOffsetV);
        Gl.Uniform1(Gl.GetUniformLocation(ShaderProgram, "uUseAdditiveBlend"), useAdditiveBlend ? 1 : 0);
    }

    private void SetBlendMode(bool isEnabled, bool useAdditiveBlend)
    {
        if (Gl is null)
        {
            return;
        }

        if (!isEnabled)
        {
            Gl.Disable(EnableCap.Blend);
            Gl.DepthMask(true);
            return;
        }

        Gl.Enable(EnableCap.Blend);
        Gl.DepthMask(false);
        Gl.BlendFunc(BlendingFactor.SrcAlpha, useAdditiveBlend ? BlendingFactor.One : BlendingFactor.OneMinusSrcAlpha);
    }

    private bool BindTexture(TextureUnit textureUnit, int? textureIndex)
    {
        if (Gl is null)
        {
            return false;
        }

        Gl.ActiveTexture(textureUnit);
        if (textureIndex is >= 0 && textureIndex.Value < TextureObjects.Count && TextureObjects[textureIndex.Value] != 0)
        {
            Gl.BindTexture(TextureTarget.Texture2D, TextureObjects[textureIndex.Value]);
            return true;
        }

        Gl.BindTexture(TextureTarget.Texture2D, 0);
        return false;
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

        var model = Matrix4x4.Identity;
        var camera = GetCameraFrame();
        var view = camera.View;
        var projection = GetProjectionMatrix(width, height, view);
        var mvp = model * view * projection;
        SetModelViewProjection(mvp);
    }

    private unsafe void SetModelViewProjection(Matrix4x4 mvp)
    {
        if (Gl is null)
        {
            return;
        }

        // System.Numerics builds row-major transforms; OpenGL reads this array as column-major.
        // Sending the row-major values directly gives the shader the equivalent column-vector matrix.
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

    private Matrix4x4 GetProjectionMatrix(uint width, uint height, Matrix4x4 view)
    {
        var aspect = width / (float)height;
        if (!CurrentRenderBounds.HasValue)
        {
            return Matrix4x4.CreatePerspectiveFieldOfView(
                MathF.PI / 4f,
                aspect,
                0.1f,
                100f);
        }

        var halfHeight = GetCurrentOrthographicHalfHeight(view, aspect);
        return Matrix4x4.CreateOrthographic(
            halfHeight * 2f * aspect,
            halfHeight * 2f,
            0.1f,
            100f);
    }

    private AssetPreviewCameraFrame GetCameraFrame()
    {
        var direction = Vector3.Normalize(Vector3.Transform(Vector3.UnitZ, CameraOrientation));
        var target = CameraTarget;
        var cameraDistance = CurrentRenderBounds.HasValue
            ? MathF.Max(CurrentRenderBounds.Radius * 3f, 1f)
            : 8f;
        var eye = target + (direction * cameraDistance);
        var up = Vector3.Normalize(Vector3.Transform(Vector3.UnitY, CameraOrientation));
        var view = Matrix4x4.CreateLookAt(eye, target, up);
        var right = Vector3.Normalize(Vector3.Transform(Vector3.UnitX, CameraOrientation));
        return new AssetPreviewCameraFrame(view, right, up, direction);
    }

    private void DrawOrientationOverlay(uint width, uint height)
    {
        if (Gl is null || OrientationOverlayVertexCount == 0)
        {
            return;
        }

        var viewportSize = Math.Min(96u, Math.Max(56u, Math.Min(width, height) / 5u));
        Gl.Viewport(10, 10, viewportSize, viewportSize);
        Gl.Disable(EnableCap.DepthTest);
        Gl.BindVertexArray(OrientationOverlayVertexArrayObject);
        Gl.BindBuffer(BufferTargetARB.ArrayBuffer, OrientationOverlayVertexBufferObject);
        SetTextures(null, null, null, false, false, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 0f, 0f);
        SetColorOverride(false, new Vector3());

        var camera = GetCameraFrame();
        var direction = camera.Direction;
        var up = camera.Up;
        var view = Matrix4x4.CreateLookAt(direction * 4f, Vector3.Zero, up);
        var projection = Matrix4x4.CreateOrthographic(3.2f, 3.2f, 0.1f, 10f);
        SetModelViewProjection(view * projection);
        Gl.LineWidth(2f);
        Gl.DrawArrays(PrimitiveType.Lines, 0, (uint)OrientationOverlayVertexCount);
        Gl.LineWidth(1f);
        Gl.Enable(EnableCap.DepthTest);
        Gl.Viewport(0, 0, width, height);
    }

    private float GetCurrentOrthographicHalfHeight(Matrix4x4 view, float aspect)
    {
        if (!CurrentRenderBounds.HasValue)
        {
            return 2f * CameraZoom;
        }

        var projectedSize = CurrentRenderBounds.GetProjectedSize(view);
        var halfHeight = MathF.Max(
            projectedSize.Height / 2f,
            projectedSize.Width / (2f * aspect));
        halfHeight = MathF.Max(halfHeight * GetOrthographicPaddingFactor() * CameraZoom, 0.01f);
        return halfHeight;
    }

    private float GetOrthographicPaddingFactor()
    {
        return 1.15f;
    }

    private void ResetDefaultCamera()
    {
        if (!CurrentRenderBounds.HasValue)
        {
            DefaultCameraTarget = Vector3.Zero;
            DefaultCameraOrientation = CreateCameraOrientation(GetCameraDirection(-0.85f, 0.55f));
            DefaultCameraZoom = 1f;
            ResetCamera();
            return;
        }

        DefaultCameraTarget = CurrentRenderBounds.Center;
        var defaultDirection = CurrentRenderBounds.GetDefaultCameraDirection();
        DefaultCameraOrientation = CreateCameraOrientation(defaultDirection);
        DefaultCameraZoom = 1f;
        ResetCamera();
    }

    private void SnapCameraToDirection(Vector3 direction, string label)
    {
        CameraTarget = CurrentRenderBounds.HasValue
            ? CurrentRenderBounds.Center
            : DefaultCameraTarget;
        CameraOrientation = CreateCameraOrientation(direction);
        CameraZoom = 1f;
        var camera = GetCameraFrame();
        SetDiagnostic($"OpenGL: camera snapped {label}, actual {camera.Direction.X:0.00}/{camera.Direction.Y:0.00}/{camera.Direction.Z:0.00}");
        RequestNextFrameRendering();
    }

    private static Quaternion CreateCameraOrientation(Vector3 direction)
    {
        direction = Vector3.Normalize(direction);
        var upReference = MathF.Abs(Vector3.Dot(direction, Vector3.UnitY)) > 0.96f
            ? Vector3.UnitZ
            : Vector3.UnitY;
        var right = Vector3.Normalize(Vector3.Cross(upReference, direction));
        var up = Vector3.Normalize(Vector3.Cross(direction, right));
        var matrix = new Matrix4x4(
            right.X,
            right.Y,
            right.Z,
            0f,
            up.X,
            up.Y,
            up.Z,
            0f,
            direction.X,
            direction.Y,
            direction.Z,
            0f,
            0f,
            0f,
            0f,
            1f);
        return Quaternion.Normalize(Quaternion.CreateFromRotationMatrix(matrix));
    }

    private static Vector3 GetCameraDirection(float yaw, float pitch)
    {
        var cosPitch = MathF.Cos(pitch);
        return Vector3.Normalize(new Vector3(
            MathF.Cos(yaw) * cosPitch,
            MathF.Sin(pitch),
            MathF.Sin(yaw) * cosPitch));
    }

    private static float[] CreateOrientationOverlayVertices()
    {
        var vertices = new List<float>();
        var nifXAxis = Vector3.UnitX;
        var nifYAxis = -Vector3.UnitZ;
        var nifZAxis = Vector3.UnitY;
        var xColor = new Vector3(1f, 0.22f, 0.18f);
        var yColor = new Vector3(0.28f, 0.95f, 0.34f);
        var zColor = new Vector3(0.32f, 0.58f, 1f);
        AddLine(vertices, Vector3.Zero, nifXAxis, xColor);
        AddLine(vertices, Vector3.Zero, nifYAxis, yColor);
        AddLine(vertices, Vector3.Zero, nifZAxis, zColor);
        AddLine(vertices, new Vector3(0.78f, 0.08f, 0f), new Vector3(1.08f, 0.38f, 0f), new Vector3(1f, 0.22f, 0.18f));
        AddLine(vertices, new Vector3(1.08f, 0.08f, 0f), new Vector3(0.78f, 0.38f, 0f), new Vector3(1f, 0.22f, 0.18f));
        AddLine(vertices, new Vector3(0.08f, 0f, -0.78f), new Vector3(0.23f, 0f, -0.95f), yColor);
        AddLine(vertices, new Vector3(0.38f, 0f, -0.78f), new Vector3(0.23f, 0f, -0.95f), yColor);
        AddLine(vertices, new Vector3(0.23f, 0f, -0.95f), new Vector3(0.23f, 0f, -1.12f), yColor);
        AddLine(vertices, new Vector3(0.08f, 0.78f, 0f), new Vector3(0.38f, 0.78f, 0f), zColor);
        AddLine(vertices, new Vector3(0.38f, 0.78f, 0f), new Vector3(0.08f, 1.08f, 0f), zColor);
        AddLine(vertices, new Vector3(0.08f, 1.08f, 0f), new Vector3(0.38f, 1.08f, 0f), zColor);
        return vertices.ToArray();
    }

    private static void AddLine(List<float> vertices, Vector3 start, Vector3 end, Vector3 color)
    {
        AddVertex(vertices, start, color);
        AddVertex(vertices, end, color);
    }

    private static void AddVertex(List<float> vertices, Vector3 position, Vector3 color)
    {
        vertices.Add(position.X);
        vertices.Add(position.Y);
        vertices.Add(position.Z);
        vertices.Add(color.X);
        vertices.Add(color.Y);
        vertices.Add(color.Z);
        vertices.Add(0f);
        vertices.Add(0f);
        vertices.Add(1f);
        vertices.Add(0f);
        vertices.Add(0f);
        vertices.Add(1f);
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

        public Vector3 Center => new Vector3(
            (MinX + MaxX) / 2f,
            (MinY + MaxY) / 2f,
            (MinZ + MaxZ) / 2f);

        public float Radius
        {
            get
            {
                var extents = Extents;
                return MathF.Max(MathF.Sqrt((extents.X * extents.X) + (extents.Y * extents.Y) + (extents.Z * extents.Z)) / 2f, 0.1f);
            }
        }

        private Vector3 Extents => new Vector3(MaxX - MinX, MaxY - MinY, MaxZ - MinZ);

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
            for (var index = 0; index + 11 < vertices.Count; index += 12)
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

        public AssetPreviewProjectedSize GetProjectedSize(Matrix4x4 view)
        {
            var minX = float.PositiveInfinity;
            var maxX = float.NegativeInfinity;
            var minY = float.PositiveInfinity;
            var maxY = float.NegativeInfinity;
            foreach (var corner in GetCorners())
            {
                var projected = Vector3.Transform(corner, view);
                minX = MathF.Min(minX, projected.X);
                maxX = MathF.Max(maxX, projected.X);
                minY = MathF.Min(minY, projected.Y);
                maxY = MathF.Max(maxY, projected.Y);
            }

            if (!IsFinite(minX) || !IsFinite(maxX) ||
                !IsFinite(minY) || !IsFinite(maxY))
            {
                return new AssetPreviewProjectedSize(0.01f, 0.01f);
            }

            return new AssetPreviewProjectedSize(maxX - minX, maxY - minY);
        }

        public Vector3 GetDefaultCameraDirection()
        {
            var extents = Extents;
            var maxExtent = MathF.Max(extents.X, MathF.Max(extents.Y, extents.Z));
            if (maxExtent <= 0.0001f)
            {
                return Vector3.Normalize(new Vector3(0.45f, 0.72f, 0.55f));
            }

            var thinThreshold = maxExtent * 0.15f;
            if (extents.Y <= thinThreshold)
            {
                return Vector3.Normalize(new Vector3(0.14f, 0.97f, -0.18f));
            }

            if (extents.X <= thinThreshold)
            {
                return Vector3.Normalize(new Vector3(0.97f, 0.14f, -0.18f));
            }

            if (extents.Z <= thinThreshold)
            {
                return Vector3.Normalize(new Vector3(0.18f, 0.14f, 0.97f));
            }

            return Vector3.Normalize(new Vector3(0.45f, 0.72f, 0.55f));
        }

        private IEnumerable<Vector3> GetCorners()
        {
            yield return new Vector3(MinX, MinY, MinZ);
            yield return new Vector3(MinX, MinY, MaxZ);
            yield return new Vector3(MinX, MaxY, MinZ);
            yield return new Vector3(MinX, MaxY, MaxZ);
            yield return new Vector3(MaxX, MinY, MinZ);
            yield return new Vector3(MaxX, MinY, MaxZ);
            yield return new Vector3(MaxX, MaxY, MinZ);
            yield return new Vector3(MaxX, MaxY, MaxZ);
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

    private readonly struct AssetPreviewCameraFrame
    {
        public AssetPreviewCameraFrame(Matrix4x4 view, Vector3 right, Vector3 up, Vector3 direction)
        {
            View = view;
            Right = right;
            Up = up;
            Direction = direction;
        }

        public Matrix4x4 View { get; }

        public Vector3 Right { get; }

        public Vector3 Up { get; }

        public Vector3 Direction { get; }
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
