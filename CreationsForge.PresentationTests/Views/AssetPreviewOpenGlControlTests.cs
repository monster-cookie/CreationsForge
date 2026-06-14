using System.Numerics;
using System.Reflection;
using CreationsForge.Views;
using Shouldly;

namespace CreationsForge.PresentationTests.Views;

public class AssetPreviewOpenGlControlTests
{
    [Theory]
    [InlineData(1f, 0f, 0f)]
    [InlineData(-1f, 0f, 0f)]
    [InlineData(0f, 1f, 0f)]
    [InlineData(0f, -1f, 0f)]
    [InlineData(0f, 0f, 1f)]
    [InlineData(0f, 0f, -1f)]
    public void CreateCameraOrientation_MapsForwardAxisToRequestedDirection(float x, float y, float z)
    {
        var expected = Vector3.Normalize(new Vector3(x, y, z));
        var orientation = CreateCameraOrientation(expected);

        var actual = Vector3.Normalize(Vector3.Transform(Vector3.UnitZ, orientation));

        actual.X.ShouldBe(expected.X, 0.0001f);
        actual.Y.ShouldBe(expected.Y, 0.0001f);
        actual.Z.ShouldBe(expected.Z, 0.0001f);
    }

    [Fact]
    public void OpenGlMatrixUploadConvention_PreservesFaceOnHeightForPositiveYView()
    {
        var corners = CreateCorners(
            minX: -0.230f,
            maxX: 0.230f,
            minY: 0f,
            maxY: 0.004f,
            minZ: -0.130f,
            maxZ: 0.130f);
        var eye = new Vector3(0f, 1f, 0f);
        var target = Vector3.Zero;
        var up = Vector3.UnitZ;
        var view = Matrix4x4.CreateLookAt(eye, target, up);
        var aspect = 490f / 974f;
        var halfHeight = MathF.Max(0.26f / 2f, 0.46f / (2f * aspect));
        var projection = Matrix4x4.CreateOrthographic(
            halfHeight * 2f * aspect,
            halfHeight * 2f,
            0.1f,
            100f);

        var rowMajorMvp = view * projection;
        var uploadMatrix = ReconstructGpuMatrix(rowMajorMvp);
        var extents = ProjectToClipSpace(uploadMatrix, corners);

        extents.Width.ShouldBe(2f, 0.01f);
        extents.Height.ShouldBeGreaterThan(0.5f);
    }

    private static Quaternion CreateCameraOrientation(Vector3 direction)
    {
        var method = typeof(AssetPreviewOpenGlControl).GetMethod(
            "CreateCameraOrientation",
            BindingFlags.NonPublic | BindingFlags.Static);
        method.ShouldNotBeNull();
        return (Quaternion)method.Invoke(null, [direction])!;
    }

    private static Vector4[] CreateCorners(float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
    {
        return
        [
            new Vector4(minX, minY, minZ, 1f),
            new Vector4(minX, minY, maxZ, 1f),
            new Vector4(minX, maxY, minZ, 1f),
            new Vector4(minX, maxY, maxZ, 1f),
            new Vector4(maxX, minY, minZ, 1f),
            new Vector4(maxX, minY, maxZ, 1f),
            new Vector4(maxX, maxY, minZ, 1f),
            new Vector4(maxX, maxY, maxZ, 1f)
        ];
    }

    private static AssetPreviewClipExtents ProjectToClipSpace(Matrix4x4 matrix, IReadOnlyList<Vector4> corners)
    {
        var minX = float.PositiveInfinity;
        var maxX = float.NegativeInfinity;
        var minY = float.PositiveInfinity;
        var maxY = float.NegativeInfinity;
        foreach (var corner in corners)
        {
            var projected = MultiplyColumnVector(matrix, corner);
            var normalizedX = projected.X / projected.W;
            var normalizedY = projected.Y / projected.W;
            minX = MathF.Min(minX, normalizedX);
            maxX = MathF.Max(maxX, normalizedX);
            minY = MathF.Min(minY, normalizedY);
            maxY = MathF.Max(maxY, normalizedY);
        }

        return new AssetPreviewClipExtents(maxX - minX, maxY - minY);
    }

    private static Matrix4x4 ReconstructGpuMatrix(Matrix4x4 rowMajorMatrix)
    {
        float[] values =
        [
            rowMajorMatrix.M11,
            rowMajorMatrix.M12,
            rowMajorMatrix.M13,
            rowMajorMatrix.M14,
            rowMajorMatrix.M21,
            rowMajorMatrix.M22,
            rowMajorMatrix.M23,
            rowMajorMatrix.M24,
            rowMajorMatrix.M31,
            rowMajorMatrix.M32,
            rowMajorMatrix.M33,
            rowMajorMatrix.M34,
            rowMajorMatrix.M41,
            rowMajorMatrix.M42,
            rowMajorMatrix.M43,
            rowMajorMatrix.M44
        ];

        return new Matrix4x4(
            values[0],
            values[4],
            values[8],
            values[12],
            values[1],
            values[5],
            values[9],
            values[13],
            values[2],
            values[6],
            values[10],
            values[14],
            values[3],
            values[7],
            values[11],
            values[15]);
    }

    private static Vector4 MultiplyColumnVector(Matrix4x4 matrix, Vector4 vector)
    {
        return new Vector4(
            (matrix.M11 * vector.X) + (matrix.M12 * vector.Y) + (matrix.M13 * vector.Z) + (matrix.M14 * vector.W),
            (matrix.M21 * vector.X) + (matrix.M22 * vector.Y) + (matrix.M23 * vector.Z) + (matrix.M24 * vector.W),
            (matrix.M31 * vector.X) + (matrix.M32 * vector.Y) + (matrix.M33 * vector.Z) + (matrix.M34 * vector.W),
            (matrix.M41 * vector.X) + (matrix.M42 * vector.Y) + (matrix.M43 * vector.Z) + (matrix.M44 * vector.W));
    }

    private sealed record AssetPreviewClipExtents(float Width, float Height);
}
