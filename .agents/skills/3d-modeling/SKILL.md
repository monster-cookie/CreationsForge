---
name: 3d-modeling
description: Plan, create, inspect, or modify 3D assets, Blender scenes, meshes, materials, UVs, textures, rigs, and game-ready models using available 3D tools.
---

# 3D modeling

Establish the destination engine or application and its asset contract: scale, axes, format, materials, import behavior, and relevant performance budgets. Inspect neighboring assets and source references to preserve consistent proportions, pivots, naming, and appearance.

Use the available modeling tools and preserve an editable source. Keep topology, normals, transforms, object hierarchy, UVs, and material assignments appropriate for the consumer. Add collision, levels of detail, rigs, or animation only when required by the asset's use. Avoid assuming a generic exporter or game pipeline matches the target.

Check the source and exported result for the properties that matter: geometry, scale, orientation, textures, materials, and supported animation or collision data. Inspect destination import or rendering when available. Distinguish a successful export from a successful import or in-game result.

When a tool or runtime is unavailable, state the resulting limitation. A specification can be a useful fallback but must not be presented as a modified scene or generated asset. Report the delivered files, meaningful checks, and remaining consumer acceptance steps. Inspect partial outputs before retrying an interrupted render, bake, or export.
