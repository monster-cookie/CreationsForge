---
name: graphic-design
description: Design visual assets, UI visual direction, branding, icons, logos, graphics, layouts, and image-generation concepts using available design tools.
---

# Graphic design

Establish the asset's audience, purpose, destination, dimensions, format, and visual constraints. Inspect supplied references and existing brand or UI conventions before choosing a direction. Preserve recognizable identity when extending an established design.

Use hierarchy, typography, spacing, contrast, composition, and scale to support the intended use. For game or application assets, account for the actual consumer's transparency, sizing, format, and integration requirements. Explore alternatives only where a design decision benefits from them.

Use available native image tools for appropriate raster work and vector tools for vector deliverables. Preserve editable source when practical. The reasoning model and image backend are separate: claim a backend only when tool metadata establishes it, and select one only when the tool supports selection. Do not provision an API or credentials solely to force a preferred model. If an exact backend is required but cannot be verified, report that execution gap.

Inspect the final asset at the intended size and relevant smaller sizes. Check legibility, contrast, crop, alpha, dimensions, and export properties that affect the consumer. An output file existing does not prove it renders correctly. If asset creation is unavailable, return a clearly labeled specification rather than claiming an asset was produced.

Deliver the source or export, relevant verification, and material remaining integration limits. A separate testing guide or PR diagram is not a routine design requirement.
