# CV Bank: Standardized CV Download + Bulk Export

## What

HR/Admin can now download a standardized CV/resume PDF for any candidate found via CV Bank
search - individually, or in bulk. Bulk selection also supports an Excel export where every
candidate is one row with the same fixed set of columns.

- `GET /recruitment/cv-bank/{candidateProfileId}/cv/download` - single candidate's CV as PDF.
- `POST /recruitment/cv-bank/cv/bulk-download` - one PDF per selected candidate, zipped.
- `POST /recruitment/cv-bank/cv/bulk-export-excel` - one row per selected candidate, fixed columns.

## Why

CV Bank (US-045) only had search + talent-pool add/remove - no way to actually get a candidate's
CV out of the system. The CV is generated from the candidate's structured profile data (education/
work-experience/skills/certifications), not their uploaded resume file, so every CV - and every
row in a bulk Excel export - follows the identical template/column set regardless of what the
candidate actually filled in.

## Design notes

- **Generated, not uploaded.** `ICvPdfGeneratorService`/`QuestPdfCvGenerator` renders a
  `CandidateProfile` into a fixed-template PDF (header photo/contact, Education table, Work
  Experience, Skills, Certifications). It never touches `CandidateDocument`/uploaded resume files -
  that's a deliberate scope split from `IApplicationCvStorageService`, which serves the
  as-uploaded resume elsewhere.
- **New repository method, not a full-table scan.** `CvBankSearchHandler` already had
  `GetAllActiveWithDetailsAsync()` (loads every active profile), but a single/bulk CV download only
  needs the selected ids - `GetByIdsWithDetailsAsync(ids)` was added instead of reusing the
  full-scan method, and backs all three new endpoints (single download wraps one id in a list).
- **Bulk PDF is a ZIP of individual files, not one merged PDF.** Keeps each candidate's CV a
  separately usable file after download, matching how HR actually re-shares/archives resumes.
- **Excel bulk export flattens child collections into one delimited cell per column** (e.g. all
  educations joined "; ") rather than expanding into extra rows/columns - keeps every row the same
  shape regardless of how many degrees/jobs/skills a given candidate has.
- **QuestPDF Community license.** Set once in `Infrastructure/Extensions/DependencyInjection.cs`
  (`QuestPDF.Settings.License = LicenseType.Community`) - free tier, matches this project's size/
  revenue profile.
- **No new NuGet source needed at build time** but the configured `ProxyFeed` source
  (`192.168.1.210:8081`, presumably office-network-only) was unreachable in this session - both
  packages were restored via `-s https://api.nuget.org/v3/index.json` as a one-off. If other
  machines hit the same proxy-unreachable error, restoring once with that same `-s` flag (or adding
  nuget.org as a fallback source) resolves it; no project file change was needed since the
  PackageReference itself doesn't pin a source.

## Files touched

- `Application/Interfaces/Repositories/ICandidateProfileRepository.cs`,
  `Infrastructure/Repositories/CandidateProfileRepository.cs` - `GetByIdsWithDetailsAsync`.
- `Application/Interfaces/Services/ICvPdfGeneratorService.cs`,
  `Infrastructure/Documents/QuestPdfCvGenerator.cs` - new.
- `Application/Features/CvBank/CvFileNaming.cs` - shared PDF filename sanitizer (single + bulk-zip).
- `Application/Features/CvBank/Models/CvBankCvFileResponse.cs`,
  `Application/Features/CvBank/Models/CvBankCvBulkRequest.cs` - new.
- `Application/Features/CvBank/Queries/CvBankCvDownload/*`,
  `Application/Features/CvBank/Queries/CvBankCvBulkDownload/*`,
  `Application/Features/CvBank/Queries/CvBankCvBulkExportExcel/*` - new CQRS slices.
- `Controllers/CvBankController.cs` - three new endpoints.
- `Infrastructure/Extensions/DependencyInjection.cs` - QuestPDF license + `ICvPdfGeneratorService`
  registration.
- `SylviaNG.Recruitment.csproj` - `QuestPDF` 2026.7.1, `ClosedXML` 0.105.0.
- Frontend: `cv-bank.service.ts` (blob-returning download methods), `cv-bank-search.component.ts/html`
  (row checkboxes + select-all, per-row download button, bulk download/export toolbar).
