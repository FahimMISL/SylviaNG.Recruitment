# EP-13 Feature 2 — Bulk CV Download, Admit-Card ZIP Naming Fix, Candidate Profile PDF (US-101/102/103)

## What

The remaining 3 EP-13 stories after F1 (US-100 candidate-list export + US-104 async queue), shipped as one combined feature/branch, same pattern as EP-12 F2+F3:

- **US-101** — "Bulk Download CVs" on the ATS application list. Selects a batch of applications, downloads all of them as one ZIP (one system-rendered CV PDF per application, `CandidateName_ApplicationID.pdf`). Batches of 20 or fewer download synchronously and instantly; larger batches queue through the EP-13 F1 async export-request infrastructure and are downloaded from the existing Export Requests page.
- **US-102** — "Download All Admit Cards" on the exam detail page. The backend and frontend for this already existed end-to-end from the earlier admit-card/seat-plan epic (`GET recruitment/exam/{examId}/admit-cards/download/zip`) — the only gap was the ZIP entry naming convention, fixed to `AdmitCard_{ExamTitle}_{CandidateName}.pdf` (was `Admit-Card-{JobApplicationId}.pdf`).
- **US-103** — "Download Profile" on the HR-facing candidate profile page. Generates a standardized, branded PDF (photo, personal details, education/experience summary, skills, certifications, latest screening score if any), synchronous, no async queue.

## Why

Next feature in the approved roadmap after EP-13 F1. Bundled as one branch since all 3 remaining stories are small/independent (S-M sized) and none has a hard dependency on the others - same rationale as EP-12's F2+F3 combination.

Branched off `demo/local-showcase`, not `dev` - same recurring stack gap every EP-09-and-later feature has hit (dev has no notification/export/download/admit-card infrastructure). PR-to-dev link given after push regardless.

## Design decisions

- **US-101 CVs are system-rendered, not the raw uploaded file** — consistent with CV Bank's existing design (the app already treats "CV" as the standardized profile-rendered PDF, not `JobApplication.ResumeUrl`). Guest applicants with no `CandidateProfileId` are silently skipped (no CV to render) - consistent with AC5 ("only CVs accessible to permission level are included").
- **Sync vs async split for US-101**: `JobApplicationService.BulkDownloadCvsSyncMaxCount` (20) decides the path. `≤20` selected applications → `POST recruitment/job-application/bulk-download-cvs`, an in-memory ZIP built and returned in the same request (mirrors `CvBankCvBulkDownloadHandler`'s pattern exactly). `>20` → `POST recruitment/export-requests/bulk-cv-zip`, which queues a `Pending` `ExportRequest` row (new `ExportTypeEnum.BulkCvZip`, `ExportFormatEnum.Zip`) for `ExportRequestWorker` to render. The worker now `switch`es on `entity.ExportType` to call the right generation method instead of always calling the candidate-list-export generator - exactly the extension point F1's doc comments called out ("F2/F3 of EP-13 add more export types onto the same queue"). No EF migration was needed for the new enum values since both are stored as `string` columns (`HasConversion<string>()`).
- **Shared ZIP-building logic**: `Application/Common/Utilities/CvZipBuilder.cs` (new) is used by both `JobApplicationService.BulkDownloadCvsAsync` (sync path) and `ExportGenerationService.GenerateBulkCvZipAsync` (async path) - same in-memory `MemoryStream`/`ZipArchive`/dedup-suffix pattern `CvBankCvBulkDownloadHandler` established, just keyed by `JobApplicationId`/`CandidateName` instead of `CandidateProfileId`. File naming (`CandidateName_ApplicationID.pdf`, AC3) is a second method on the existing `CvFileNaming` helper.
- **US-102**: no new backend/frontend endpoints - fixed ZIP entry naming inside the existing `ExamSeatPlanService.GenerateAdmitCardZipAsync`. "Failed-delivery fallback" (AC4) was already satisfied by default - the method has never filtered by `EmailNotificationStatus`/`SmsNotificationStatus`, every enrollment for the exam is always bundled; added a regression test asserting this explicitly rather than changing behavior.
- **US-103 screening score**: `CandidateProfile` has no direct score field - score lives on `AutoShortlistResult`, keyed by `JobApplicationId`/`JobPostingId`. The profile PDF shows the score of the candidate's **most-recently-applied** scored application (`CandidateProfileService.ResolveLatestScreeningScoreAsync` walks `ApplicationHistory` newest-first, caching `IAutoShortlistRunRepository.GetLatestScoresByJobPostingIdAsync` results per job posting to avoid redundant queries), or "N/A" if none of their applications were ever auto-shortlisted.
- **Response model duplication is intentional**, matching this codebase's existing convention (`CvBankCvFileResponse`/`ExportRequestFileResponse` are already separate near-identical shapes per feature) - `JobApplicationCvBulkDownloadResponse` and `CandidateProfileDownloadResponse` follow the same pattern rather than introducing a new shared "any binary file" DTO.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Application/Features/CvBank/CvFileNaming.cs` — `ToApplicationCvFileName` added.
- `Application/Common/Utilities/CvZipBuilder.cs` — new, shared zip-building loop.
- `Application/Features/JobPostings/Models/JobApplicationCvBulkDownloadModels.cs` — new (`JobApplicationCvBulkDownloadRequest`/`Response`).
- `Application/Interfaces/Services/IJobApplicationService.cs` + `Application/Services/JobApplicationService.cs` — `BulkDownloadCvsAsync`, `BulkDownloadCvsSyncMaxCount`, `ICvPdfGeneratorService` added to constructor.
- `Controllers/JobApplicationController.cs` — `POST bulk-download-cvs`.
- `Domain/Enums/Enum.cs` — `ExportTypeEnum.BulkCvZip`, `ExportFormatEnum.Zip`.
- `Application/Interfaces/Services/IExportGenerationService.cs` + `Application/Services/ExportGenerationService.cs` — `GenerateBulkCvZipAsync`, `ICvPdfGeneratorService` added to constructor.
- `Application/Interfaces/Services/IExportRequestService.cs` + `Application/Services/ExportRequestService.cs` — `RequestBulkCvZipExportAsync`.
- `Infrastructure/BackgroundServices/ExportRequestWorker.cs` — dispatches on `entity.ExportType`.
- `Application/Features/ExportRequests/Commands/ExportRequestCreateBulkCvZip/**` — new CQRS command/handler.
- `Controllers/ExportRequestController.cs` — `POST bulk-cv-zip`.
- `Application/Services/ExamSeatPlanService.cs` — `GenerateAdmitCardZipAsync` naming fix (`BuildAdmitCardEntryName`/`Sanitize`).
- `Application/Interfaces/Services/ICandidateProfilePdfGeneratorService.cs` — new.
- `Infrastructure/Documents/QuestPdfCandidateProfileGenerator.cs` — new.
- `Application/Features/CandidateProfiles/Models/CandidateProfileDownloadResponse.cs` — new.
- `Application/Interfaces/Services/ICandidateProfileService.cs` + `Application/Services/CandidateProfileService.cs` — `DownloadProfilePdfAsync`, `ResolveLatestScreeningScoreAsync`, `IAutoShortlistRunRepository`/`ICandidateProfilePdfGeneratorService` added to constructor.
- `Application/Features/CandidateProfiles/Queries/CandidateProfileDownloadPdf/**` — new CQRS query/handler.
- `Controllers/CandidateProfileController.cs` — `GET {candidateProfileId}/download/pdf`.
- `Infrastructure/Extensions/DependencyInjection.cs` — `ICandidateProfilePdfGeneratorService` registration.
- Tests: `JobApplicationServiceTests`, `ExportGenerationServiceTests`, `ExportRequestServiceTests`, `ExamSeatPlanServiceTests`, `CandidateProfileServiceTests` — new cases for all of the above.

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/services/recruitment/job-application/job-application.service.ts` — `bulkDownloadCvs`.
- `@core/services/recruitment/export-request/export-request.service.ts` — `requestBulkCvZipExport`.
- `pages/application-tracking/ats-dashboard/` — "Bulk Download CVs" button in the existing bulk-action bar; `bulkDownloadCvs()` picks sync vs async by `BULK_DOWNLOAD_CVS_SYNC_MAX` (20).
- `@core/services/recruitment/candidate-profile/candidate-profile.service.ts` — `downloadProfilePdf`.
- `pages/candidate-management/candidate-detail/` — "Download Profile" button in the profile header, unconditional.
- US-102: no frontend change — the exam detail page's "Download All Admit Cards (ZIP)" button (built under the earlier admit-card epic) already calls the fixed backend endpoint.

## Verification

- `dotnet build` clean; `dotnet test` — 704/708 passing. The 4 failures: 3 pre-existing documented `InternalJobBoardControllerTests` NRE baseline (unrelated), plus 1 `AuthLoginSmokeTests` failure that requires a live local Keycloak to pass (environment-dependent, unrelated to this change). All new US-101/102/103 test cases pass.
- `npx tsc --noEmit` clean; `ng build` compiles clean (only a pre-existing initial-bundle-size budget warning, unrelated).
