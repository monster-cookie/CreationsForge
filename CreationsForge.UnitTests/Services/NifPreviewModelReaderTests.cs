using System.Globalization;
using System.Text;
using CreationsForge.Bethesda.Assets.Nif;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

public class NifPreviewModelReaderTests
{
    [Fact]
    public void TryRead_ReturnsMeshForMinimalBSTriShape()
    {
        var reader = new NifPreviewModelReader();

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/Preview.nif",
            DisplayName = "Preview",
            Data = CreateMinimalBSTriShapeNif()
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes.Count.ShouldBe(1);
        result.Model.Meshes[0].Vertices.Count.ShouldBe(3);
        result.Model.Meshes[0].Indices.ShouldBe([0, 1, 2]);
        result.Model.Meshes[0].Vertices[2].Position.Y.ShouldBe(1f);
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("descriptor 0x", StringComparison.Ordinal));
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("first vertex bytes", StringComparison.Ordinal));
    }

    [Fact]
    public void TryRead_ReturnsMeshForStarfieldBSGeometryBlock()
    {
        var reader = new NifPreviewModelReader();

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/Items/digipic/DigiPic.nif",
            DisplayName = "DigiPic",
            Data = CreateMinimalNifWithBlock("BSGeometry", CreateBSTriShapeBlock(0f, 0f, 0f, 0f), bethesdaVersion: 172U)
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes.Count.ShouldBe(1);
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("BSGeometry block", StringComparison.Ordinal));
    }

    [Fact]
    public void TryRead_ReturnsMeshForStarfieldBSGeometryWithoutExplicitDataSize()
    {
        var reader = new NifPreviewModelReader();

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/Items/digipic/DigiPic.nif",
            DisplayName = "DigiPic",
            Data = CreateMinimalNifWithBlock("BSGeometry", CreateStarfieldBSGeometryBlockWithoutExplicitDataSize(), bethesdaVersion: 172U)
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes.Count.ShouldBe(1);
        result.Model.Meshes[0].Vertices.Count.ShouldBe(3);
        result.Model.Meshes[0].Indices.ShouldBe([0, 1, 2]);
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("count layout StarfieldGeometry", StringComparison.Ordinal));
    }

    [Fact]
    public void TryRead_ProbesPaddedStarfieldBSGeometryCountFieldsAfterVertexDescriptor()
    {
        var reader = new NifPreviewModelReader();

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/Items/digipic/DigiPic.nif",
            DisplayName = "DigiPic",
            Data = CreateMinimalNifWithBlock("BSGeometry", CreateStarfieldBSGeometryBlockWithPaddedCountsAfterDescriptor(), bethesdaVersion: 172U)
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes.Count.ShouldBe(1);
        result.Model.Meshes[0].Vertices.Count.ShouldBe(3);
        result.Model.Meshes[0].Indices.ShouldBe([0, 1, 2]);
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("count layout StarfieldGeometry", StringComparison.Ordinal));
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("count offset 4", StringComparison.Ordinal));
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("stride bytes 20", StringComparison.Ordinal));
    }

    [Fact]
    public void TryRead_PrefersAnchoredStarfieldBSGeometryDescriptorOverEarlierFalseDescriptor()
    {
        var reader = new NifPreviewModelReader();

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/Items/digipic/DigiPic.nif",
            DisplayName = "DigiPic",
            Data = CreateMinimalNifWithBlock("BSGeometry", CreateStarfieldBSGeometryBlockWithFalsePreDescriptorBytes(), bethesdaVersion: 172U)
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes.Count.ShouldBe(1);
        result.Model.Meshes[0].Vertices.Count.ShouldBe(3);
        result.Model.Meshes[0].Indices.ShouldBe([0, 1, 2]);
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("count layout StarfieldGeometry", StringComparison.Ordinal));
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("selected from 1 candidate mesh layout(s), anchored offsets 1", StringComparison.Ordinal));
    }

    [Fact]
    public void TryRead_LoadsStarfieldExternalGeometryMeshReference()
    {
        var reader = new NifPreviewModelReader();
        const string geometryPath = @"geometries\cf623091ecaffe5a43fa\249816728d4437f890e8.mesh";

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/Items/digipic/DigiPic.nif",
            DisplayName = "DigiPic",
            Data = CreateMinimalNifWithBlock("BSGeometry", CreateStarfieldBSGeometryBlockWithExternalGeometryReference(), bethesdaVersion: 173U),
            ResolveExternalAsset = path => string.Equals(path, geometryPath, StringComparison.OrdinalIgnoreCase)
                ? CreateStarfieldGeometryMesh()
                : null
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes.Count.ShouldBe(1);
        result.Model.Meshes[0].Vertices.Count.ShouldBe(3);
        result.Model.Meshes[0].Indices.ShouldBe([0, 1, 2]);
        result.Model.Meshes[0].Vertices[0].Position.X.ShouldBe(0f, 0.0001f);
        result.Model.Meshes[0].Vertices[0].Position.Y.ShouldBe(0f, 0.0001f);
        result.Model.Meshes[0].Vertices[0].Position.Z.ShouldBe(0f, 0.0001f);
        result.Model.Meshes[0].Vertices[1].Position.X.ShouldBe(1.00003f, 0.0001f);
        result.Model.Meshes[0].Vertices[2].Position.Y.ShouldBe(1.00003f, 0.0001f);
        result.Model.Meshes[0].Vertices.ShouldAllBe(vertex =>
            vertex.Normal.X == 0f &&
            vertex.Normal.Y == 0f &&
            vertex.Normal.Z == 0f);
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("external Starfield geometry", StringComparison.Ordinal));
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("scale 2", StringComparison.Ordinal));
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("position stride 6", StringComparison.Ordinal));
        result.Diagnostics.ShouldNotContain(diagnostic => diagnostic.Contains("geometry bounds metadata", StringComparison.Ordinal));
    }

    [Fact]
    public void TryRead_AppliesStarfieldExternalGeometryMeshWorldTransform()
    {
        var reader = new NifPreviewModelReader();
        const string geometryPath = @"geometries\cf623091ecaffe5a43fa\249816728d4437f890e8.mesh";

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/Items/digipic/DigiPic.nif",
            DisplayName = "DigiPic",
            Data = CreateMinimalNifWithBlock(
                "BSGeometry",
                CreateStarfieldBSGeometryBlockWithExternalGeometryReference(translationX: 10f, translationY: 20f, translationZ: 30f),
                bethesdaVersion: 173U),
            ResolveExternalAsset = path => string.Equals(path, geometryPath, StringComparison.OrdinalIgnoreCase)
                ? CreateStarfieldGeometryMesh()
                : null
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes.Count.ShouldBe(1);
        result.Model.Meshes[0].Vertices[0].Position.X.ShouldBe(10f, 0.0001f);
        result.Model.Meshes[0].Vertices[0].Position.Y.ShouldBe(20f, 0.0001f);
        result.Model.Meshes[0].Vertices[0].Position.Z.ShouldBe(30f, 0.0001f);
        result.Model.Meshes[0].Vertices[1].Position.X.ShouldBe(11.00003f, 0.0001f);
        result.Model.Meshes[0].Vertices[2].Position.Y.ShouldBe(21.00003f, 0.0001f);
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("raw mesh-space bounds", StringComparison.Ordinal));
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("applied NIF world transform", StringComparison.Ordinal));
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("preview bounds", StringComparison.Ordinal));
    }

    [Fact]
    public void TryRead_DecodesStarfieldExternalGeometryUvStream()
    {
        var reader = new NifPreviewModelReader();
        const string geometryPath = @"geometries\cf623091ecaffe5a43fa\249816728d4437f890e8.mesh";

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/Items/digipic/DigiPic.nif",
            DisplayName = "DigiPic",
            Data = CreateMinimalNifWithBlock("BSGeometry", CreateStarfieldBSGeometryBlockWithExternalGeometryReference(), bethesdaVersion: 173U),
            ResolveExternalAsset = path => string.Equals(path, geometryPath, StringComparison.OrdinalIgnoreCase)
                ? CreateStarfieldGeometryMeshWithUvStream()
                : null
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes.Count.ShouldBe(1);
        result.Model.Meshes[0].Vertices[0].UV.U.ShouldBe(0.25f, 0.0001f);
        result.Model.Meshes[0].Vertices[0].UV.V.ShouldBe(0.75f, 0.0001f);
        result.Model.Meshes[0].Vertices[1].UV.U.ShouldBe(1f, 0.0001f);
        result.Model.Meshes[0].Vertices[2].UV.V.ShouldBe(1f, 0.0001f);
        result.Model.Meshes[0].Vertices[0].Alpha.ShouldBe(0f, 0.0001f);
        result.Model.Meshes[0].Vertices[1].Alpha.ShouldBe(0.5019f, 0.0001f);
        result.Model.Meshes[0].Vertices[2].Alpha.ShouldBe(1f, 0.0001f);
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("UV stream decoded 3 half-precision UVs", StringComparison.Ordinal));
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("vertex color stream decoded 3 BGRA colors", StringComparison.Ordinal));
    }

    [Fact]
    public void TryRead_AttachesMaterialTextureToStarfieldExternalGeometryMesh()
    {
        var reader = new NifPreviewModelReader();
        const string geometryPath = @"geometries\cf623091ecaffe5a43fa\249816728d4437f890e8.mesh";
        const string materialPath = @"Materials\Cinimatics\DigiPic\DigiPic_Base.mat";

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/Items/digipic/DigiPic.nif",
            DisplayName = "DigiPic",
            Data = CreateMinimalNifWithBlocks(
                [
                    ("BSGeometry", CreateStarfieldBSGeometryBlockWithExternalGeometryReference(shaderProperty: 1)),
                    ("BSLightingShaderProperty", CreateLightingShaderPropertyBlock(1, -1))
                ],
                [
                    "DigiPick_Final:0",
                    materialPath
                ],
                bethesdaVersion: 173U),
            ResolveExternalAsset = path =>
            {
                if (string.Equals(path, geometryPath, StringComparison.OrdinalIgnoreCase))
                {
                    return CreateStarfieldGeometryMesh();
                }

                return string.Equals(path, materialPath, StringComparison.OrdinalIgnoreCase)
                    ? CreateStarfieldMaterialFile(@"Textures\\Cinimatics\\DigiPic\\DigiPick_Material_color.DDS")
                    : null;
            }
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes.Count.ShouldBe(1);
        result.Model.Meshes[0].Name.ShouldBe("DigiPick_Final:0");
        result.Model.Meshes[0].MaterialName.ShouldBe(materialPath);
        result.Model.Meshes[0].TexturePath.ShouldBe(@"Textures\Cinimatics\DigiPic\DigiPick_Material_color.DDS");
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("external material: Materials\\Cinimatics\\DigiPic\\DigiPic_Base.mat, texture Textures\\Cinimatics\\DigiPic\\DigiPick_Material_color.DDS", StringComparison.Ordinal));
    }

    [Fact]
    public void TryRead_ReturnsFailureForInvalidStarfieldBSGeometryWithoutExplicitDataSize()
    {
        var reader = new NifPreviewModelReader();

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/Items/digipic/DigiPic.nif",
            DisplayName = "DigiPic",
            Data = CreateMinimalNifWithBlock("BSGeometry", CreateInvalidStarfieldBSGeometryBlockWithoutExplicitDataSize(), bethesdaVersion: 172U)
        });

        result.IsSuccess.ShouldBeFalse();
        result.StatusMessage.ShouldContain("No supported preview geometry");
        result.StatusMessage.ShouldContain("BSGeometry");
    }

    [Fact]
    public void TryRead_ReturnsMeshForBSTriShapeWithoutProcessScriptHeader()
    {
        var reader = new NifPreviewModelReader();

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/Preview.nif",
            DisplayName = "Preview",
            Data = CreateMinimalBSTriShapeNif(includeProcessScript: false)
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes.Count.ShouldBe(1);
        result.Model.Meshes[0].Vertices.Count.ShouldBe(3);
        result.Model.Meshes[0].Indices.ShouldBe([0, 1, 2]);
    }

    [Fact]
    public void TryRead_ReturnsMeshForSkyrimSpecialEditionStreamVersion()
    {
        var reader = new NifPreviewModelReader();

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/Clutter/Basket04.NIF",
            DisplayName = "Basket04",
            Data = CreateMinimalBSTriShapeNif(bethesdaVersion: 100U)
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes.Count.ShouldBe(1);
        result.Model.Meshes[0].Vertices.Count.ShouldBe(3);
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("Header user 12, Bethesda 100", StringComparison.Ordinal));
    }

    [Fact]
    public void TryRead_UsesSkyrimSpecialEditionBSTriShapeLayout()
    {
        var reader = new NifPreviewModelReader();

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/Clutter/Basket04.NIF",
            DisplayName = "Basket04",
            Data = CreateSkyrimSpecialEditionLayoutBSTriShapeNif()
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes.Count.ShouldBe(1);
        result.Model.Meshes[0].Vertices.Count.ShouldBe(3);
        result.Model.Meshes[0].Vertices[0].Position.X.ShouldBe(-1f);
        result.Model.Meshes[0].Vertices[2].Position.Y.ShouldBe(1f);
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("parsed with Skyrim SSE layout", StringComparison.Ordinal));
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("count layout SkyrimSpecialEdition", StringComparison.Ordinal));
    }

    [Fact]
    public void TryRead_UsesFloatPositionsForSkyrimSpecialEditionStrideSevenShape()
    {
        var reader = new NifPreviewModelReader();

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/Clutter/Preview.NIF",
            DisplayName = "Preview",
            Data = CreateSkyrimSpecialEditionStrideSevenBSTriShapeNif()
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes.Count.ShouldBe(1);
        result.Model.Meshes[0].Vertices[0].Position.X.ShouldBe(-1f);
        result.Model.Meshes[0].Vertices[1].Position.X.ShouldBe(1f);
        result.Model.Meshes[0].Vertices[2].Position.Y.ShouldBe(1f);
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("position format Float3", StringComparison.Ordinal));
    }

    [Fact]
    public void TryRead_ReturnsMeshWhenHeaderHasUnknownBytesBeforeBlockTables()
    {
        var reader = new NifPreviewModelReader();

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/Preview.nif",
            DisplayName = "Preview",
            Data = CreateMinimalBSTriShapeNif(extraBytesBeforeTables: [0x13, 0x37, 0x99, 0x42])
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes.Count.ShouldBe(1);
        result.Model.Meshes[0].Indices.ShouldBe([0, 1, 2]);
    }

    [Fact]
    public void TryRead_ReturnsMeshForHalfPositionBSTriShape()
    {
        var reader = new NifPreviewModelReader();

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/Preview.nif",
            DisplayName = "Preview",
            Data = CreateMinimalHalfPositionBSTriShapeNif()
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes.Count.ShouldBe(1);
        result.Model.Meshes[0].Vertices.Count.ShouldBe(3);
        result.Model.Meshes[0].Vertices[2].Position.Y.ShouldBe(1f);
    }

    [Fact]
    public void TryRead_ReadsVertexAttributesFromDescriptorOffsets()
    {
        var reader = new NifPreviewModelReader();

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/Preview.nif",
            DisplayName = "Preview",
            Data = CreateOffsetVertexAttributeBSTriShapeNif()
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes.Count.ShouldBe(1);
        result.Model.Meshes[0].Vertices[0].UV.U.ShouldBe(0.25f);
        result.Model.Meshes[0].Vertices[0].UV.V.ShouldBe(0.75f);
        result.Model.Meshes[0].Vertices[0].Normal.X.ShouldBe(0f, 0.01f);
        result.Model.Meshes[0].Vertices[0].Normal.Y.ShouldBe(0f, 0.01f);
        result.Model.Meshes[0].Vertices[0].Normal.Z.ShouldBeGreaterThan(0.99f);
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("uv offset 12, normal offset 16", StringComparison.Ordinal));
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("normals: 3/3 valid", StringComparison.Ordinal));
    }

    [Fact]
    public void TryRead_AppliesNiAVObjectTransformBeforeReturningMesh()
    {
        var reader = new NifPreviewModelReader();

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/Preview.nif",
            DisplayName = "Preview",
            Data = CreateMinimalTransformedBSTriShapeNif()
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes.Count.ShouldBe(1);
        result.Model.Meshes[0].Vertices[0].Position.X.ShouldBe(8f);
        result.Model.Meshes[0].Vertices[0].Position.Y.ShouldBe(3f);
        result.Model.Meshes[0].Vertices[2].Position.Y.ShouldBe(7f);
    }

    [Fact]
    public void TryRead_AppliesInheritedNiNodeTransformBeforeReturningMesh()
    {
        var reader = new NifPreviewModelReader();

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/Preview.nif",
            DisplayName = "Preview",
            Data = CreateParentedTransformedBSTriShapeNif()
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes.Count.ShouldBe(2);
        result.Model.Meshes[0].Vertices[0].Position.Z.ShouldBe(2f);
        result.Model.Meshes[1].Vertices[0].Position.Z.ShouldBe(-1f);
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("BSTriShape block 5: object transform header", StringComparison.Ordinal));
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("NiNode block 0: children 2, 8", StringComparison.Ordinal));
    }

    [Fact]
    public void TryRead_AttachesShaderMaterialAndTextureMetadata()
    {
        var reader = new NifPreviewModelReader();

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/Preview.nif",
            DisplayName = "Preview",
            Data = CreateMaterialLinkedBSTriShapeNif()
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes.Count.ShouldBe(1);
        result.Model.Meshes[0].Name.ShouldBe("BottleTop:20");
        result.Model.Meshes[0].MaterialName.ShouldBe(@"Materials\SetDressing\BabyBottle\BabyBottleDirty01.BGSM");
        result.Model.Meshes[0].TexturePath.ShouldBe(@"textures\SetDressing\BabyBottle\BabyBottleDirty01_d.dds");
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("material Materials\\SetDressing\\BabyBottle\\BabyBottleDirty01.BGSM", StringComparison.Ordinal));
    }

    [Fact]
    public void TryRead_ResolvesPreviewTextureFromStarfieldMaterialFile()
    {
        var reader = new NifPreviewModelReader();
        const string materialPath = @"Materials\Cinimatics\DigiPic\DigiPic_Base.mat";

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/Items/digipic/DigiPic.nif",
            DisplayName = "DigiPic",
            Data = CreateMaterialLinkedBSTriShapeNif(materialPath),
            ResolveExternalAsset = path => string.Equals(path, materialPath, StringComparison.OrdinalIgnoreCase)
                ? CreateStarfieldMaterialFile(
                    @"textures\Cinimatics\DigiPic\DigiPic_Base_n.dds",
                    @"textures\Cinimatics\DigiPic\DigiPic_Base_color.dds")
                : null
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes.Count.ShouldBe(1);
        result.Model.Meshes[0].MaterialName.ShouldBe(materialPath);
        result.Model.Meshes[0].TexturePath.ShouldBe(@"textures\Cinimatics\DigiPic\DigiPic_Base_color.dds");
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("resolved preview texture textures\\Cinimatics\\DigiPic\\DigiPic_Base_color.dds", StringComparison.Ordinal));
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("unsupported material features:", StringComparison.Ordinal));
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("effect", StringComparison.Ordinal));
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("glass", StringComparison.Ordinal));
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("layered", StringComparison.Ordinal));
    }

    [Fact]
    public void TryRead_SkipsStarfieldMaterialDatabaseWhenMaterialFileProvidesColorTexture()
    {
        var reader = new NifPreviewModelReader();
        const string materialPath = @"Materials\Cinimatics\DigiPic\DigiPic_Base.mat";
        const string materialDatabasePath = "materials/materialsbeta.cdb";

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/Items/digipic/DigiPic.nif",
            DisplayName = "DigiPic",
            Data = CreateMaterialLinkedBSTriShapeNif(materialPath),
            ResolveExternalAsset = path =>
            {
                if (string.Equals(path, materialPath, StringComparison.OrdinalIgnoreCase))
                {
                    return CreateStarfieldMaterialFile(@"Textures\Cinimatics\DigiPic\DigiPick_Material_color.DDS");
                }

                return string.Equals(path, materialDatabasePath, StringComparison.OrdinalIgnoreCase)
                    ? CreateStarfieldMaterialDatabase(@"Textures\Verified\DigiPic\DigiPick_Material_color.DDS")
                    : null;
            }
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes.Count.ShouldBe(1);
        result.Model.Meshes[0].TexturePath.ShouldBe(@"Textures\Cinimatics\DigiPic\DigiPick_Material_color.DDS");
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("from material", StringComparison.Ordinal));
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("skipped because material already provided preview color texture", StringComparison.Ordinal));
        result.Diagnostics.ShouldNotContain(diagnostic => diagnostic.Contains("requested materials/materialsbeta.cdb", StringComparison.Ordinal));
    }

    [Fact]
    public void TryRead_ReportsMaterialDatabaseProbeFailuresWhenDatabaseDoesNotResolve()
    {
        var reader = new NifPreviewModelReader();
        const string materialPath = @"Materials\Cinimatics\DigiPic\DigiPic_Base.mat";

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/Items/digipic/DigiPic.nif",
            DisplayName = "DigiPic",
            Data = CreateMaterialLinkedBSTriShapeNif(materialPath),
            ResolveExternalAsset = path => string.Equals(path, materialPath, StringComparison.OrdinalIgnoreCase)
                ? CreateStarfieldMaterialFile(@"Textures\Cinimatics\DigiPic\DigiPick_Material_normal.DDS")
                : null
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes.Count.ShouldBe(1);
        result.Model.Meshes[0].TexturePath.ShouldBe(@"Textures\Cinimatics\DigiPic\DigiPick_Material_normal.DDS");
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("from material", StringComparison.Ordinal));
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("requested materials/materialsbeta.cdb", StringComparison.Ordinal));
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("materials/materialsbeta.cdb was not resolved", StringComparison.Ordinal));
    }

    [Fact]
    public void TryRead_PreservesVerifiedStarfieldCinimaticsTexturePathWhenDatabaseDoesNotMatch()
    {
        var reader = new NifPreviewModelReader();
        const string materialPath = @"Materials\Cinimatics\DigiPic\DigiPic_Base.mat";
        const string materialDatabasePath = "materials/materialsbeta.cdb";

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/Items/digipic/DigiPic.nif",
            DisplayName = "DigiPic",
            Data = CreateMaterialLinkedBSTriShapeNif(materialPath),
            ResolveExternalAsset = path =>
            {
                if (string.Equals(path, materialPath, StringComparison.OrdinalIgnoreCase))
                {
                    return CreateStarfieldMaterialFile(@"Textures\Cinimatics\DigiPic\DigiPick_Material_color.DDS");
                }

                return string.Equals(path, materialDatabasePath, StringComparison.OrdinalIgnoreCase)
                    ? CreateStarfieldMaterialDatabase(@"Textures\Other\Unrelated_color.DDS")
                    : null;
            }
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes.Count.ShouldBe(1);
        result.Model.Meshes[0].TexturePath.ShouldBe(@"Textures\Cinimatics\DigiPic\DigiPick_Material_color.DDS");
        result.Diagnostics.ShouldNotContain(diagnostic => diagnostic.Contains("applied known Starfield texture folder correction", StringComparison.Ordinal));
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("skipped because material already provided preview color texture", StringComparison.Ordinal));
    }

    [Fact]
    public void TryRead_ReadsMaterialDatabaseStringsFromLaterEmbeddedStringTable()
    {
        var reader = new NifPreviewModelReader();
        const string materialPath = @"Materials\Cinimatics\DigiPic\DigiPic_Base.mat";
        const string materialDatabasePath = "materials/materialsbeta.cdb";

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/Items/digipic/DigiPic.nif",
            DisplayName = "DigiPic",
            Data = CreateMaterialLinkedBSTriShapeNif(materialPath),
            ResolveExternalAsset = path =>
            {
                if (string.Equals(path, materialPath, StringComparison.OrdinalIgnoreCase))
                {
                    return CreateStarfieldMaterialFile(@"Textures\Cinimatics\DigiPic\DigiPick_Material_normal.DDS");
                }

                return string.Equals(path, materialDatabasePath, StringComparison.OrdinalIgnoreCase)
                    ? CreateEmbeddedStarfieldMaterialDatabase(
                        ["Unrelated"],
                        [@"Textures\Verified\DigiPic\DigiPick_Material_normal.DDS"])
                    : null;
            }
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes.Count.ShouldBe(1);
        result.Model.Meshes[0].TexturePath.ShouldBe(@"Textures\Verified\DigiPic\DigiPick_Material_normal.DDS");
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("parsed 2 string(s) from 2 STRT table(s)", StringComparison.Ordinal));
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("from material database", StringComparison.Ordinal));
    }

    [Fact]
    public void TryRead_MarksStarfieldDecalMaterialAndUsesDirectOverlayTexture()
    {
        var reader = new NifPreviewModelReader();
        const string materialPath = @"Materials\SetDressing\Books\DecalLetterLineOverlay01_LinesStripes.mat";

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/SetDressing/Books/BookSmall01.nif",
            DisplayName = "BookSmall01",
            Data = CreateMaterialLinkedBSTriShapeNif(materialPath),
            ResolveExternalAsset = path => string.Equals(path, materialPath, StringComparison.OrdinalIgnoreCase)
                ? CreateStarfieldMaterialFileWithStrings(
                    "BSLayeredMaterial",
                    "1LayerStandardDecal",
                    "Additive",
                    @"Textures\SetDressing\Books\DecalLetterLineOverlay01_LinesStripes_color.dds")
                : null
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes[0].IsDecal.ShouldBeTrue();
        result.Model.Meshes[0].UseAdditiveBlend.ShouldBeTrue();
        result.Model.Meshes[0].TexturePath.ShouldBeNull();
        result.Model.Meshes[0].OverlayTexturePath.ShouldBe(@"Textures\SetDressing\Books\DecalLetterLineOverlay01_LinesStripes_color.dds");
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("decal material", StringComparison.Ordinal));
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("additive blend", StringComparison.Ordinal));
    }

    [Fact]
    public void TryRead_ParsesStarfieldDecalUvTransformFromMaterial()
    {
        var reader = new NifPreviewModelReader();
        const string materialPath = @"Materials\SetDressing\Books\DecalLetterLineOverlay01_LinesStripes.mat";

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/SetDressing/Books/BookSmall01.nif",
            DisplayName = "BookSmall01",
            Data = CreateMaterialLinkedBSTriShapeNif(materialPath),
            ResolveExternalAsset = path => string.Equals(path, materialPath, StringComparison.OrdinalIgnoreCase)
                ? CreateStarfieldDecalMaterialJsonFileWithUvTransform(
                    materialPath,
                    @"Data\Textures\Common\Decal\DecalLetterLineOverlay01_opacity.DDS",
                    -0.5f,
                    0.3f,
                    0f,
                    0.505f)
                : null
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes[0].DecalUvScaleU.ShouldBe(-0.5f);
        result.Model.Meshes[0].DecalUvScaleV.ShouldBe(0.3f);
        result.Model.Meshes[0].DecalUvOffsetU.ShouldBe(0f);
        result.Model.Meshes[0].DecalUvOffsetV.ShouldBe(0.505f);
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("decal UV scale (-0.5,0.3), offset (0,0.505)", StringComparison.Ordinal));
    }

    [Fact]
    public void TryRead_ResolvesStarfieldDecalOverlayTextureFromMaterialDatabaseByMaterialName()
    {
        var reader = new NifPreviewModelReader();
        const string materialPath = @"Materials\SetDressing\Books\DecalLetterLineOverlay01_LinesStripes.mat";
        const string materialDatabasePath = "materials/materialsbeta.cdb";

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/SetDressing/Books/BookSmall01.nif",
            DisplayName = "BookSmall01",
            Data = CreateMaterialLinkedBSTriShapeNif(materialPath),
            ResolveExternalAsset = path =>
            {
                if (string.Equals(path, materialPath, StringComparison.OrdinalIgnoreCase))
                {
                    return CreateStarfieldMaterialFileWithStrings(
                        "BSLayeredMaterial",
                        "1LayerStandardDecal",
                        "Additive",
                        @"Data\Materials\SetDressing\Books\DecalLetterLineOverlay01_LinesStripes");
                }

                return string.Equals(path, materialDatabasePath, StringComparison.OrdinalIgnoreCase)
                    ? CreateStarfieldMaterialDatabase(
                        @"Textures\SetDressing\Books\DecalLetterLineOverlay01_LinesStripes_color.dds",
                        @"Textures\SetDressing\Books\Unrelated_color.dds")
                    : null;
            }
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes[0].IsDecal.ShouldBeTrue();
        result.Model.Meshes[0].OverlayTexturePath.ShouldBe(@"Textures\SetDressing\Books\DecalLetterLineOverlay01_LinesStripes_color.dds");
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("material-name DDS candidate", StringComparison.Ordinal));
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("selected Textures\\SetDressing\\Books\\DecalLetterLineOverlay01_LinesStripes_color.dds", StringComparison.Ordinal));
    }

    [Fact]
    public void TryRead_DoesNotUseStarfieldParentOpacityTextureAsRenderableDecalOverlay()
    {
        var reader = new NifPreviewModelReader();
        const string materialPath = @"Materials\SetDressing\Books\DecalLetterLineOverlay01_LinesStripes.mat";
        const string baseMaterialPath = @"Materials\SetDressing\Books\DecalLetterLineOverlay01_Book_Base.mat";
        const string commonDecalMaterialPath = @"Materials\Common\Decal\DecalLetterLineOverlay01.mat";

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/SetDressing/Books/BookSmall01.nif",
            DisplayName = "BookSmall01",
            Data = CreateMaterialLinkedBSTriShapeNif(materialPath),
            ResolveExternalAsset = path =>
            {
                if (string.Equals(path, materialPath, StringComparison.OrdinalIgnoreCase))
                {
                    return CreateStarfieldMaterialJsonFile(
                        materialPath,
                        @"Data\MATERIALS\SetDressing\Books\DecalLetterLineOverlay01_Book_Base.mat",
                        "1LayerStandardDecal",
                        []);
                }

                if (string.Equals(path, baseMaterialPath, StringComparison.OrdinalIgnoreCase))
                {
                    return CreateStarfieldMaterialJsonFile(
                        baseMaterialPath,
                        @"Data\MATERIALS\Common\Decal\DecalLetterLineOverlay01.mat",
                        "1LayerStandardDecal",
                        []);
                }

                return string.Equals(path, commonDecalMaterialPath, StringComparison.OrdinalIgnoreCase)
                    ? CreateStarfieldDecalMaterialJsonFile(
                        commonDecalMaterialPath,
                        @"Data\Textures\Common\Decal\DecalLetterLineOverlay01_opacity.DDS")
                    : null;
            }
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes[0].IsDecal.ShouldBeTrue();
        result.Model.Meshes[0].OverlayTexturePath.ShouldBeNull();
        result.Model.Meshes[0].DecalOpacityTexturePath.ShouldBe(@"Textures\Common\Decal\DecalLetterLineOverlay01_opacity.DDS");
        result.Model.Meshes[0].DecalTintRed.ShouldBe(0.901961f, 0.0001f);
        result.Model.Meshes[0].DecalTintGreen.ShouldBe(0.894118f, 0.0001f);
        result.Model.Meshes[0].DecalTintBlue.ShouldBe(0.886275f, 0.0001f);
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("parent material", StringComparison.Ordinal) && diagnostic.Contains("DecalLetterLineOverlay01_Book_Base.mat resolved", StringComparison.Ordinal));
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("parent material", StringComparison.Ordinal) && diagnostic.Contains("DecalLetterLineOverlay01.mat resolved", StringComparison.Ordinal));
    }

    [Fact]
    public void TryRead_PrefersStarfieldRootMaterialColorTextureOverParentColorTexture()
    {
        var reader = new NifPreviewModelReader();
        const string materialPath = @"Materials\SetDressing\Books\GenBookA01_PagesAged.mat";
        const string parentMaterialPath = @"Materials\Common\Textile\TextileDenim01.mat";

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/SetDressing/Books/BookSmall01.nif",
            DisplayName = "BookSmall01",
            Data = CreateMaterialLinkedBSTriShapeNif(materialPath),
            ResolveExternalAsset = path =>
            {
                if (string.Equals(path, materialPath, StringComparison.OrdinalIgnoreCase))
                {
                    return CreateStarfieldMaterialJsonFile(
                        materialPath,
                        @"Data\MATERIALS\Common\Textile\TextileDenim01.mat",
                        "1LayerStandard",
                        [@"Data\Textures\SetDressing\Books\genbooka01_color.dds"]);
                }

                return string.Equals(path, parentMaterialPath, StringComparison.OrdinalIgnoreCase)
                    ? CreateStarfieldMaterialJsonFile(
                        parentMaterialPath,
                        null,
                        "1LayerStandard",
                        [@"Data\Textures\Common\Textile\TextileDenim01_color.DDS"])
                    : null;
            }
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes[0].TexturePath.ShouldBe(@"Textures\SetDressing\Books\genbooka01_color.dds");
    }

    [Fact]
    public void TryRead_ParsesStarfieldMaterialTintFromObjectMaterialColor()
    {
        var reader = new NifPreviewModelReader();
        const string materialPath = @"Materials\SetDressing\Books\GenBookA01_PagesAged.mat";

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/SetDressing/Books/BookSmall01.nif",
            DisplayName = "BookSmall01",
            Data = CreateMaterialLinkedBSTriShapeNif(materialPath),
            ResolveExternalAsset = path => string.Equals(path, materialPath, StringComparison.OrdinalIgnoreCase)
                ? CreateStarfieldMaterialJsonFileWithObjectMaterialColor(
                    materialPath,
                    @"Data\Textures\SetDressing\Books\genbooka01_color.dds",
                    0.595254f,
                    0.589136f,
                    0.575418f,
                    0f)
                : null
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes[0].MaterialTintRed.ShouldBe(0.595254f, 0.0001f);
        result.Model.Meshes[0].MaterialTintGreen.ShouldBe(0.589136f, 0.0001f);
        result.Model.Meshes[0].MaterialTintBlue.ShouldBe(0.575418f, 0.0001f);
        result.Model.Meshes[0].MaterialTintAlpha.ShouldBe(0f, 0.0001f);
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("material tint (0.595,0.589,0.575,0)", StringComparison.Ordinal));
    }

    [Fact]
    public void TryRead_MarksStarfieldInvisibleMaterial()
    {
        var reader = new NifPreviewModelReader();
        const string materialPath = @"Materials\Common\Utility\Decal_Blank_Off01.mat";

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/SetDressing/Books/BookSmall01.nif",
            DisplayName = "BookSmall01",
            Data = CreateMaterialLinkedBSTriShapeNif(materialPath),
            ResolveExternalAsset = path => string.Equals(path, materialPath, StringComparison.OrdinalIgnoreCase)
                ? CreateStarfieldMaterialFileWithStrings("BSLayeredMaterial", "Invisible")
                : null
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes[0].IsInvisible.ShouldBeTrue();
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("invisible material", StringComparison.Ordinal));
    }

    [Fact]
    public void TryRead_MarksStarfieldInvisibleMaterialFromShaderModelParentWhenSummaryIsNull()
    {
        var reader = new NifPreviewModelReader();
        const string materialPath = @"Materials\Common\Utility\Decal_Blank_Off01.mat";

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/SetDressing/Books/BookSmall01.nif",
            DisplayName = "BookSmall01",
            Data = CreateMaterialLinkedBSTriShapeNif(materialPath),
            ResolveExternalAsset = path => string.Equals(path, materialPath, StringComparison.OrdinalIgnoreCase)
                ? Encoding.UTF8.GetBytes("""
                    {
                        "Import" : [ "Data\\MATERIALS\\Layered\\ShaderModels\\Invisible.mat" ],
                        "Objects" : [
                            {
                                "Parent" : "Data\\MATERIALS\\Layered\\ShaderModels\\Invisible.mat"
                            }
                        ],
                        "Summary" : null,
                        "Version" : 1
                    }
                    """)
                : null
        });

        result.IsSuccess.ShouldBeTrue(result.StatusMessage);
        result.Model.ShouldNotBeNull();
        result.Model.Meshes[0].IsInvisible.ShouldBeTrue();
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("invisible material", StringComparison.Ordinal));
    }

    [Fact]
    public void TryRead_ReturnsFailureForUnsupportedVersion()
    {
        var data = CreateMinimalBSTriShapeNif(version: 0x14000005);
        var reader = new NifPreviewModelReader();

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/Preview.nif",
            DisplayName = "Preview",
            Data = data
        });

        result.IsSuccess.ShouldBeFalse();
        result.StatusMessage.ShouldContain("not supported");
    }

    [Fact]
    public void TryRead_ReturnsFailureWhenNoSupportedGeometryExists()
    {
        var data = CreateMinimalNifWithBlock("NiNode", []);
        var reader = new NifPreviewModelReader();

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/Preview.nif",
            DisplayName = "Preview",
            Data = data
        });

        result.IsSuccess.ShouldBeFalse();
        result.StatusMessage.ShouldContain("No supported preview geometry");
        result.StatusMessage.ShouldContain("Block types: NiNode x1");
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Contains("Header user 12", StringComparison.Ordinal));
    }

    [Fact]
    public void TryRead_ReturnsSkinnedGeometryStatusWhenSkyrimMeshUsesSkinBlocks()
    {
        var data = CreateMinimalNifWithBlocks(
            [
                ("NiNode", []),
                ("NiSkinInstance", []),
                ("NiSkinPartition", []),
                ("BSTriShape", new byte[128])
            ],
            bethesdaVersion: 100U);
        var reader = new NifPreviewModelReader();

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/Clutter/Preview.nif",
            DisplayName = "Preview",
            Data = data
        });

        result.IsSuccess.ShouldBeFalse();
        result.StatusMessage.ShouldContain("Skinned or partitioned NIF geometry is not supported");
    }

    [Fact]
    public void TryRead_ReturnsFailureWhenHeaderTablesCannotBeLocated()
    {
        var data = CreateMinimalNifWithBlock("BSTriShape", [0xFF, 0xFF, 0xFF, 0xFF], extraBytesBeforeTables: [0xFF, 0xFF, 0xFF, 0xFF], writeTables: false);
        var reader = new NifPreviewModelReader();

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/Preview.nif",
            DisplayName = "Preview",
            Data = data
        });

        result.IsSuccess.ShouldBeFalse();
        result.StatusMessage.ShouldContain("header tables could not be located");
    }

    [Fact]
    public void TryRead_ReturnsFailureWhenVertexPositionIsOutsidePreviewRange()
    {
        var data = CreateMinimalBSTriShapeNif(firstVertexX: 2000000f);
        var reader = new NifPreviewModelReader();

        var result = reader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = "Meshes/Preview.nif",
            DisplayName = "Preview",
            Data = data
        });

        result.IsSuccess.ShouldBeFalse();
        result.StatusMessage.ShouldContain("outside the supported preview range");
    }

    private static byte[] CreateMinimalBSTriShapeNif(
        uint version = 0x14020007,
        bool includeProcessScript = true,
        byte[]? extraBytesBeforeTables = null,
        float firstVertexX = -1f,
        uint bethesdaVersion = 130U)
    {
        using var blockStream = new MemoryStream();
        using (var writer = new BinaryWriter(blockStream, Encoding.UTF8, leaveOpen: true))
        {
            writer.Write(((ulong)0x401 << 44) | 4UL);
            writer.Write(1U);
            writer.Write((ushort)3);
            writer.Write(54U);
            WriteVertex(writer, firstVertexX, -1f, 0f);
            WriteVertex(writer, 1f, -1f, 0f);
            WriteVertex(writer, 0f, 1f, 0f);
            writer.Write((ushort)0);
            writer.Write((ushort)1);
            writer.Write((ushort)2);
        }

        return CreateMinimalNifWithBlock("BSTriShape", blockStream.ToArray(), version, includeProcessScript, extraBytesBeforeTables, bethesdaVersion: bethesdaVersion);
    }

    private static byte[] CreateSkyrimSpecialEditionLayoutBSTriShapeNif()
    {
        return CreateMinimalNifWithBlock(
            "BSTriShape",
            CreateSkyrimSpecialEditionBSTriShapeBlock(),
            bethesdaVersion: 100U);
    }

    private static byte[] CreateSkyrimSpecialEditionStrideSevenBSTriShapeNif()
    {
        return CreateMinimalNifWithBlock(
            "BSTriShape",
            CreateSkyrimSpecialEditionStrideSevenBSTriShapeBlock(),
            bethesdaVersion: 100U);
    }

    private static byte[] CreateMinimalHalfPositionBSTriShapeNif()
    {
        using var blockStream = new MemoryStream();
        using (var writer = new BinaryWriter(blockStream, Encoding.UTF8, leaveOpen: true))
        {
            writer.Write(((ulong)0x1 << 44) | 4UL);
            writer.Write(1U);
            writer.Write((ushort)3);
            writer.Write(54U);
            WriteHalfVertex(writer, -1f, -1f, 0f);
            WriteHalfVertex(writer, 1f, -1f, 0f);
            WriteHalfVertex(writer, 0f, 1f, 0f);
            writer.Write((ushort)0);
            writer.Write((ushort)1);
            writer.Write((ushort)2);
        }

        return CreateMinimalNifWithBlock("BSTriShape", blockStream.ToArray());
    }

    private static byte[] CreateOffsetVertexAttributeBSTriShapeNif()
    {
        using var blockStream = new MemoryStream();
        using (var writer = new BinaryWriter(blockStream, Encoding.UTF8, leaveOpen: true))
        {
            writer.Write(((ulong)0x40B << 44) | (4UL << 16) | (3UL << 8) | 6UL);
            writer.Write(1U);
            writer.Write((ushort)3);
            writer.Write(78U);
            WriteVertexWithOffsetAttributes(writer, -1f, -1f, 0f, 0.25f, 0.75f);
            WriteVertexWithOffsetAttributes(writer, 1f, -1f, 0f, 0.5f, 0.5f);
            WriteVertexWithOffsetAttributes(writer, 0f, 1f, 0f, 0.75f, 0.25f);
            writer.Write((ushort)0);
            writer.Write((ushort)1);
            writer.Write((ushort)2);
        }

        return CreateMinimalNifWithBlock("BSTriShape", blockStream.ToArray());
    }

    private static byte[] CreateMinimalTransformedBSTriShapeNif()
    {
        using var blockStream = new MemoryStream();
        using (var writer = new BinaryWriter(blockStream, Encoding.UTF8, leaveOpen: true))
        {
            WriteTransformPrefix(writer, 10f, 5f, 0f, 2f);
            writer.Write(((ulong)0x401 << 44) | 4UL);
            writer.Write(1U);
            writer.Write((ushort)3);
            writer.Write(54U);
            WriteVertex(writer, -1f, -1f, 0f);
            WriteVertex(writer, 1f, -1f, 0f);
            WriteVertex(writer, 0f, 1f, 0f);
            writer.Write((ushort)0);
            writer.Write((ushort)1);
            writer.Write((ushort)2);
        }

        return CreateMinimalNifWithBlock("BSTriShape", blockStream.ToArray());
    }

    private static byte[] CreateParentedTransformedBSTriShapeNif()
    {
        var blocks = new List<(string BlockType, byte[] BlockData)>();
        blocks.Add(("NiNode", CreateNodeBlock([2, 8])));
        blocks.Add(("BSXFlags", []));
        blocks.Add(("NiNode", CreateNodeBlock([5])));
        blocks.Add(("bhkNPCollisionObject", []));
        blocks.Add(("bhkPhysicsSystem", []));
        blocks.Add(("BSTriShape", CreateBSTriShapeBlock(0f, 0f, 2f, 0f)));
        blocks.Add(("BSLightingShaderProperty", []));
        blocks.Add(("BSShaderTextureSet", []));
        blocks.Add(("BSTriShape", CreateBSTriShapeBlock(0f, 0f, 0f, -1f)));

        return CreateMinimalNifWithBlocks(blocks);
    }

    private static byte[] CreateMaterialLinkedBSTriShapeNif()
    {
        var blocks = new List<(string BlockType, byte[] BlockData)>();
        blocks.Add(("BSTriShape", CreateBSTriShapeBlock(0f, 0f, 0f, 0f, shaderProperty: 1, nameIndex: 0)));
        blocks.Add(("BSLightingShaderProperty", CreateLightingShaderPropertyBlock(1, 2)));
        blocks.Add(("BSShaderTextureSet", CreateTextureSetBlock(@"textures\SetDressing\BabyBottle\BabyBottleDirty01_d.dds")));

        return CreateMinimalNifWithBlocks(
            blocks,
            [
                "BottleTop:20",
                @"Materials\SetDressing\BabyBottle\BabyBottleDirty01.BGSM"
            ]);
    }

    private static byte[] CreateMaterialLinkedBSTriShapeNif(string materialPath)
    {
        var blocks = new List<(string BlockType, byte[] BlockData)>();
        blocks.Add(("BSTriShape", CreateBSTriShapeBlock(0f, 0f, 0f, 0f, shaderProperty: 1, nameIndex: 0)));
        blocks.Add(("BSLightingShaderProperty", CreateLightingShaderPropertyBlock(1, -1)));

        return CreateMinimalNifWithBlocks(
            blocks,
            [
                "DigiPick_Final:0",
                materialPath
            ],
            bethesdaVersion: 173U);
    }

    private static byte[] CreateStarfieldMaterialFile(params string[] texturePaths)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: false);
        WriteSizedString(writer, "BSLayeredMaterial");
        WriteSizedString(writer, "1LayerEffectGlass");
        WriteSizedString(writer, "UseLayeredEdgeFalloff");
        foreach (var texturePath in texturePaths)
        {
            WriteSizedString(writer, texturePath);
        }

        return stream.ToArray();
    }

    private static byte[] CreateStarfieldMaterialFileWithStrings(params string[] values)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: false);
        foreach (var value in values)
        {
            WriteSizedString(writer, value);
        }

        return stream.ToArray();
    }

    private static byte[] CreateStarfieldMaterialJsonFile(string fileName, string? parent, string shaderModel, string[] textureFiles)
    {
        var textureJson = textureFiles.Length == 0
            ? "null"
            : "{" + string.Join(",", textureFiles.Select((textureFile, index) => $"\"Texture{index}\":{{\"File\":\"{EscapeJson(textureFile)}\"}}")) + "}";
        var parentJson = parent == null
            ? "\"\""
            : $"\"{EscapeJson(parent)}\"";
        var json = $$"""
            {
                "Filename" : "{{EscapeJson(fileName)}}",
                "Import" : [ "Data\\MATERIALS\\Layered\\ShaderModels\\{{shaderModel}}.mat" ],
                "Objects" : [
                    {
                        "Parent" : {{parentJson}}
                    }
                ],
                "Summary" : {
                    "Layer1" : {
                        "Parent" : {{parentJson}},
                        "ShaderModel" : "{{shaderModel}}",
                        "Textures" : {{textureJson}}
                    }
                },
                "Version" : 1
            }
            """;
        return Encoding.UTF8.GetBytes(json);
    }

    private static byte[] CreateStarfieldDecalMaterialJsonFile(string fileName, string opacityTexturePath)
    {
        var json = $$"""
            {
                "Filename" : "{{EscapeJson(fileName)}}",
                "Import" : [ "Data\\MATERIALS\\Layered\\ShaderModels\\1LayerStandardDecal.mat" ],
                "Objects" : [],
                "Summary" : {
                    "Layer1" : {
                        "ShaderModel" : "1LayerStandardDecal",
                        "Textures" : {
                            "Albedo" : {
                                "File" : "",
                                "Replacement" : {
                                    "w" : 1,
                                    "x" : 0.901961,
                                    "y" : 0.894118,
                                    "z" : 0.886275
                                },
                                "UseReplacement" : true
                            },
                            "Opacity" : {
                                "File" : "{{EscapeJson(opacityTexturePath)}}",
                                "Replacement" : {
                                    "w" : 1,
                                    "x" : 1,
                                    "y" : 1,
                                    "z" : 1
                                },
                                "UseReplacement" : false
                            }
                        }
                    }
                },
                "Version" : 1
            }
            """;
        return Encoding.UTF8.GetBytes(json);
    }

    private static byte[] CreateStarfieldMaterialJsonFileWithAlbedoTint(string fileName, string colorTexturePath, float red, float green, float blue, float alpha)
    {
        var redText = red.ToString("0.######", CultureInfo.InvariantCulture);
        var greenText = green.ToString("0.######", CultureInfo.InvariantCulture);
        var blueText = blue.ToString("0.######", CultureInfo.InvariantCulture);
        var alphaText = alpha.ToString("0.######", CultureInfo.InvariantCulture);
        var json = $$"""
            {
                "Filename" : "{{EscapeJson(fileName)}}",
                "Import" : [ "Data\\MATERIALS\\Layered\\ShaderModels\\1LayerStandard.mat" ],
                "Objects" : [],
                "Summary" : {
                    "Layer1" : {
                        "ShaderModel" : "1LayerStandard",
                        "Textures" : {
                            "Albedo" : {
                                "File" : "{{EscapeJson(colorTexturePath)}}",
                                "Replacement" : {
                                    "w" : {{alphaText}},
                                    "x" : {{redText}},
                                    "y" : {{greenText}},
                                    "z" : {{blueText}}
                                },
                                "UseReplacement" : true
                            }
                        }
                    }
                },
                "Version" : 1
            }
            """;
        return Encoding.UTF8.GetBytes(json);
    }

    private static byte[] CreateStarfieldMaterialJsonFileWithObjectMaterialColor(string fileName, string colorTexturePath, float red, float green, float blue, float alpha)
    {
        var redText = red.ToString("0.######", CultureInfo.InvariantCulture);
        var greenText = green.ToString("0.######", CultureInfo.InvariantCulture);
        var blueText = blue.ToString("0.######", CultureInfo.InvariantCulture);
        var alphaText = alpha.ToString("0.######", CultureInfo.InvariantCulture);
        var json = $$"""
            {
                "Filename" : "{{EscapeJson(fileName)}}",
                "Import" : [ "Data\\MATERIALS\\Layered\\ShaderModels\\1LayerStandard.mat" ],
                "Objects" : [
                    {
                        "Components" : [
                            {
                                "Data" : {
                                    "Name" : "GenBookA01_PagesAged_Material1"
                                },
                                "Index" : 0,
                                "Type" : "BSComponentDB::CTName"
                            },
                            {
                                "Data" : {
                                    "ID" : "res:7FC24A27:0005EB54:A577170D"
                                },
                                "Index" : 0,
                                "Type" : "BSMaterial::TextureSetID"
                            },
                            {
                                "Data" : {
                                    "Value" : {
                                        "Data" : {
                                            "w" : "{{alphaText}}",
                                            "x" : "{{redText}}",
                                            "y" : "{{greenText}}",
                                            "z" : "{{blueText}}"
                                        },
                                        "Type" : "XMFLOAT4"
                                    }
                                },
                                "Index" : 0,
                                "Type" : "BSMaterial::Color",
                                "Version" : 1
                            }
                        ],
                        "ID" : "res:7FC24A25:0005EB54:A577170D",
                        "Parent" : "res:06763197:0005DAE6:A64340C8"
                    },
                    {
                        "Components" : [
                            {
                                "Data" : {
                                    "Name" : "GenBookA01_PagesAged_TextureSet1"
                                },
                                "Index" : 0,
                                "Type" : "BSComponentDB::CTName"
                            },
                            {
                                "Data" : {
                                    "FileName" : "{{EscapeJson(colorTexturePath)}}"
                                },
                                "Index" : 0,
                                "Type" : "BSMaterial::MRTextureFile",
                                "Version" : 2
                            }
                        ],
                        "ID" : "res:7FC24A27:0005EB54:A577170D",
                        "Parent" : "res:067631C2:0005DAE6:A64340C8"
                    }
                ],
                "Summary" : {
                    "Layer1" : {
                        "ShaderModel" : "1LayerStandard"
                    }
                },
                "Version" : 1
            }
            """;
        return Encoding.UTF8.GetBytes(json);
    }

    private static byte[] CreateStarfieldDecalMaterialJsonFileWithUvTransform(string fileName, string opacityTexturePath, float scaleU, float scaleV, float offsetU, float offsetV)
    {
        var scaleUText = scaleU.ToString("0.###", CultureInfo.InvariantCulture);
        var scaleVText = scaleV.ToString("0.###", CultureInfo.InvariantCulture);
        var offsetUText = offsetU.ToString("0.###", CultureInfo.InvariantCulture);
        var offsetVText = offsetV.ToString("0.###", CultureInfo.InvariantCulture);
        var json = $$"""
            {
                "Filename" : "{{EscapeJson(fileName)}}",
                "Import" : [ "Data\\MATERIALS\\Layered\\ShaderModels\\1LayerStandardDecal.mat" ],
                "Objects" : [],
                "Summary" : {
                    "Layer1" : {
                        "ShaderModel" : "1LayerStandardDecal",
                        "UV Stream" : {
                            "Scale" : {
                                "x" : {{scaleUText}},
                                "y" : {{scaleVText}}
                            },
                            "Offset" : {
                                "x" : {{offsetUText}},
                                "y" : {{offsetVText}}
                            }
                        },
                        "Textures" : {
                            "Opacity" : {
                                "File" : "{{EscapeJson(opacityTexturePath)}}",
                                "UseReplacement" : false
                            }
                        }
                    }
                },
                "Version" : 1
            }
            """;
        return Encoding.UTF8.GetBytes(json);
    }

    private static string EscapeJson(string value)
    {
        return value.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("\"", "\\\"", StringComparison.Ordinal);
    }

    private static byte[] CreateStarfieldMaterialDatabase(params string[] strings)
    {
        var stringTable = Encoding.UTF8.GetBytes(string.Join('\0', strings) + '\0');
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: false);
        writer.Write(0x0000000848544542UL);
        writer.Write(4U);
        writer.Write(0U);
        writer.Write(0x54525453U);
        writer.Write((uint)stringTable.Length);
        writer.Write(stringTable);
        return stream.ToArray();
    }

    private static byte[] CreateEmbeddedStarfieldMaterialDatabase(params string[][] stringTables)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: false);
        foreach (var strings in stringTables)
        {
            writer.Write([0x13, 0x37, 0x42]);
            writer.Write(CreateStarfieldMaterialDatabase(strings));
        }

        return stream.ToArray();
    }

    private static byte[] CreateNodeBlock(IReadOnlyList<int> children)
    {
        using var stream = new MemoryStream();
        using (var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
        {
            WriteTransformPrefix(writer, 0f, 0f, 0f, 1f);
            writer.Write((uint)children.Count);
            foreach (var child in children)
            {
                writer.Write(child);
            }
        }

        return stream.ToArray();
    }

    private static byte[] CreateBSTriShapeBlock(
        float translationX,
        float translationY,
        float translationZ,
        float firstVertexZ,
        int shaderProperty = -1,
        int alphaProperty = -1,
        uint nameIndex = 0)
    {
        using var stream = new MemoryStream();
        using (var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
        {
            WriteTransformPrefix(writer, translationX, translationY, translationZ, 1f, nameIndex);
            writer.Write(0f);
            writer.Write(0f);
            writer.Write(0f);
            writer.Write(1f);
            writer.Write(-1);
            writer.Write(shaderProperty);
            writer.Write(alphaProperty);
            writer.Write(((ulong)0x401 << 44) | 4UL);
            writer.Write(1U);
            writer.Write((ushort)3);
            writer.Write(54U);
            WriteVertex(writer, 0f, 0f, firstVertexZ);
            WriteVertex(writer, 1f, 0f, firstVertexZ);
            WriteVertex(writer, 0f, 1f, firstVertexZ);
            writer.Write((ushort)0);
            writer.Write((ushort)1);
            writer.Write((ushort)2);
        }

        return stream.ToArray();
    }

    private static byte[] CreateStarfieldBSGeometryBlockWithoutExplicitDataSize()
    {
        using var stream = new MemoryStream();
        using (var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
        {
            WriteTransformPrefix(writer, 0f, 0f, 0f, 1f);
            writer.Write(0f);
            writer.Write(0f);
            writer.Write(0f);
            writer.Write(1f);
            writer.Write(-1);
            writer.Write(-1);
            writer.Write(-1);
            writer.Write(((ulong)0x401 << 44) | 4UL);
            writer.Write(1U);
            writer.Write((ushort)3);
            WriteVertex(writer, 0f, 0f, 0f);
            WriteVertex(writer, 1f, 0f, 0f);
            WriteVertex(writer, 0f, 1f, 0f);
            writer.Write((ushort)0);
            writer.Write((ushort)1);
            writer.Write((ushort)2);
        }

        return stream.ToArray();
    }

    private static byte[] CreateInvalidStarfieldBSGeometryBlockWithoutExplicitDataSize()
    {
        using var stream = new MemoryStream();
        using (var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
        {
            WriteTransformPrefix(writer, 0f, 0f, 0f, 1f);
            writer.Write(0f);
            writer.Write(0f);
            writer.Write(0f);
            writer.Write(1f);
            writer.Write(-1);
            writer.Write(-1);
            writer.Write(-1);
            writer.Write(((ulong)0x401 << 44) | 4UL);
            writer.Write(1U);
            writer.Write((ushort)3);
        }

        return stream.ToArray();
    }

    private static byte[] CreateStarfieldBSGeometryBlockWithPaddedCountsAfterDescriptor()
    {
        using var stream = new MemoryStream();
        using (var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
        {
            WriteTransformPrefix(writer, 0f, 0f, 0f, 1f);
            writer.Write(0f);
            writer.Write(0f);
            writer.Write(0f);
            writer.Write(1f);
            writer.Write(-1);
            writer.Write(-1);
            writer.Write(-1);
            writer.Write(((ulong)0x401 << 44) | 5UL);
            writer.Write(0U);
            writer.Write(1U);
            writer.Write((ushort)3);
            WriteStarfieldStrideFiveVertex(writer, 0f, 0f, 0f);
            WriteStarfieldStrideFiveVertex(writer, 1f, 0f, 0f);
            WriteStarfieldStrideFiveVertex(writer, 0f, 1f, 0f);
            writer.Write((ushort)0);
            writer.Write((ushort)1);
            writer.Write((ushort)2);
        }

        return stream.ToArray();
    }

    private static byte[] CreateStarfieldBSGeometryBlockWithFalsePreDescriptorBytes()
    {
        using var stream = new MemoryStream();
        using (var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
        {
            WriteTransformPrefix(writer, 0f, 0f, 0f, 1f);
            writer.Write(((ulong)0x401 << 44) | 4UL);
            writer.Write(0f);
            writer.Write(1f);
            writer.Write(-1);
            writer.Write(-1);
            writer.Write(-1);
            writer.Write(((ulong)0x401 << 44) | 4UL);
            writer.Write(1U);
            writer.Write((ushort)3);
            WriteVertex(writer, 0f, 0f, 0f);
            WriteVertex(writer, 1f, 0f, 0f);
            WriteVertex(writer, 0f, 1f, 0f);
            writer.Write((ushort)0);
            writer.Write((ushort)1);
            writer.Write((ushort)2);
        }

        return stream.ToArray();
    }

    private static byte[] CreateStarfieldBSGeometryBlockWithExternalGeometryReference(
        int shaderProperty = -1,
        float translationX = 0f,
        float translationY = 0f,
        float translationZ = 0f)
    {
        using var stream = new MemoryStream();
        using (var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
        {
            WriteTransformPrefix(writer, translationX, translationY, translationZ, 1f, nameIndex: 0);
            writer.Write(0f);
            writer.Write(0f);
            writer.Write(0f);
            writer.Write(1f);
            writer.Write(0.25f);
            writer.Write(0.5f);
            writer.Write(0.75f);
            writer.Write(0.125f);
            writer.Write(0.25f);
            writer.Write(0.5f);
            writer.Write(-1);
            writer.Write(shaderProperty);
            writer.Write(-1);
            WriteSizedString(writer, @"cf623091ecaffe5a43fa\249816728d4437f890e8");
        }

        return stream.ToArray();
    }

    private static byte[] CreateStarfieldGeometryMesh()
    {
        using var stream = new MemoryStream();
        using (var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
        {
            writer.Write(2U);
            writer.Write(3U);
            writer.Write((ushort)0);
            writer.Write((ushort)1);
            writer.Write((ushort)2);
            writer.Write(2f);
            writer.Write(0U);
            writer.Write(3U);
            WriteStarfieldGeometryVertex(writer, 0, 0, 0);
            WriteStarfieldGeometryVertex(writer, 16384, 0, 0);
            WriteStarfieldGeometryVertex(writer, 0, 16384, 0);
        }

        return stream.ToArray();
    }

    private static byte[] CreateStarfieldGeometryMeshWithUvStream()
    {
        using var stream = new MemoryStream();
        using (var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
        {
            writer.Write(2U);
            writer.Write(3U);
            writer.Write((ushort)0);
            writer.Write((ushort)1);
            writer.Write((ushort)2);
            writer.Write(1f);
            writer.Write(0U);
            writer.Write(3U);
            WriteStarfieldGeometryVertex(writer, 0, 0, 0);
            WriteStarfieldGeometryVertex(writer, 16384, 0, 0);
            WriteStarfieldGeometryVertex(writer, 0, 16384, 0);
            writer.Write(3U);
            WriteHalfUv(writer, 0.25f, 0.75f);
            WriteHalfUv(writer, 1f, 0f);
            WriteHalfUv(writer, 0f, 1f);
            writer.Write(0U);
            writer.Write(3U);
            WritePackedAlpha(writer, 0);
            WritePackedAlpha(writer, 128);
            WritePackedAlpha(writer, 255);
        }

        return stream.ToArray();
    }

    private static byte[] CreateSkyrimSpecialEditionBSTriShapeBlock()
    {
        using var stream = new MemoryStream();
        using (var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
        {
            WriteTransformPrefix(writer, 0f, 0f, 0f, 1f);
            writer.Write(0f);
            writer.Write(0f);
            writer.Write(0f);
            writer.Write(1f);
            writer.Write(-1);
            writer.Write(-1);
            writer.Write(-1);
            writer.Write(((ulong)0x00B << 44) | (4UL << 16) | (3UL << 8) | 8UL);
            writer.Write((ushort)1);
            writer.Write((ushort)3);
            writer.Write(102U);
            WriteSkyrimSpecialEditionVertex(writer, -1f, -1f, 0f);
            WriteSkyrimSpecialEditionVertex(writer, 1f, -1f, 0f);
            WriteSkyrimSpecialEditionVertex(writer, 0f, 1f, 0f);
            writer.Write((ushort)0);
            writer.Write((ushort)1);
            writer.Write((ushort)2);
        }

        return stream.ToArray();
    }

    private static byte[] CreateSkyrimSpecialEditionStrideSevenBSTriShapeBlock()
    {
        using var stream = new MemoryStream();
        using (var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
        {
            WriteTransformPrefix(writer, 0f, 0f, 0f, 1f);
            writer.Write(0f);
            writer.Write(0f);
            writer.Write(0f);
            writer.Write(1f);
            writer.Write(-1);
            writer.Write(-1);
            writer.Write(-1);
            writer.Write(((ulong)0x00B << 44) | (4UL << 16) | (3UL << 8) | 7UL);
            writer.Write((ushort)1);
            writer.Write((ushort)3);
            writer.Write(90U);
            WriteSkyrimSpecialEditionStrideSevenVertex(writer, -1f, -1f, 0f);
            WriteSkyrimSpecialEditionStrideSevenVertex(writer, 1f, -1f, 0f);
            WriteSkyrimSpecialEditionStrideSevenVertex(writer, 0f, 1f, 0f);
            writer.Write((ushort)0);
            writer.Write((ushort)1);
            writer.Write((ushort)2);
        }

        return stream.ToArray();
    }

    private static byte[] CreateLightingShaderPropertyBlock(uint nameIndex, int textureSetReference)
    {
        using var stream = new MemoryStream();
        using (var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
        {
            writer.Write(nameIndex);
            writer.Write(0U);
            writer.Write(-1);
            writer.Write(textureSetReference);
        }

        return stream.ToArray();
    }

    private static byte[] CreateTextureSetBlock(string texturePath)
    {
        using var stream = new MemoryStream();
        using (var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
        {
            writer.Write(1U);
            WriteSizedString(writer, texturePath);
        }

        return stream.ToArray();
    }

    private static byte[] CreateMinimalNifWithBlock(
        string blockType,
        byte[] blockData,
        uint version = 0x14020007,
        bool includeProcessScript = true,
        byte[]? extraBytesBeforeTables = null,
        bool writeTables = true,
        uint bethesdaVersion = 130U)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: false);
        writer.Write(Encoding.ASCII.GetBytes("Gamebryo File Format, Version 20.2.0.7\n"));
        writer.Write(version);
        writer.Write((byte)1);
        writer.Write(12U);
        writer.Write(1U);
        writer.Write(bethesdaVersion);
        WriteSizedString(writer, "Creations Forge");
        if (includeProcessScript)
        {
            WriteSizedString(writer, "Process");
        }

        WriteSizedString(writer, "Export");
        WriteSizedString(writer, string.Empty);
        if (extraBytesBeforeTables != null)
        {
            writer.Write(extraBytesBeforeTables);
        }

        if (!writeTables)
        {
            writer.Write(blockData);
            return stream.ToArray();
        }

        writer.Write((ushort)1);
        WriteSizedString(writer, blockType);
        writer.Write((ushort)0);
        writer.Write((uint)blockData.Length);
        writer.Write(0U);
        writer.Write(0U);
        writer.Write(0U);
        writer.Write(blockData);
        return stream.ToArray();
    }

    private static byte[] CreateMinimalNifWithBlocks(
        IReadOnlyList<(string BlockType, byte[] BlockData)> blocks,
        IReadOnlyList<string>? strings = null,
        uint bethesdaVersion = 130U)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: false);
        writer.Write(Encoding.ASCII.GetBytes("Gamebryo File Format, Version 20.2.0.7\n"));
        writer.Write(0x14020007U);
        writer.Write((byte)1);
        writer.Write(12U);
        writer.Write((uint)blocks.Count);
        writer.Write(bethesdaVersion);
        WriteSizedString(writer, "Creations Forge");
        WriteSizedString(writer, "Process");
        WriteSizedString(writer, "Export");
        WriteSizedString(writer, string.Empty);
        writer.Write((ushort)blocks.Count);
        foreach (var block in blocks)
        {
            WriteSizedString(writer, block.BlockType);
        }

        for (var index = 0; index < blocks.Count; index++)
        {
            writer.Write((ushort)index);
        }

        foreach (var block in blocks)
        {
            writer.Write((uint)block.BlockData.Length);
        }

        writer.Write((uint)(strings?.Count ?? 0));
        writer.Write((uint)(strings == null || strings.Count == 0 ? 0 : strings.Max(value => Encoding.UTF8.GetByteCount(value))));
        if (strings != null)
        {
            foreach (var value in strings)
            {
                WriteSizedString(writer, value);
            }
        }

        writer.Write(0U);
        foreach (var block in blocks)
        {
            writer.Write(block.BlockData);
        }

        return stream.ToArray();
    }

    private static void WriteVertex(BinaryWriter writer, float x, float y, float z)
    {
        writer.Write(x);
        writer.Write(y);
        writer.Write(z);
        writer.Write(0U);
    }

    private static void WriteStarfieldStrideFiveVertex(BinaryWriter writer, float x, float y, float z)
    {
        writer.Write(x);
        writer.Write(y);
        writer.Write(z);
        writer.Write(0U);
        writer.Write(0U);
    }

    private static void WriteStarfieldGeometryVertex(BinaryWriter writer, short x, short y, short z)
    {
        writer.Write(x);
        writer.Write(y);
        writer.Write(z);
    }

    private static void WriteHalfUv(BinaryWriter writer, float u, float v)
    {
        writer.Write(BitConverter.HalfToUInt16Bits((Half)u));
        writer.Write(BitConverter.HalfToUInt16Bits((Half)v));
    }

    private static void WritePackedAlpha(BinaryWriter writer, byte alpha)
    {
        writer.Write((byte)255);
        writer.Write((byte)255);
        writer.Write((byte)255);
        writer.Write(alpha);
    }

    private static void WriteHalfVertex(BinaryWriter writer, float x, float y, float z)
    {
        writer.Write(BitConverter.HalfToUInt16Bits((Half)x));
        writer.Write(BitConverter.HalfToUInt16Bits((Half)y));
        writer.Write(BitConverter.HalfToUInt16Bits((Half)z));
        writer.Write((ushort)0);
        writer.Write(0U);
        writer.Write(0U);
    }

    private static void WriteVertexWithOffsetAttributes(BinaryWriter writer, float x, float y, float z, float u, float v)
    {
        writer.Write(x);
        writer.Write(y);
        writer.Write(z);
        writer.Write(BitConverter.HalfToUInt16Bits((Half)u));
        writer.Write(BitConverter.HalfToUInt16Bits((Half)v));
        writer.Write((byte)128);
        writer.Write((byte)128);
        writer.Write((byte)255);
        writer.Write((byte)0);
        writer.Write(0U);
    }

    private static void WriteSkyrimSpecialEditionVertex(BinaryWriter writer, float x, float y, float z)
    {
        writer.Write(x);
        writer.Write(y);
        writer.Write(z);
        writer.Write(BitConverter.HalfToUInt16Bits((Half)0f));
        writer.Write(BitConverter.HalfToUInt16Bits((Half)0f));
        writer.Write((byte)128);
        writer.Write((byte)128);
        writer.Write((byte)255);
        writer.Write((byte)0);
        writer.Write(0U);
        writer.Write(0U);
        writer.Write(0U);
    }

    private static void WriteSkyrimSpecialEditionStrideSevenVertex(BinaryWriter writer, float x, float y, float z)
    {
        writer.Write(x);
        writer.Write(y);
        writer.Write(z);
        writer.Write(BitConverter.HalfToUInt16Bits((Half)0f));
        writer.Write(BitConverter.HalfToUInt16Bits((Half)0f));
        writer.Write((byte)128);
        writer.Write((byte)128);
        writer.Write((byte)255);
        writer.Write((byte)0);
        writer.Write(0U);
        writer.Write(0U);
    }

    private static void WriteTransformPrefix(BinaryWriter writer, float x, float y, float z, float scale, uint nameIndex = 0)
    {
        writer.Write(nameIndex);
        writer.Write(0U);
        writer.Write(-1);
        writer.Write(0U);
        writer.Write(x);
        writer.Write(y);
        writer.Write(z);
        writer.Write(1f);
        writer.Write(0f);
        writer.Write(0f);
        writer.Write(0f);
        writer.Write(1f);
        writer.Write(0f);
        writer.Write(0f);
        writer.Write(0f);
        writer.Write(1f);
        writer.Write(scale);
        writer.Write(-1);
    }

    private static void WriteSizedString(BinaryWriter writer, string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        writer.Write((uint)bytes.Length);
        writer.Write(bytes);
    }
}
