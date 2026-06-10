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
        renderMesh.Vertices[10].ShouldBe(0.9f, 0.0001f);
        renderMesh.Vertices[2].ShouldBe(0f, 0.0001f);
        renderMesh.Vertices[11].ShouldBe(0f, 0.0001f);
    }

    [Fact]
    public void CreateRenderMesh_DoesNotRemapFallbackStopSign()
    {
        var factory = new AssetPreviewRenderMeshFactory(new LoggerConfiguration().CreateLogger());

        var renderMesh = factory.CreateRenderMesh(null);

        renderMesh.Vertices[1].ShouldBe(0f);
        renderMesh.Vertices[10].ShouldBe(0.9f);
        renderMesh.Vertices[11].ShouldBe(0f);
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
        renderMesh.Vertices[10].ShouldBe(0.9f, 0.0001f);
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
    public void CreateRenderMesh_UsesTextureHintForBasketPreviewColor()
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

        renderMesh.Vertices[3].ShouldBe(0.58f, 0.0001f);
        renderMesh.Vertices[4].ShouldBe(0.43f, 0.0001f);
        renderMesh.Vertices[5].ShouldBe(0.26f, 0.0001f);
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
