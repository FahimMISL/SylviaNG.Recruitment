# EP-12 Feature 2 + Feature 3 — Pre-Boarding Validation, Fitment Data, Office Note (US-096, US-097, US-129)

## What

Three small additions bundled onto one branch/PR since each is individually small:

- **F2a — HR Validates and Locks Pre-Boarding Data (US-096):** HR reviews a candidate's submitted `PreBoardingSubmission`, then either Validates (locks as `Approved`) or Requests Corrections (`NeedsCorrection`, with a comment) — which re-opens the candidate's form for edits exactly like `Draft`.
- **F2b — Configure Fitment Data for Salary and Grade (US-097):** a new `FitmentData` entity (grade/designation/location/salary structure) manually entered per `JobApplication`, standalone CRUD screen.
- **F3 — Generate Office Note for Onboarding Enclosures (US-129):** HR generates a PDF listing whichever onboarding documents (offer letter, appointment letter, joining booklet) exist for a `JobApplication`, with a free-text remarks field.

## Why

F2/F3 follow F1 (Final Selection Pool + Pre-Boarding collection, already live). Branched off `demo/local-showcase` (not `dev` — dev is still missing EP-09/EP-10/EP-12-F1 prerequisites, same gap as F1). Both small enough to combine into one branch/doc/PR per the approved plan.

## Design decisions

- **FitmentData is 1:1 with `JobApplication`** (unique FK index, mirrors `PreBoardingSubmission`↔`FinalSelectionPool`).
- **`FitmentData.Designation`/`Grade`/`Location` are plain strings** — no new Designation lookup entity; matches `OfferLetter.Designation`'s existing convention, and no local Designation table exists anywhere in this codebase (`JobPosting.DesignationId`/`Employee.DesignatioId` are unmapped external FKs).
- **Offer-letter-form pre-populate from FitmentData is deferred.** `OfferLetterService`/`OfferLetterGenerateRequest` are untouched this round — zero risk to existing working code. Follow-on note only, not built.
- **Payroll auto-fetch (US-097 AC2/AC3) skipped.** Payroll integration is EP-16, explicitly out of scope for this project (per prior descoping decision) — manual entry only.
- **"Verification summary" enclosure dropped from US-129.** EP-11 verification workflow was descoped entirely from this project. Office note enclosures = offer letter, appointment letter, joining booklet only.
- **Office note bulk generation skipped.** AC5 covers individual generation only.
- **No new Angular modules.** Office Note + Fitment Data fold into the existing `document-management` module (same PDF/document-generation shape as offer-letter/joining-booklet). The pre-boarding review screen folds into the existing `final-selection-pool-management` module (reached via a row action, no new top-level nav entry for it).
- **Correction comment stored on a new `PreBoardingSubmission.CorrectionComment` field**, not the unused generic `Audit.Remarks` (zero usages elsewhere in the codebase at the time).
- **`OfficeNote.Remarks` reuses the inherited `Audit.Remarks` field directly** instead of declaring a second, identically-typed property that would shadow it — a genuine first real use of that generic bookkeeping column.
- **AC4 "document history"** satisfied by extending the existing `DocumentTrackingService.GetAllAsync` in-memory aggregator with an `OfficeNote` branch, not a new construct.
- **The crux fix for AC5:** `PreBoardingService.EnsureNotLocked` changed from `Status != Draft` to `Status is not (Draft or NeedsCorrection)` — this one-line change is what actually re-opens the candidate's save/submit after HR requests a correction. No new candidate-side endpoint was needed; the existing `SubmitAsync` carries `NeedsCorrection → Submitted` on resubmit.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Domain/Enums/Enum.cs` — `PreBoardingSubmissionStatusEnum` +`Approved`/+`NeedsCorrection`; `RecruitmentEventEnum` +`PreBoardingApproved`/+`PreBoardingCorrectionRequested`; `DocumentTypeEnum` +`OfficeNote`.
- `Domain/Entities/PreBoardingSubmission.cs` — +`CorrectionComment`. `Domain/Entities/{FitmentData,OfficeNote}.cs` — new. `Domain/Entities/JobApplication.cs` — +`FitmentData?`/+`OfficeNotes` navs. `Domain/Entities/DocumentTemplate.cs` — +`OfficeNotes` nav.
- `Infrastructure/Configurations/{FitmentData,OfficeNote}Configuration.cs` — new. `PreBoardingSubmissionConfiguration.cs` — extended. `Infrastructure/Data/ApplicationDBContext.cs` — 2 new `DbSet<>`.
- `Migrations/20260726144402_AddFitmentDataOfficeNoteAndPreBoardingCorrection.cs` — new migration (applied to local Postgres).
- `Application/Interfaces/Repositories/{IFitmentDataRepository,IOfficeNoteRepository}.cs` + `Infrastructure/Repositories/{FitmentDataRepository,OfficeNoteRepository}.cs` — new. `IPreBoardingSubmissionRepository`/impl — +`GetByIdWithDetailsAsync`.
- `Application/Services/PreBoardingService.cs`/`IPreBoardingService.cs` — +`GetByFinalSelectionPoolIdForHrAsync`/+`ValidateAsync`/+`RequestCorrectionAsync`, `EnsureNotLocked` fix. `Application/Services/{FitmentDataService,OfficeNoteService}.cs` + interfaces — new. `DocumentTrackingService.cs` — extended with an OfficeNote branch.
- `Application/Interfaces/Services/IOfficeNotePdfGeneratorService.cs` + `Infrastructure/Documents/QuestPdfOfficeNoteGenerator.cs` — new (copy of the JoiningBooklet QuestPDF generator).
- `Application/Features/PreBoarding/**` — extended: `Queries/PreBoardingGetByPoolForHr`, `Commands/{PreBoardingValidate,PreBoardingRequestCorrection}`, `Models/PreBoardingRequestCorrectionRequest`; `PreBoardingSubmissionResponse`/`PreBoardingMapper` +`CorrectionComment`.
- `Application/Features/FitmentDatas/**` — new CQRS vertical (Models/Queries/Commands). `Application/Features/OfficeNotes/**` — new CQRS vertical (Models/Queries×3/Commands).
- `Application/Mappings/{FitmentDataMapper,OfficeNoteMapper}.cs` — new.
- `Controllers/{PreBoardingController,FitmentDataController,OfficeNoteController}.cs` — new, all `[Authorize(Roles = "Admin,HR")]`.
- `Application/Extensions/DependencyInjection.cs`, `Infrastructure/Extensions/DependencyInjection.cs` — registered new services/repositories/PDF generator.
- `SylviaNG.Recruitment.Tests/Services/{PreBoardingServiceTests,FitmentDataServiceTests,OfficeNoteServiceTests}.cs` — extended/new.

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/enums/recruitment.enum.ts` — `PreBoardingSubmissionStatusEnum` +`Approved`/+`NeedsCorrection`; `DocumentTypeEnum` +`OfficeNote`.
- `@core/interfaces/recruitment-management/{pre-boarding,fitment-data,office-note}.interface.ts` — extended/new.
- `@core/services/recruitment/{pre-boarding,fitment-data,office-note}/*.service.ts` — new (HR-facing `PreBoardingService`, distinct from the candidate-only `PreBoardingCandidateService`).
- `pages/final-selection-pool-management/pre-boarding-review/**` — new: HR read-only review + Validate/Request-Correction actions. Row action added to `final-selection-pool-list`; new route in `final-selection-pool-management-routing.module.ts`.
- `pages/document-management/fitment-data-form/**`, `pages/document-management/office-note-list/**`, `pages/document-management/office-note-generate/**` — new, folded into the existing `document-management` module/routing (3 new routes with `Admin,HR` roleData, distinct from the module's existing Admin-only routes).
- `pages/candidate-profile-management/pre-boarding-form/pre-boarding-form.component.ts/.html` — `isLocked` now includes `Approved`; new `needsCorrection` banner showing HR's comment.
- `@core/constants/nav-menu-items.ts` — "Fitment Data"/"Office Notes" added under the Recruitment group (Admin/HR); no separate nav entry for pre-boarding review (row-action only).

## Verification

- `dotnet build` clean; `dotnet test` — 682/686 passing (4 pre-existing failures unrelated to this feature: 3 documented `InternalJobBoardControllerTests` NRE baseline, 1 Keycloak smoke test needing a seeded `sadia` user). All new `PreBoardingServiceTests`/`FitmentDataServiceTests`/`OfficeNoteServiceTests` pass.
- `dotnet ef database update` applied cleanly against local Postgres; `FitmentDatas`/`OfficeNotes` tables + `PreBoardingSubmissions.CorrectionComment` column confirmed created.
- `npx tsc --noEmit` clean; `ng build` compiles clean (`document-management-module` and `final-selection-pool-management` chunks regenerated).
- Full end-to-end via `curl` against the running local stack (Keycloak + Postgres, candidate-login OTP temporarily disabled for the session then restored):
  - **F2 cycle:** using a leftover F1-verification pool entry (`finalSelectionPoolId=1`, `Submitted`) — HR `request-correction` with a comment → `NeedsCorrection` + comment persisted; candidate `GET` confirmed unlocked and the comment visible; candidate `PUT` a field edit succeeded (previously would have thrown `InvalidStatusTransitionException` — the AC5 regression check); candidate `submit` → `Submitted`; HR `validate` → `Approved`, comment cleared; a second `validate` on an already-`Approved` submission correctly rejected (`400`, "cannot transition from status Approved to Approved"); HR `request-correction` again from `Approved` correctly succeeded (AC5 "reopen a locked submission").
  - **F3 flow:** `PUT /fitment-data` create then update (same `fitmentDataId`, no duplicate row) confirmed; `GET /office-notes/enclosures?jobApplicationId=25` correctly showed only Offer Letter present; authored an active `OfficeNote`-type `DocumentTemplate`; `POST /office-notes/generate` succeeded, `enclosuresSummary` = `"Offer Letter"`; generated PDF fetched via its static path and confirmed to be a real single-page PDF (36KB); `GET /document-tracking?documentType=OfficeNote` confirmed the new note listed (AC4); a job application with zero generated documents correctly rejected `generate` with `400` ("No enclosures exist yet... generate at least an offer letter first").
- Browser UI not visually verified in this session (no browser-automation tool available) — API and build-level verification only, same caveat as prior EP-12 features.
