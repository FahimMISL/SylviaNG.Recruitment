# EP-18 F1: Document Branding Foundation

## Epic

EP-18: Unified Document Design System — redesign every generated document (admit card, offer letter, certificates, etc.) into one branded, corporate-grade visual language, inspired by the MISL admit card reference, sellable as a SaaS product to other organizations.

## Why

Every generated document today looks like a plain report (see [architecture audit](../../../../../../../../../../.claude — see project memory `document-generation-architecture`)). The 10 existing generators (`Infrastructure/Documents/QuestPdf*Generator.cs`) each hand-roll their own QuestPDF layout with zero shared components, no branding concept, and no theme system. MISL wants to sell this RMS commercially — every document must read as an official corporate artifact of one consistent product family.

## Scope (this feature)

Foundation only — no visual redesign of any individual document yet (that's F2/F3).

1. **`CompanyBranding` entity** (org-scoped, SaaS-ready schema, single MISL row seeded for now):
   - Logo (path/URL), Primary/Secondary/Accent color, Font family
   - Header layout, Footer layout enums
   - Margins, Border style, Corner radius
   - Background watermark toggle/opacity
   - Header divider style, Footer divider style
   - QR code position, Signature position, Seal position (enums: TopLeft/TopRight/BottomLeft/BottomRight/Center etc.)
   - Document reference number format (token string, e.g. `{ORG}/{DOCTYPE}/{YEAR}/{SEQ}`)
   - Page number toggle
2. **Shared QuestPDF component layer** (`Infrastructure/Documents/Shared/`) using QuestPDF's `IComponent` pattern, each resolving styling from `CompanyBranding`:
   - `DocumentHeaderComponent` — logo, company name, reference number, issue date, divider
   - `DocumentFooterComponent` — address, phone, email, website, page number
   - `InfoSectionComponent` — labeled card/section for structured info (reusable for exam details, salary summary, etc.)
   - `SignatureBlockComponent` — signature image/line + name + title
   - `QrCodeComponent` — generates + positions a QR code (verification URL or reference payload)
   - `ReferenceNumberComponent` — formats/renders the reference number per branding config
3. **`IBrandingResolverService`** — resolves the active `CompanyBranding` for the current tenant context (single-tenant now, tenant-scoped lookup already shaped for later multi-tenant use since Finbuckle `TenantInfo` is already wired).

## Out of scope (later features)

- Redesigning any actual document (F2: Admit Card + Offer Letter reference implementation)
- Rolling remaining 8 generators onto the shared layer (F3)
- Angular branding-settings admin UI, live template editor, theme presets, multi-tenant switcher UI

## Key files

- `Domain/Entities/CompanyBranding.cs` (new)
- `Migrations/*_AddCompanyBranding.cs` (new)
- `Infrastructure/Documents/Shared/DocumentHeaderComponent.cs`, `DocumentFooterComponent.cs`, `InfoSectionComponent.cs`, `SignatureBlockComponent.cs`, `QrCodeComponent.cs`, `ReferenceNumberComponent.cs` (new)
- `Application/Interfaces/Services/IBrandingResolverService.cs` + `Infrastructure/Documents/BrandingResolverService.cs` (new)
- `Infrastructure/Extensions/DependencyInjection.cs` (register new services)

## Decisions locked

- Single-tenant now (MISL only), schema built SaaS-ready — no tenant-switch UI this round.
- Keep QuestPDF as the rendering engine (no HTML-to-PDF/headless-browser swap).
- Reference implementation for F2 will be Admit Card + Offer Letter (proves shared components across a compact-card layout and a full-page-letter layout).
