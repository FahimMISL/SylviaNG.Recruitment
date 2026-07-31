# EP-10 Feature 3 — Bulk Joining Booklets + Medical/Target Letters (US-084, US-086)

## What

HR bulk-generates joining booklets for a batch of Accepted-offer candidates in one action, tracks per-candidate success/failure, and downloads all generated booklets as a single ZIP. Separately, HR generates one-off medical referral and target letters for a single Accepted-offer candidate from that candidate's application detail page, using the same template-preview-then-finalize flow as the existing Appointment Letter feature. All three new document types plug into the existing document-tracking dashboard and candidate-notification pipeline built in F1/F2.

## Why

Third and last of the 3 EP-10 features. `DocumentTypeEnum.JoiningBooklet`/`MedicalReferral`/`TargetLetter` were added in F1 as unwired hook values specifically for this feature to wire up.

Branched off `demo/local-showcase` (same reason as F1/F2 — `dev` is missing this epic's prerequisites).

## Design decisions

- **US-084 AC1's "Final Selection Pool" doesn't exist yet — lightweight batch stub instead.** The Final Selection Pool is US-094 (EP-12), which comes *after* EP-10 in the approved epic order. Rather than pull a slice of EP-12 forward, HR selects directly from candidates whose `OfferLetter.Status == Accepted` (`GET /joining-booklet/eligible-candidates`) and enters a `BatchLabel`/`JoiningDate` ad hoc on the generate request — no new pool/table entity. Same "HR-entered per-generate, no source entity yet" pattern `OfferLetter.Designation`/`ReportingManager` already use. When the real Final Selection Pool lands in EP-12, only the candidate-source query changes.
- **Offer-accepted candidate query is a dedicated SQL-filtered repo method** (`IOfferLetterRepository.GetAcceptedOrderedAsync`), not an in-memory filter like `DocumentTrackingService` uses. That in-memory pattern exists there because it merges two *different* entity shapes (a genuine UNION concern) — here it's one entity type filtered by one enum column, so pushing `Status == Accepted` into the `WHERE` clause is strictly better.
- **Medical and Target letters are fully separate entities/services/controllers**, each mirroring `AppointmentLetter` 1:1 (own PDF-generator interface, own repository, own storage subfolder), not merged into one vertical despite both being simple single-generate flows — their field shapes differ (`MedicalTestCenter`/`RequiredTests` vs `Kpis`/`Objectives`) and merging would break the one-controller-per-entity route convention every other document type in this codebase follows, for the sake of saving 2 files.
- **Joining Booklet bulk flow is two endpoints, not one atomic call**: `POST /joining-booklet/bulk-generate` (persists rows, returns a per-candidate JSON success/failure report — AC5) then `POST /joining-booklet/bulk-download` (zips already-generated booklets by id — AC4). AC2 and AC4 are separate ACs, and one HTTP response can't cleanly be both a JSON failure report and a binary ZIP. This also lets HR fix one failed candidate's data and retry just that one without re-zipping the whole batch.
- **Bulk-download regenerates PDF bytes in-memory from the persisted `RenderedBody`**, rather than reading a saved file back off disk. `IFileStorageService` only has `SaveAsync`/`DeleteAsync` (no read-back) — adding a third method to a shared interface for this one narrow need wasn't worth it. `JoiningBooklet.RenderedBody`/`CandidateName`/template `Name` are exactly the same 3 inputs the PDF generator needs, so regeneration is deterministic and reuses the identical code path as single-generate.
- **Single-generate and bulk-generate share one core method** (`JoiningBookletService.GenerateForOfferLetterAsync`), called both directly by `GenerateAsync` and in a loop by `BulkGenerateAsync` — PDF-generation logic is never duplicated between the two paths. `BulkGenerateAsync` wraps each candidate's call in its own `try/catch`, so one candidate's failure (missing/invalid data) never aborts the rest of the batch (AC5).
- **`JoiningBooklet.RenderedBody` is server-rendered** (via the existing `IPlaceholderSubstitutionService`, same as `OfferLetterService`), unlike Appointment/Medical/Target letters which take a client-submitted `FinalBody` after a preview-then-finalize round trip. A bulk batch has no per-candidate review UI, so there's nothing for HR to individually edit before finalizing.
- **`DocumentTemplateRepository.CountOfferLetterUsageAsync` renamed to `CountUsageAsync` and fixed to sum across all 5 letter tables** (`OfferLetters`/`AppointmentLetters`/`JoiningBooklets`/`MedicalLetters`/`TargetLetters`), not just `OfferLetters`. This was already a pre-existing under-count once `AppointmentLetter` shipped in F2 (the delete-guard could let an admin delete a template that was actually still in use) — F3 makes the gap worse by adding 3 more referencing tables, so it's fixed now rather than left to compound further. Verified via `curl`: deleting an in-use `JoiningBooklet`-type template now correctly returns 409.
- **Medical/Target letter requests stay keyed by `OfferLetterId`**, mirroring `AppointmentLetterGenerateRequest` exactly, even though AC3 triggers generation from the application-detail page (which only has `jobApplicationId`). The frontend resolves `OfferLetterId` client-side via the existing `GET /offer-letter?jobApplicationId=` and picks the latest `Accepted` record — no new backend lookup endpoint needed, and the backend request shape stays identical to the proven Appointment Letter pattern.
- **Medical letter's "candidate reference" (AC1) is computed, not stored**: `JA-{jobApplicationId}`, same convention as `ExamTakingService`'s `EXM-...` reference numbers. No `ReferenceNumber` field exists anywhere in the domain; since it's a pure function of an id both sides already have, client and server independently compute the identical value with no round trip and no new column.
- **Medical `RequiredTests` / Target `Kpis`+`Objectives` are free-text HR-entered fields** on the generate request, not structured child entities — no test-catalog or role-configuration entity exists yet, and modeling one would be new scope beyond what these Size S/M stories called for. Same "fitment-data hook" pattern as `OfferLetter.Designation`/`ReportingManager`.
- **3 new `RecruitmentEventEnum` values** (`JoiningBookletAvailable`, `MedicalLetterAvailable`, `TargetLetterAvailable`) — no seed data needed, `EventTemplateMapping` rows are admin-authored at runtime same as F1/F2. `DispatchAsync` never throws on a missing mapping (logs a `Skipped` `NotificationLog` row) — confirmed via `curl`: dispatch logged as `Skipped` with no mapping present, then logged as `Sent` after creating one, exercising the full send path exactly like the existing Offer/Appointment letter notifications.
- **Document-tracking dashboard extended with 3 more in-memory-merge blocks**, all mapping `AcceptanceStatus = NotApplicable` (none of the 3 new types have a candidate-decision step, same as `AppointmentLetter`).
- **No candidate-portal UI built for booklets/medical/target letters.** AC5 (US-086) and the implicit candidate-facing side of US-084 are satisfied entirely by the existing email-notification mechanism (`{{PortalLink}}`) — consistent with F1/F2's own scope boundary, no new candidate-facing pages needed for these ACs.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Domain/Entities/{JoiningBooklet,MedicalLetter,TargetLetter}.cs` — new. `OfferLetter.cs`/`JobApplication.cs`/`DocumentTemplate.cs` — 3 new nav collections each.
- `Domain/Enums/Enum.cs` — 3 `RecruitmentEventEnum` values (no `DocumentTypeEnum` change — hook values already existed from F1).
- `Infrastructure/Configurations/{JoiningBooklet,MedicalLetter,TargetLetter}Configuration.cs` — new. `Infrastructure/Data/ApplicationDBContext.cs` — 3 new `DbSet<>`.
- `Migrations/20260725125359_AddJoiningBookletMedicalAndTargetLetters.cs` — new migration, applied to local Postgres.
- `Application/Interfaces/Repositories/{IJoiningBooklet,IMedicalLetter,ITargetLetter}Repository.cs` + `Infrastructure/Repositories/*Repository.cs` — new. `IOfferLetterRepository`/`OfferLetterRepository` — `GetAcceptedOrderedAsync`/`GetByIdsWithDetailsAsync` added. `IDocumentTemplateRepository`/`DocumentTemplateRepository` — `CountOfferLetterUsageAsync` renamed to `CountUsageAsync`, now sums all 5 letter tables; `DocumentTemplateService`'s one call site updated.
- `Application/Interfaces/Services/I{JoiningBooklet,MedicalLetter,TargetLetter}PdfGeneratorService.cs` + `Infrastructure/Documents/QuestPdf{JoiningBooklet,MedicalLetter,TargetLetter}Generator.cs` — new, same QuestPDF shape as F1/F2's generators.
- `Application/Interfaces/Services/I{JoiningBooklet,MedicalLetter,TargetLetter}Service.cs` + `Application/Services/{JoiningBooklet,MedicalLetter,TargetLetter}Service.cs` — new.
- `Application/Mappings/{JoiningBooklet,MedicalLetter,TargetLetter}Mapper.cs` — new.
- `Application/Features/{JoiningBooklets,MedicalLetters,TargetLetters}/**` — new CQRS verticals (Generate/GetAll/GetById for Medical/Target; Generate/BulkGenerate/BulkDownload/GetAll/GetById/GetEligibleCandidates for JoiningBooklet).
- `Controllers/{JoiningBooklet,MedicalLetter,TargetLetter}Controller.cs` — new, `[Authorize(Roles = "Admin")]`.
- `Application/Services/DocumentTrackingService.cs` — 3 new repos injected, 3 new merge blocks + `ToTrackingItem` overloads.
- `Application/Extensions/DependencyInjection.cs`, `Infrastructure/Extensions/DependencyInjection.cs` — registered new services/repositories/PDF generators.
- `SylviaNG.Recruitment.Tests/Services/{JoiningBooklet,MedicalLetter,TargetLetter}ServiceTests.cs` — new. `DocumentTemplateServiceTests.cs` — updated for the `CountUsageAsync` rename.

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/interfaces/recruitment-management/{joining-booklet,medical-letter,target-letter}.interface.ts` — new.
- `@core/services/recruitment/{joining-booklet,medical-letter,target-letter}/*.service.ts` — new. `joining-booklet.service.ts` reuses `cv-bank.service.ts`'s exported `saveFileResponse` helper for the ZIP download rather than duplicating it.
- `pages/document-management/{joining-booklet-list,joining-booklet-batch-generate,medical-letter-form,target-letter-form}/**` — new, added to the existing `document-management` module (no new lazy module).
- `pages/document-management/document-tracking-list/document-tracking-list.component.ts` — filter dropdown widened to include the 3 new document types.
- `pages/application-tracking/application-detail/application-detail.component.html` — "Generate Medical Letter"/"Generate Target Letter" action links added next to the existing Offer Letter actions.
- `pages/document-management/document-management-routing.module.ts`/`.module.ts` — new routes/declarations.
- `@core/constants/nav-menu-items.ts` — "Joining Booklets" nav entry added (Medical/Target letters have no standalone nav entry — reachable only from the application-detail page per AC3).

## Verification

- `dotnet build` clean; `dotnet test` — 652/655 passing, the 3 failures are the pre-existing documented `InternalJobBoardControllerTests` NRE baseline, unrelated. All 29 new Joining Booklet/Medical Letter/Target Letter service tests pass.
- `dotnet ef database update` applied cleanly against local Postgres — 3 new tables, all FKs `DeleteBehavior.Restrict`, verified in the generated migration diff before applying.
- `ng build` compiles clean; `document-management-module` chunk regenerated with the 4 new components; pre-existing initial-bundle-size warning unrelated. Browser UI not visually verified this session (no browser-automation tool available, same caveat as F1/F2) — `ng build` plus the backend `curl` pass below are the verification signal.
- Full end-to-end via `curl` against the running local stack (Admin fallback account), one check per AC, using real pre-existing demo data (an Accepted `OfferLetter` for candidate "Sadia Test"):
  - US-084 AC1: `GET /joining-booklet/eligible-candidates` returns only the Accepted-offer candidate.
  - US-084 AC2/AC3: `POST /joining-booklet/bulk-generate` with a valid `OfferLetterId` generates a booklet using the configured `JoiningBooklet`-type template, placeholders substituted correctly.
  - US-084 AC4: `POST /joining-booklet/bulk-download` with the generated id returns a valid ZIP containing one correctly-named PDF entry.
  - US-084 AC5: bulk-generate called with one valid + one nonexistent `OfferLetterId` returns `successCount: 1, failureCount: 1` with a per-candidate error message, batch not aborted.
  - US-086 AC1: medical letter generated with `MedicalTestCenter`/`RequiredTests` and computed `CandidateReference` (`JA-23`) correctly substituted into the preview and final body.
  - US-086 AC2: target letter generated with `Kpis`/`Objectives` correctly substituted.
  - US-086 AC3: both generate endpoints work given only the `OfferLetterId` resolved from `jobApplicationId` via the existing `GET /offer-letter?jobApplicationId=` (the frontend's AC3 entry-point flow).
  - US-086 AC4: both letters persisted and downloadable (`GET /medical-letter`, `GET /target-letter` return them with their `GeneratedPdfPath`).
  - US-086 AC5 (and the implicit US-084 notification): confirmed `DispatchAsync` fires for all 3 new events (`NotificationLog` row with `Skipped` status and "No active template mapping" reason when unmapped), then created a template + `EventTemplateMapping` for `JoiningBookletAvailable` and re-generated — confirmed a second `NotificationLog` row with `Sent` status, proving the full send path works end-to-end.
  - Document-tracking dashboard: `GET /document-tracking?documentType=X` returns the newly generated documents for all 3 new types.
  - `CountUsageAsync` fix: `DELETE /document-template/{id}` on the in-use `JoiningBooklet` template returns 409 with the correct usage count.
