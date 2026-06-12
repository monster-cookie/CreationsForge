using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Services;
using Serilog;
using Shouldly;

namespace CreationsForge.PresentationTests.Services;

public class AssetPreviewRenderMeshFactoryTests
{
    [Fact]
    public void CreateRenderMesh_MapsNifZAxisToRenderYAxis()
    {
        var factory = new AssetPreviewRenderMeshFactory(new LoggerConfiguration().CreateLogger());
        var model = new AssetPreviewModelDTO
        {
            DisplayName = "Preview",
            SourcePath = "Meshes/Preview.nif",
            Meshes =
            {
                new AssetPreviewMeshDTO
                {
                    Name = "Mesh",
                    MaterialName = "Material",
                    Vertices =
                    {
                        CreateVertex(0f, 0f, -5f),
                        CreateVertex(0f, 0f, 5f),
                        CreateVertex(1f, 0f, 0f)
                    },
                    Indices =
                    {
                        0,
                        1,
                        2
                    }
                }
            }
        };

        var renderMesh = factory.CreateRenderMesh(model);

        renderMesh.Vertices[1].ShouldBe(-0.9f, 0.0001f);
        renderMesh.Vertices[13].ShouldBe(0.9f, 0.0001f);
        renderMesh.Vertices[2].ShouldBe(0f, 0.0001f);
        renderMesh.Vertices[14].ShouldBe(0f, 0.0001f);
    }

    [Fact]
    public void CreateRenderMesh_DoesNotRemapFallbackStopSign()
    {
        var factory = new AssetPreviewRenderMeshFactory(new LoggerConfiguration().CreateLogger());

        var renderMesh = factory.CreateRenderMesh(null);

        renderMesh.Vertices[1].ShouldBe(0f);
        renderMesh.Vertices[13].ShouldBe(0.9f);
        renderMesh.Vertices[14].ShouldBe(0f);
    }

    [Fact]
    public void CreateRenderMesh_AppliesMeshFilterBeforeCoordinateConversion()
    {
        var factory = new AssetPreviewRenderMeshFactory(new LoggerConfiguration().CreateLogger());
        var model = new AssetPreviewModelDTO
        {
            DisplayName = "Preview",
            SourcePath = "Meshes/Preview.nif",
            Meshes =
            {
                CreateMesh("First", -2f),
                CreateMesh("Second", 8f)
            }
        };

        var renderMesh = factory.CreateRenderMesh(model, new AssetPreviewRenderOptions
        {
            MeshIndex = 1
        });

        renderMesh.Vertices[1].ShouldBe(-0.9f, 0.0001f);
        renderMesh.Vertices[13].ShouldBe(0.9f, 0.0001f);
    }

    [Fact]
    public void CreateRenderMesh_UsesDecodedNormalsWhenAvailable()
    {
        var factory = new AssetPreviewRenderMeshFactory(new LoggerConfiguration().CreateLogger());
        var model = new AssetPreviewModelDTO
        {
            DisplayName = "Preview",
            SourcePath = "Meshes/Preview.nif",
            Meshes =
            {
                new AssetPreviewMeshDTO
                {
                    Name = "Mesh",
                    MaterialName = "Material",
                    Vertices =
                    {
                        CreateVertex(0f, 0f, -1f),
                        CreateVertex(0f, 0f, 1f),
                        CreateVertex(1f, 0f, 0f)
                    },
                    Indices =
                    {
                        0,
                        1,
                        2
                    }
                }
            }
        };

        var renderMesh = factory.CreateRenderMesh(model);

        renderMesh.Vertices[6].ShouldBe(0f, 0.0001f);
        renderMesh.Vertices[7].ShouldBe(1f, 0.0001f);
        renderMesh.Vertices[8].ShouldBe(0f, 0.0001f);
    }

    [Fact]
    public void CreateRenderMesh_UsesMaterialMetadataForPreviewColor()
    {
        var factory = new AssetPreviewRenderMeshFactory(new LoggerConfiguration().CreateLogger());
        var mesh = CreateMesh("Body", 8f);
        mesh.MaterialName = @"Materials\SetDressing\BabyBottle\BabyBottleDirty01empty.BGEM";
        mesh.TexturePath = @"textures\SetDressing\BabyBottle\BabyBottleDirty01_d.dds";
        var model = new AssetPreviewModelDTO
        {
            DisplayName = "Preview",
            SourcePath = "Meshes/Preview.nif",
            Meshes =
            {
                mesh
            }
        };

        var renderMesh = factory.CreateRenderMesh(model);

        renderMesh.Vertices[3].ShouldBe(0.70f, 0.0001f);
        renderMesh.Vertices[4].ShouldBe(0.72f, 0.0001f);
        renderMesh.Vertices[5].ShouldBe(0.72f, 0.0001f);
    }

    [Fact]
    public void CreateRenderMesh_DoesNotUseTextureHintForPreviewColor()
    {
        var factory = new AssetPreviewRenderMeshFactory(new LoggerConfiguration().CreateLogger());
        var mesh = CreateMesh("Basket04", 8f);
        mesh.MaterialName = "Basket04";
        mesh.TexturePath = @"textures\clutter\Basket01.dds";
        var model = new AssetPreviewModelDTO
        {
            DisplayName = "Preview",
            SourcePath = "Meshes/Preview.nif",
            Meshes =
            {
                mesh
            }
        };

        var renderMesh = factory.CreateRenderMesh(model);

        renderMesh.Vertices[3].ShouldBe(0.70f, 0.0001f);
        renderMesh.Vertices[4].ShouldBe(0.72f, 0.0001f);
        renderMesh.Vertices[5].ShouldBe(0.72f, 0.0001f);
    }

    [Fact]
    public void CreateRenderMesh_TracksTextureMetadataForDiagnostics()
    {
        var factory = new AssetPreviewRenderMeshFactory(new LoggerConfiguration().CreateLogger());
        var mesh = CreateMesh("Basket04", 8f);
        mesh.TexturePath = @"textures\clutter\Basket01.dds";
        var model = new AssetPreviewModelDTO
        {
            DisplayName = "Preview",
            SourcePath = "Meshes/Preview.nif",
            Meshes =
            {
                mesh
            }
        };

        var renderMesh = factory.CreateRenderMesh(model);

        renderMesh.TexturePaths.ShouldBe([@"textures\clutter\Basket01.dds"]);
    }

    [Fact]
    public void CreateRenderMesh_TracksLoadedTextureForMeshPart()
    {
        var factory = new AssetPreviewRenderMeshFactory(new LoggerConfiguration().CreateLogger());
        var mesh = CreateMesh("Basket04", 8f);
        mesh.TexturePath = @"textures\clutter\Basket01.dds";
        mesh.Texture = new AssetPreviewTextureDTO
        {
            Path = @"textures\clutter\Basket01.dds",
            Data = [1, 2, 3, 4]
        };
        var model = new AssetPreviewModelDTO
        {
            DisplayName = "Preview",
            SourcePath = "Meshes/Preview.nif",
            Meshes =
            {
                mesh
            }
        };

        var renderMesh = factory.CreateRenderMesh(model);

        renderMesh.Textures.Count.ShouldBe(1);
        renderMesh.Textures[0].Path.ShouldBe(@"textures\clutter\Basket01.dds");
        renderMesh.Textures[0].Data.ShouldBe([1, 2, 3, 4]);
        renderMesh.MeshParts.Count.ShouldBe(1);
        renderMesh.MeshParts[0].TextureIndex.ShouldBe(0);
    }

    [Fact]
    public void CreateRenderMesh_TracksMaterialTintForMeshPart()
    {
        var factory = new AssetPreviewRenderMeshFactory(new LoggerConfiguration().CreateLogger());
        var mesh = CreateMesh("Basket04", 8f);
        mesh.MaterialTintRed = 0.6f;
        mesh.MaterialTintGreen = 0.5f;
        mesh.MaterialTintBlue = 0.4f;
        mesh.MaterialTintAlpha = 0f;
        var model = new AssetPreviewModelDTO
        {
            DisplayName = "Preview",
            SourcePath = "Meshes/Preview.nif",
            Meshes =
            {
                mesh
            }
        };

        var renderMesh = factory.CreateRenderMesh(model);

        renderMesh.MeshParts.Count.ShouldBe(1);
        renderMesh.MeshParts[0].MaterialTintRed.ShouldBe(0.6f);
        renderMesh.MeshParts[0].MaterialTintGreen.ShouldBe(0.5f);
        renderMesh.MeshParts[0].MaterialTintBlue.ShouldBe(0.4f);
        renderMesh.MeshParts[0].MaterialTintAlpha.ShouldBe(0f);
    }

    [Fact]
    public void CreateRenderMesh_TracksOverlayTextureForMeshPart()
    {
        var factory = new AssetPreviewRenderMeshFactory(new LoggerConfiguration().CreateLogger());
        var mesh = CreateMesh("Letters", 8f);
        mesh.OverlayTexturePath = @"textures\setdressing\books\letters.dds";
        mesh.OverlayTexture = new AssetPreviewTextureDTO
        {
            Path = @"textures\setdressing\books\letters.dds",
            Data = [4, 3, 2, 1]
        };
        mesh.UseAdditiveBlend = true;
        var model = new AssetPreviewModelDTO
        {
            DisplayName = "Preview",
            SourcePath = "Meshes/Preview.nif",
            Meshes =
            {
                mesh
            }
        };

        var renderMesh = factory.CreateRenderMesh(model);

        renderMesh.TexturePaths.ShouldBe([@"textures\setdressing\books\letters.dds"]);
        renderMesh.Textures.Count.ShouldBe(1);
        renderMesh.Textures[0].Path.ShouldBe(@"textures\setdressing\books\letters.dds");
        renderMesh.MeshParts.Count.ShouldBe(1);
        renderMesh.MeshParts[0].TextureIndex.ShouldBeNull();
        renderMesh.MeshParts[0].OverlayTextureIndex.ShouldBe(0);
        renderMesh.MeshParts[0].UseAdditiveBlend.ShouldBeTrue();
    }

    [Fact]
    public void CreateRenderMesh_TracksDecalOpacityTextureAndTintForMeshPart()
    {
        var factory = new AssetPreviewRenderMeshFactory(new LoggerConfiguration().CreateLogger());
        var mesh = CreateMesh("Letters", 8f);
        SetUsableUvs(mesh);
        mesh.IsDecal = true;
        mesh.DecalOpacityTexturePath = @"textures\common\decal\letters_opacity.dds";
        mesh.DecalOpacityTexture = new AssetPreviewTextureDTO
        {
            Path = @"textures\common\decal\letters_opacity.dds",
            Data = [9, 8, 7, 6]
        };
        mesh.DecalTintRed = 0.9f;
        mesh.DecalTintGreen = 0.8f;
        mesh.DecalTintBlue = 0.7f;
        mesh.DecalOpacity = 0.6f;
        mesh.DecalUvScaleU = -0.5f;
        mesh.DecalUvScaleV = 0.3f;
        mesh.DecalUvOffsetU = 0.1f;
        mesh.DecalUvOffsetV = 0.505f;
        var model = new AssetPreviewModelDTO
        {
            DisplayName = "Preview",
            SourcePath = "Meshes/Preview.nif",
            Meshes =
            {
                mesh
            }
        };

        var renderMesh = factory.CreateRenderMesh(model);

        renderMesh.TexturePaths.ShouldBe([@"textures\common\decal\letters_opacity.dds"]);
        renderMesh.Textures.Count.ShouldBe(1);
        renderMesh.Textures[0].Path.ShouldBe(@"textures\common\decal\letters_opacity.dds");
        renderMesh.MeshParts.Count.ShouldBe(1);
        renderMesh.MeshParts[0].DecalOpacityTextureIndex.ShouldBe(0);
        renderMesh.MeshParts[0].IsDecal.ShouldBeTrue();
        renderMesh.MeshParts[0].DecalTintRed.ShouldBe(0.9f);
        renderMesh.MeshParts[0].DecalTintGreen.ShouldBe(0.8f);
        renderMesh.MeshParts[0].DecalTintBlue.ShouldBe(0.7f);
        renderMesh.MeshParts[0].DecalOpacity.ShouldBe(0.6f);
        renderMesh.MeshParts[0].DecalUvScaleU.ShouldBe(-0.5f);
        renderMesh.MeshParts[0].DecalUvScaleV.ShouldBe(0.3f);
        renderMesh.MeshParts[0].DecalUvOffsetU.ShouldBe(0.1f);
        renderMesh.MeshParts[0].DecalUvOffsetV.ShouldBe(0.505f);
    }

    [Fact]
    public void CreateRenderMesh_PreservesVertexAlpha()
    {
        var factory = new AssetPreviewRenderMeshFactory(new LoggerConfiguration().CreateLogger());
        var mesh = CreateMesh("Letters", 8f);
        mesh.Vertices[0].Alpha = 0.25f;
        var model = new AssetPreviewModelDTO
        {
            DisplayName = "Preview",
            SourcePath = "Meshes/Preview.nif",
            Meshes =
            {
                mesh
            }
        };

        var renderMesh = factory.CreateRenderMesh(model);

        renderMesh.Vertices[11].ShouldBe(0.25f, 0.0001f);
    }

    [Fact]
    public void CreateRenderMesh_SkipsDecalOpacityTextureForMeshPartWithUnusableUvs()
    {
        var factory = new AssetPreviewRenderMeshFactory(new LoggerConfiguration().CreateLogger());
        var mesh = CreateMesh("Letters", 8f);
        mesh.IsDecal = true;
        mesh.DecalOpacityTexturePath = @"textures\common\decal\letters_opacity.dds";
        mesh.DecalOpacityTexture = new AssetPreviewTextureDTO
        {
            Path = @"textures\common\decal\letters_opacity.dds",
            Data = [9, 8, 7, 6]
        };
        var model = new AssetPreviewModelDTO
        {
            DisplayName = "Preview",
            SourcePath = "Meshes/Preview.nif",
            Meshes =
            {
                mesh
            }
        };

        var renderMesh = factory.CreateRenderMesh(model);

        renderMesh.TexturePaths.ShouldBe([@"textures\common\decal\letters_opacity.dds"]);
        renderMesh.Textures.Count.ShouldBe(1);
        renderMesh.MeshParts.Count.ShouldBe(1);
        renderMesh.MeshParts[0].DecalOpacityTextureIndex.ShouldBeNull();
        renderMesh.MeshParts[0].IsDecal.ShouldBeTrue();
    }

    [Fact]
    public void CreateRenderMesh_SkipsInvisibleMeshes()
    {
        var factory = new AssetPreviewRenderMeshFactory(new LoggerConfiguration().CreateLogger());
        var visibleMesh = CreateMesh("Visible", 8f);
        var invisibleMesh = CreateMesh("Invisible", 10f);
        invisibleMesh.IsInvisible = true;
        var model = new AssetPreviewModelDTO
        {
            DisplayName = "Preview",
            SourcePath = "Meshes/Preview.nif",
            Meshes =
            {
                invisibleMesh,
                visibleMesh
            }
        };

        var renderMesh = factory.CreateRenderMesh(model);

        renderMesh.MeshParts.Count.ShouldBe(1);
        renderMesh.Vertices.Count.ShouldBe(36);
    }

    private static AssetPreviewMeshDTO CreateMesh(string name, float zOffset)
    {
        return new AssetPreviewMeshDTO
        {
            Name = name,
            MaterialName = name,
            Vertices =
            {
                CreateVertex(0f, 0f, zOffset - 1f),
                CreateVertex(0f, 0f, zOffset + 1f),
                CreateVertex(1f, 0f, zOffset)
            },
            Indices =
            {
                0,
                1,
                2
            }
        };
    }

    private static void SetUsableUvs(AssetPreviewMeshDTO mesh)
    {
        mesh.Vertices[0].UV = new AssetPreviewUVDTO
        {
            U = 0f,
            V = 0f
        };
        mesh.Vertices[1].UV = new AssetPreviewUVDTO
        {
            U = 1f,
            V = 0f
        };
        mesh.Vertices[2].UV = new AssetPreviewUVDTO
        {
            U = 0f,
            V = 1f
        };
    }

    private static AssetPreviewVertexDTO CreateVertex(float x, float y, float z)
    {
        return new AssetPreviewVertexDTO
        {
            Position = new AssetPreviewVector3DTO
            {
                X = x,
                Y = y,
                Z = z
            },
            Normal = new AssetPreviewVector3DTO
            {
                X = 0f,
                Y = 0f,
                Z = 1f
            },
            UV = new AssetPreviewUVDTO
            {
                U = 0f,
                V = 0f
            }
        };
    }
}
