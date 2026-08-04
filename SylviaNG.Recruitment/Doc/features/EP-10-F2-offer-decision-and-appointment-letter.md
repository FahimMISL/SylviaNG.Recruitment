# EP-10 Feature 2 — Offer Accept/Decline + Appointment Letter + Document Tracking (US-082, US-083, US-085)

## What

Candidate-facing accept/decline on a generated offer letter (with a mandatory reason on decline, a locked one-time decision, and an HR notification), HR-side appointment letter generation gated on an Accepted offer (pre-filled via the existing template-preview endpoint, HR can review/adjust the rendered body before finalizing), and a combined HR document-tracking dashboard showing every generated OfferLetter/AppointmentLetter with its acceptance status, filterable, with a follow-up-reminder action on Pending offers.

## Why

Second of 3 EP-10 features. F1 (US-080/081) deliberately left `OfferLetterStatusEnum.Sent/Accepted/Declined` unused and `DocumentTypeEnum.AppointmentLetter` unwired specifically for this feature to complete. F3 (US-084/086 — bulk booklets + medical/target letters) follows after this.

Branched off `demo/local-showcase` (not `dev`, same reason as F1 — dev is missing this epic's prerequisites, template engine included).

## Design decisions

- **Candidate identity resolved inside the service, not passed by the controller.** `OfferLetterService.AcceptAsync`/`DeclineAsync`/`GetAllForCandidateAsync` inject `ICurrentCandidateService` directly and call `GetOrCreateCurrentProfileIdAsync()` themselves — mirrors `ExamTakingService.GetOwnedEnrollmentAsync`'s shape rather than threading a candidateProfileId parameter through the command/controller.
- **Separate `OfferLetterCandidateController` (`recruitment/me/offer-letter`, `[Authorize(Roles = "Candidate")]`)**, not new actions on the existing Admin-only `OfferLetterController` — that controller's own F1 comment already flagged this split, and ASP.NET Core `[Authorize(Roles=...)]` at class+method level is ANDed, not ORed, so reusing the same controller would have required both roles simultaneously.
- **AC5 lock is a service-level status check, not a DB constraint.** `EnsureUndecided` rejects Accept/Decline unless `Status is Generated or Sent`, throwing the existing `InvalidStatusTransitionException`. Simpler than a DB-level guard and consistent with how every other status-gate in this codebase is enforced.
- **No new AppointmentLetter preview endpoint.** `DocumentTemplateController.POST /preview` (stateless `{Body, PlaceholderValues} -> {RenderedBody, DetectedPlaceholders}`, from F1) is reused as-is for AC3 "review and adjust before finalizing" — the frontend calls it with the template body + assembled offer-letter placeholders, seeds an editable textarea, and submits the (possibly HR-edited) text as `FinalBody` at generate time. `AppointmentLetterService.GenerateAsync` never re-renders server-side; it PDFs exactly what was submitted.
- **`AppointmentLetter` is its own entity**, mirroring `OfferLetter`'s shape (own PDF-generator interface `IAppointmentLetterPdfGeneratorService`/`QuestPdfAppointmentLetterGenerator`, own repository, own storage subfolder `documents/appointment-letters`) rather than extending `OfferLetter` with a discriminator — matches this codebase's established one-entity-per-document-type convention (`OfferLetter`, and now `AppointmentLetter`, alongside `ICvPdfGeneratorService`/`IAdmitCardPdfGeneratorService`/`ISeatPlanPdfGeneratorService` all being separate interfaces despite an identical `Generate(title, name, body)` shape). It doesn't duplicate Designation/Salary/JoiningDate — those already live on the linked `OfferLetter`; it stores `FinalBody` (the actual reviewed/edited text sent) as the AC5 document-history record, no separate history entity.
- **US-085 tracking merges two repository reads in memory**, not a SQL UNION — `DocumentTrackingService.GetAllAsync` calls `IOfferLetterRepository.GetAllOrderedAsync(null)` and `IAppointmentLetterRepository.GetAllOrderedAsync(null)`, maps both to a unified `DocumentTrackingItemResponse`, filters/paginates in memory. Acceptable at this feature's data scale; a real SQL-level merge across two differently-shaped entities wasn't worth the complexity for a Size-S story.
- **New `DocumentAcceptanceStatusEnum` (`Pending/Accepted/Declined/NotApplicable`)** decouples the tracking dashboard's status from `OfferLetterStatusEnum` — `Generated`/`Sent` both collapse to `Pending`, and `AppointmentLetter` (no candidate-decision step) always reports `NotApplicable`.
- **Follow-up (AC3) is scoped to `OfferLetter` only** — `DocumentTrackingService.FollowUpAsync` rejects `AppointmentLetter` and any non-Pending `OfferLetter` with a `ValidationException`, since "candidates who have not yet responded" only makes sense for the accept/decline flow. It re-dispatches the same `OfferLetterAvailable` event used at generation time.
- **AC4 (flag declining candidates)** is a client-side visual flag (red row highlight) on the tracking table, not a new workflow/entity — a full replacement-hire flow isn't mentioned anywhere else in EP-10 and would be scope creep for this story.
- **No new config for the portal deep-link.** F1's plan assumed a config gap here, but `PortalSettings.FrontendBaseUrl` (`Portal:FrontendBaseUrl` in appsettings, already used by `ExamNotificationService`) already exists and is reused for the `{{PortalLink}}` placeholder in both the offer-available and appointment-letter-generated notifications.
- **4 new `RecruitmentEventEnum` values** (`OfferLetterAvailable`, `OfferAccepted`, `OfferDeclined`, `AppointmentLetterGenerated`) — no seed data needed; `EventTemplateMapping` rows are created at runtime via the existing admin CRUD screen (confirmed no `HasData` pattern exists for this table), so HR must wire templates for these events post-deploy before notifications actually send. `DispatchAsync` never throws on a missing mapping (logs a Skipped `NotificationLog` row instead), so this doesn't block the feature.
- **Frontend inline PDF viewer is a plain `<iframe [src]>`** with `DomSanitizer.bypassSecurityTrustResourceUrl`, not a new npm dependency — no pdf-viewer library exists anywhere in this codebase (confirmed), and a single-document browser-native view didn't justify adding one.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Domain/Entities/OfferLetter.cs` — `DecisionAt`/`DeclineReason` added, `AppointmentLetters` nav collection. `JobApplication.cs`/`DocumentTemplate.cs` — `AppointmentLetters` nav collection added.
- `Domain/Entities/AppointmentLetter.cs` — new.
- `Domain/Enums/Enum.cs` — 4 `RecruitmentEventEnum` values, new `DocumentAcceptanceStatusEnum`.
- `Infrastructure/Configurations/AppointmentLetterConfiguration.cs` — new. `Infrastructure/Data/ApplicationDBContext.cs` — new `DbSet<AppointmentLetter>`.
- `Migrations/20260725074400_AddOfferDecisionAndAppointmentLetter.cs` — new migration.
- `Application/Interfaces/{Repositories,Services}/IAppointmentLetter{Repository,Service,PdfGeneratorService}.cs`, `IDocumentTrackingService.cs` + `Infrastructure/Repositories/AppointmentLetterRepository.cs`, `Application/Services/{AppointmentLetterService,DocumentTrackingService}.cs` — new. `IOfferLetterRepository`/`OfferLetterRepository` — `GetAllForCandidateAsync` added. `IOfferLetterService`/`OfferLetterService` — Accept/Decline/GetAllForCandidate/GetByIdForCandidate added, `GenerateAsync` now dispatches `OfferLetterAvailable`.
- `Infrastructure/Documents/QuestPdfAppointmentLetterGenerator.cs` — new, same QuestPDF shape as `QuestPdfOfferLetterGenerator`.
- `Application/Features/OfferLetters/{Commands/OfferLetterAccept,Commands/OfferLetterDecline,Queries/OfferLetterGetAllForCandidate,Queries/OfferLetterGetByIdForCandidate}/**`, `Models/OfferLetterDeclineRequest.cs` — new. `OfferLetterResponse.cs`/`OfferLetterMapper.cs` — `DecisionAt`/`DeclineReason` added.
- `Application/Features/AppointmentLetters/**` — CQRS: Generate/GetAll/GetById, full new vertical.
- `Application/Features/DocumentTracking/**` — CQRS: GetAll/FollowUp, full new vertical.
- `Application/Mappings/AppointmentLetterMapper.cs` — new.
- `Controllers/OfferLetterCandidateController.cs`, `AppointmentLetterController.cs`, `DocumentTrackingController.cs` — new.
- `Application/Extensions/DependencyInjection.cs`, `Infrastructure/Extensions/DependencyInjection.cs` — registered new services/repositories/PDF generator.
- `SylviaNG.Recruitment.Tests/Services/OfferLetterServiceTests.cs` — extended with Accept/Decline/ownership/lock tests, existing constructor updated for new deps.

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/enums/recruitment.enum.ts` — `DocumentAcceptanceStatusEnum` new, `RecruitmentEventEnum` extended.
- `@core/interfaces/recruitment-management/{offer-letter,appointment-letter,document-tracking}.interface.ts` — offer-letter extended, appointment-letter/document-tracking new.
- `@core/services/recruitment/{offer-letter-candidate,appointment-letter,document-tracking}/*.service.ts` — new.
- `pages/candidate-profile-management/{my-offer-letters,my-offer-letter-detail}/**` — new: candidate self-service list + detail (inline PDF, Accept/Decline, decline-reason dialog). Routing module extended with `offer-letters`/`offer-letters/:id`.
- `pages/document-management/{appointment-letter-form,appointment-letter-list,document-tracking-list}/**` — new. `offer-letter-list.component.html` — decision/decline-reason columns + "Appointment Letter" row action.
- `pages/document-management/document-management-routing.module.ts`/`.module.ts` — new routes/declarations.
- `@core/constants/nav-menu-items.ts` — "My Offer Letters" (Candidate), "Appointment Letters"/"Document Tracking" (System Administration).

## Verification

- `dotnet build` clean; `dotnet test` — 631/634 passing, the 3 failures are the pre-existing documented `InternalJobBoardControllerTests` NRE baseline, unrelated (confirmed via `git status` no overlap). All new OfferLetterService tests (Accept/Decline/lock/ownership) pass.
- `dotnet ef database update` applied cleanly against local Postgres.
- `ng build` compiles clean; `candidate-profile-management-module` and `document-management-module` chunks both regenerated with the new components; pre-existing initial-bundle-size warning unrelated.
- Full end-to-end via `curl` against the running local stack (Admin/Candidate fallback accounts, Keycloak unreachable so ROPC fallback path exercised): created a JobApplication as a candidate, generated an offer letter as Admin, confirmed the candidate's own list/detail endpoints return it and a 403 on a different candidate's offer letter; accepted it, confirmed a second Accept/Decline call is rejected (`InvalidStatusTransitionException`, AC5 lock); generated an appointment letter against the Accepted offer via the preview-then-finalize round-trip, confirmed the same call against a still-Pending offer is rejected (AC1 gate); downloaded the generated PDF and confirmed it's a valid single-page PDF; declined a second offer, confirmed an empty-reason decline is rejected by the validator (AC4) and a reasoned decline succeeds; confirmed the document-tracking endpoint returns all 3 documents merged with correct statuses, filters correctly by status, and rejects a follow-up on an AppointmentLetter or a decided OfferLetter while accepting one on a genuinely Pending OfferLetter.
- Browser UI not visually verified in this session (no browser-automation tool available) — API and build-level verification only, same caveat as F1.
