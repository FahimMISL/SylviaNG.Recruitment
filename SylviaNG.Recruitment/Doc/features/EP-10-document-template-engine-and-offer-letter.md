# EP-10 Feature 1 — Document Template Engine + Offer Letter Generation (US-080, US-081)

## What

An admin-managed `DocumentTemplate` engine (8 closed `DocumentType`s: OfferLetter, AppointmentLetter, JoiningBooklet, MedicalReferral, TargetLetter, RejectionLetter, ExperienceCertificate, RelievingLetter), each edit auto-snapshotted into `DocumentTemplateVersion` history, with `{{Placeholder}}` preview via the existing `PlaceholderSubstitutionService`. Offer letter generation (`OfferLetterController.Generate`) takes a `JobApplicationId` + an active OfferLetter-type template + HR-entered offer fields (designation, salary, joining date, reporting manager, validity date), renders the body, generates a single-page PDF via a new `QuestPdfOfferLetterGenerator`, persists it via the existing `IFileStorageService`, and records an `OfferLetter` row (`Status = Generated`). Admin CRUD screens live under System Administration; a "Generate Offer Letter" / "View Offer Letters" action pair was added to the existing Application Detail page.

## Why

First of 3 EP-10 features (7 stories, US-080–086). Every other letter type (F3) and the accept/decline + appointment-letter flow (F2) build on this engine + the generated offer letter existing, so it had to land first.

Branched off `demo/local-showcase` (not `dev` — `git merge-base dev demo/local-showcase` showed `dev` is a strict ancestor of `demo/local-showcase` with zero unique commits; `dev` predates Candidate Profile, Interview, Exam, and the EP-09 template engine this feature reuses).

## Design decisions

- **Reused EP-09's engine wholesale instead of building a parallel one.** `NotificationTemplate`/`NotificationTemplateVersion` (CRUD + auto-versioning) and `PlaceholderSubstitutionService` (`{{Token}}` regex substitution) already existed and are structurally identical to what US-080 asks for. `DocumentTemplate`/`DocumentTemplateVersion` mirror that shape exactly (own table, since document generation is a distinct concern from notification dispatch — different `DocumentType` closed set, no `Channel`/`Subject`); `IPlaceholderSubstitutionService` is injected and reused unchanged, no new substitution code written.
- **No EP-12 fitment-data entity to source offer fields from** — EP-12 (Fitment) is scheduled after EP-10 in the roadmap and doesn't exist yet. HR enters designation/salary/joining date/reporting manager/validity date directly in the generate request; that request payload is the fitment-data hook for now, to be replaced by a real EP-12 read once that epic lands.
- **PDF rendering is plain-text-with-line-breaks, not real rich text.** `DocumentTemplate.Body` has no HTML/rich-text editor in this pass (a plain `<textarea>`, same as `NotificationTemplate.Body`); QuestPDF can't render arbitrary HTML anyway. `QuestPdfOfferLetterGenerator` splits the already-substituted body on newlines and renders each line as its own paragraph, mirroring `QuestPdfAdmitCardGenerator`'s `Document.Create`/`Compose*` shape.
- **Storage reuses `IFileStorageService`/`FileStorageSettings`** (existing local-disk abstraction, subfolder `documents/offer-letters`) rather than a new storage setting — same reuse convention already established for candidate photo/signature/document uploads.
- **Business-rule validation thrown as `FluentValidation.ValidationException` from the service**, not a generated validator — template-type mismatch (selected template must be `DocumentType.OfferLetter`) and inactive-template checks happen in `OfferLetterService.GenerateAsync` since they need the loaded entity, matching EP-09's channel-mismatch convention.
- **No dedicated download endpoint.** `OfferLetter.GeneratedPdfPath` is a web-relative path served by static-file middleware directly (same convention as `CandidateProfile.ProfilePhotoPath`/`JobPostingAttachment`), not proxied through a controller action like CV Bank's on-demand generation. The frontend builds the download URL the same way `candidate-list.component.ts`'s `getPhotoUrl()` does.
- **Every endpoint `[Authorize(Roles = "Admin")]`**, HR-only tooling — F2 will add its own candidate-facing accept/decline endpoints separately, not here.
- **Found + fixed a pre-existing compile-breaking test gap** unrelated to this feature: `AuthServiceTests`/`JobApplicationServiceTests` constructor calls were out of sync with `AuthService`/`JobApplicationService` (missing `OtpSettings`/`ICandidateLoginOtpRepository`/`INotificationDispatchService`/`IMemoryCache`/`IUnitOfWork` params, and `INotificationDispatchService` respectively) — left broken by a prior feature's commit (US-075/076/077) that didn't update its own tests. `dotnet test` could not run at all on `demo/local-showcase` before this fix. Minimal fix: added the missing mocked dependencies, no test logic changed.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Domain/Enums/Enum.cs` — `DocumentTypeEnum`, `OfferLetterStatusEnum` appended.
- `Domain/Entities/DocumentTemplate.cs`, `DocumentTemplateVersion.cs`, `OfferLetter.cs` — new. `JobApplication.OfferLetters` nav collection added.
- `Infrastructure/Configurations/DocumentTemplateConfiguration.cs`, `DocumentTemplateVersionConfiguration.cs`, `OfferLetterConfiguration.cs` — new.
- `Infrastructure/Data/ApplicationDBContext.cs` — 3 new DbSets.
- `Migrations/20260725062809_AddDocumentTemplateEngineAndOfferLetter.cs` — new migration.
- `Application/Interfaces/{Repositories,Services}/I{DocumentTemplate,OfferLetter}{Repository,Service}.cs`, `IOfferLetterPdfGeneratorService.cs` + `Infrastructure/Repositories/`, `Application/Services/` implementations — new.
- `Infrastructure/Documents/QuestPdfOfferLetterGenerator.cs` — new, same QuestPDF shape as `QuestPdfAdmitCardGenerator`.
- `Application/Features/DocumentTemplates/**` — CQRS: Create/Update/Delete/GetAll/GetById/GetVersions/Preview.
- `Application/Features/OfferLetters/**` — CQRS: Generate/GetAll/GetById.
- `Application/Mappings/DocumentTemplateMapper.cs`, `OfferLetterMapper.cs` — new.
- `Controllers/DocumentTemplateController.cs` (`recruitment/document-template`), `OfferLetterController.cs` (`recruitment/offer-letter`) — new.
- `Application/Extensions/DependencyInjection.cs`, `Infrastructure/Extensions/DependencyInjection.cs` — registered new services/repositories/PDF generator.
- `SylviaNG.Recruitment.Tests/Services/DocumentTemplateServiceTests.cs`, `OfferLetterServiceTests.cs` — new (14 tests). `AuthServiceTests.cs`, `JobApplicationServiceTests.cs` — fixed pre-existing constructor drift (see above), no assertions changed.

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/enums/recruitment.enum.ts` — `DocumentTypeEnum`, `OfferLetterStatusEnum` mirrors.
- `@core/interfaces/recruitment-management/document-template.interface.ts`, `offer-letter.interface.ts` — new.
- `@core/services/recruitment/document-template/document-template.service.ts`, `offer-letter/offer-letter.service.ts` — new.
- `pages/document-management/` — new module: `document-template-list`/`-form`, `offer-letter-list`/`-form`, routing module, module.ts.
- `pages/pages-routing.module.ts` — lazy `document-management` route.
- `pages/application-tracking/application-detail/application-detail.component.html` — "Generate Offer Letter" / "View Offer Letters" action buttons, pass `jobApplicationId` as a query param.
- `@core/constants/nav-menu-items.ts` — 2 new "System Administration" subitems (Document Templates, Offer Letters).

## Verification

- `dotnet build` clean; `dotnet test` — 627/630 passing, the 3 failures are the pre-existing documented `InternalJobBoardControllerTests` NRE baseline (`ControllerContext.User` gap), unrelated — confirmed via `git status` that none of the touched files overlap. All 14 new tests pass.
- `dotnet ef database update` applied cleanly against local Postgres.
- `ng build` compiles clean, `document-management-module` chunk generated (33.19 kB); pre-existing initial-bundle-size warning unrelated to this feature.
- Full end-to-end via `curl` against the running local stack (logged in as the local Admin fallback user): created an OfferLetter-type `DocumentTemplate` with placeholders, confirmed `Preview` renders correctly and detects placeholders, confirmed duplicate `Code` is rejected (409); generated an offer letter against a real `JobApplication`, confirmed the response and `GetAll`/`GetById` return the correct candidate/template names; downloaded the generated PDF via its static URL and confirmed it is a valid single-page PDF (`file` reports `PDF document, version 1.4, 1 page(s)`, 35KB); confirmed generating against a wrong-`DocumentType` template (AppointmentLetter) is rejected (400).
