namespace CreationsForge.Bethesda.Assets.Nif;

public partial class NifPreviewModelReader
{
    private static Dictionary<int, NifObjectTransform> CreateLocalTransformMap(IReadOnlyList<NifBlock> blocks, List<string> diagnostics)
    {
        var transforms = new Dictionary<int, NifObjectTransform>();
        for (var blockIndex = 0; blockIndex < blocks.Count; blockIndex++)
        {
            var block = blocks[blockIndex];
            if (!IsTransformBearingBlock(block))
            {
                continue;
            }

            if (TryReadNiAVObjectHeader(block.Data, out var transform, out var position, out var failureReason))
            {
                transforms[blockIndex] = transform;
                diagnostics.Add($"{block.TypeName} block {blockIndex}: object transform header at {position}, {transform.Description}");
            }
            else
            {
                diagnostics.Add($"{block.TypeName} block {blockIndex}: object transform header not parsed ({failureReason}), first bytes {CreateHexSample(block.Data, 0, DiagnosticHexByteCount)}");
            }
        }

        return transforms;
    }

    private static Dictionary<int, int> CreateParentMap(IReadOnlyList<NifBlock> blocks, List<string> diagnostics)
    {
        var parents = new Dictionary<int, int>();
        for (var blockIndex = 0; blockIndex < blocks.Count; blockIndex++)
        {
            var block = blocks[blockIndex];
            var failureReason = string.Empty;
            if (!block.TypeName.Contains("Node", StringComparison.Ordinal) ||
                !TryReadNiNodeChildren(block.Data, out var children, out failureReason))
            {
                if (block.TypeName.Contains("Node", StringComparison.Ordinal))
                {
                    diagnostics.Add($"{block.TypeName} block {blockIndex}: children not parsed ({failureReason})");
                }

                continue;
            }

            diagnostics.Add($"{block.TypeName} block {blockIndex}: children {string.Join(", ", children)}");
            foreach (var child in children)
            {
                if (child >= 0 && child < blocks.Count && !parents.ContainsKey(child))
                {
                    parents[child] = blockIndex;
                }
            }
        }

        return parents;
    }

    private static bool IsTransformBearingBlock(NifBlock block)
    {
        return block.TypeName.Contains("Node", StringComparison.Ordinal) ||
            IsPreviewGeometryBlock(block);
    }

    private static bool IsPreviewGeometryBlock(NifBlock block)
    {
        return block.TypeName.Contains("TriShape", StringComparison.Ordinal) ||
            string.Equals(block.TypeName, "BSGeometry", StringComparison.Ordinal);
    }
}
