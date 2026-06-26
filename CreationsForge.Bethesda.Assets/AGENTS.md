# CreationsForge.Bethesda.Assets rules

This folder owns Bethesda asset archive and asset lookup behavior, including BA2/BSA reading and normalized asset metadata.

## Boundaries

- Asset archive parsing and metadata extraction belong here.
- Presentation rendering, preview controls, and UI fallback states belong in CreationsForge.
- Game-specific import orchestration belongs in the relevant game project or Core service, not directly in low-level asset readers.
- Do not make this project depend on Avalonia or presentation-only types.

## Archive handling

- Treat archive files as potentially large. Prefer streaming and indexed lookup over loading whole archives into memory.
- Dispose streams, file handles, decompression objects, and unmanaged resources deterministically.
- Preserve case-normalization and path-normalization conventions for archive paths and entry paths.
- Support known compressed and uncompressed paths according to existing implementation. Do not guess unsupported archive variants without inspecting the format and calling out the risk in the plan.
- Do not extract files into the repository or source tree.
- Do not write temporary files unless the plan identifies the location, cleanup behavior, and failure handling.

## Database and lookup behavior

- If archive metadata schema changes, coordinate with CreationsForge.Migrations and Documentation/Database.
- Favor lookup indexes that support exact asset path lookup, archive browsing, and preview workflows.
- Do not log full binary payloads or large serialized asset data.

## Validation

Asset changes should include small fixture coverage where practical and manual validation against at least one known BA2 or BSA archive when automated coverage is not practical.
