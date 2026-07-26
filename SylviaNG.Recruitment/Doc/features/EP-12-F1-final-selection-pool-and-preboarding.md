# EP-12 Feature 1 — Final Selection Pool + Pre-Boarding Collection (US-094, US-095)

## What

Auto-created `FinalSelectionPool` entry (joining date/batch/has-joined tracking) the moment a candidate accepts an offer letter, an HR list screen to manage pool entries (batch label/joining date edit, mark-joined action), and a candidate self-service pre-boarding form (nominees/emergency contact/insurance/bank) with draft-save and a one-way lock on submit.

## Why

First of 3 EP-12 features — the entry point of the onboarding data pipeline. F2 (HR validation/lock + fitment data config) and F3 (office note PDF) follow after this.

Branched off `demo/local-showcase` (not `dev` — dev is missing EP-09/EP-10 prerequisites this feature builds on: notification dispatch, `OfferLetter`/`OfferLetterService.AcceptAsync`). `JoiningBooklet.cs`'s own doc-comment from EP-10 explicitly anticipated this feature ("no FinalSelectionPool entity exists yet - that's US-094/EP-12").

## Design decisions

- **Pool entry has no Create endpoint.** `FinalSelectionPoolService.CreateFromAcceptedOfferAsync` is internal-only, called directly from `OfferLetterService.AcceptAsync` right after the existing `NotifyHrOfDecisionAsync(OfferAccepted)` call — every Accepted offer gets exactly one pool row, with no other path able to accept an offer without also enrolling it. A defensive `GetByOfferLetterIdAsync` existence check guards against a double-create, on top of `EnsureUndecided`'s re-accept block and a DB-level unique index on `OfferLetterId`.
- **Two separate save commits, not a shared transaction.** `CreateFromAcceptedOfferAsync` issues its own `SaveChangesAsync` after `AcceptAsync`'s own save — matches this codebase's existing multi-save-per-request pattern (e.g. `OfferLetterService.GenerateAsync` saves the offer, then separately dispatches a notification). No `IUnitOfWork.BeginTransactionAsync` wrapper exists anywhere else in the codebase to model a true single-transaction guarantee against.
- **`PreBoardingSubmission.Status` uses `new` to shadow `Audit.Status` (int)**, the same pattern `OfferLetter.Status` already uses — `Audit` declares a generic `int Status` for soft-delete/lifecycle bookkeeping unrelated to the domain-specific enum.
- **Nominees are a full-replace collection, not per-item CRUD.** `PreBoardingService.SaveDraftAsync` clears and rebuilds `PreBoardingSubmission.Nominees` on every save, since the whole pre-boarding form (including nominees) submits as one payload from a single Angular `FormArray` — deliberately diverges from `CandidateEducation`'s per-item add/edit/delete endpoints, which fit a different UX (one section, saved independently per item).
- **Draft auto-provisions on first GET**, mirroring `CurrentCandidateService`'s profile auto-provisioning — `PreBoardingService.GetForCurrentCandidateAsync` creates an empty `Draft` `PreBoardingSubmission` the first time a candidate with a pool entry hits the endpoint, rather than requiring an explicit "start" action.
- **Submit is a one-way lock enforced in the service, not the DB.** `EnsureNotLocked` rejects any `SaveDraftAsync`/`SubmitAsync` once `Status == Submitted` with the existing `InvalidStatusTransitionException` — F2 (not built yet) owns the correction-request/reopen workflow. `EnsureReadyToSubmit` validates emergency contact + bank fields fully populated, at least one nominee, and nominee `SharePercentage` summing to exactly 100 — all via a `FluentValidation.ValidationException` with per-field failures, matching `OfferLetterService.GenerateAsync`'s inline-validation style rather than a separate FluentValidation validator (these are cross-field business rules over persisted state, not the command payload FluentValidation validators check).
- **2 new `RecruitmentEventEnum` values** (`PreBoardingRequested` — candidate notified on pool entry; `PreBoardingSubmitted` — HR notified on submit) — no seed data; `EventTemplateMapping` rows are authored at runtime via the existing admin screen, same as every prior `RecruitmentEventEnum` addition. `DispatchAsync` never throws on a missing mapping.
- **HR pool controller is `[Authorize(Roles = "Admin,HR")]`**, not Admin-only like `OfferLetterController` — that controller's own comment flags its Admin-only scoping as specific to that feature's original build, not the majority convention; this is HR's core onboarding queue, so it follows the more common `Admin,HR` pairing used by ~18 of the ~20 other routing modules.
- **HR list gets its own module** (`pages/final-selection-pool-management/`), not folded into `document-management` — a pool entry isn't a document, mirrors how `talent-pool-management` got its own module for the same reason.
- **No Employee/Core-HR linkage in this feature.** `CandidateProfile.EmployeeId` (US-005 Core HR sync) and the pool entry are deliberately not connected here — pool entry is recruitment-side only; there's no Employee-picker widget precedent in this codebase (deferred per US-062), and Employee sync is one-way inbound from Core HR via Kafka, not something this feature writes to.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Domain/Entities/{FinalSelectionPool,PreBoardingSubmission,PreBoardingNominee}.cs` — new.
- `Domain/Enums/Enum.cs` — `PreBoardingSubmissionStatusEnum` new, 2 `RecruitmentEventEnum` values appended.
- `Infrastructure/Configurations/{FinalSelectionPool,PreBoardingSubmission,PreBoardingNominee}Configuration.cs` — new. `Infrastructure/Data/ApplicationDBContext.cs` — 3 new `DbSet<>`.
- `Migrations/20260726073629_AddFinalSelectionPoolAndPreBoarding.cs` — new migration.
- `Application/Interfaces/Repositories/{IFinalSelectionPoolRepository,IPreBoardingSubmissionRepository}.cs` + `Infrastructure/Repositories/{FinalSelectionPoolRepository,PreBoardingSubmissionRepository}.cs` — new.
- `Application/Interfaces/Services/{IFinalSelectionPoolService,IPreBoardingService}.cs` + `Application/Services/{FinalSelectionPoolService,PreBoardingService}.cs` — new. `Application/Services/OfferLetterService.cs` — `IFinalSelectionPoolService` dependency added, `AcceptAsync` now calls `CreateFromAcceptedOfferAsync`.
- `Application/Features/FinalSelectionPools/**` — CQRS: GetAll/GetById/MarkHasJoined/UpdateBatch, full new vertical (no Create — internal only).
- `Application/Features/PreBoarding/**` — CQRS: GetForCandidate/SaveDraft/Submit, full new vertical.
- `Application/Mappings/{FinalSelectionPoolMapper,PreBoardingMapper}.cs` — new.
- `Controllers/{FinalSelectionPoolController,PreBoardingCandidateController}.cs` — new.
- `Application/Extensions/DependencyInjection.cs`, `Infrastructure/Extensions/DependencyInjection.cs` — registered new services/repositories.
- `SylviaNG.Recruitment.Tests/Services/{FinalSelectionPoolServiceTests,PreBoardingServiceTests}.cs` — new. `OfferLetterServiceTests.cs` — extended constructor for `IFinalSelectionPoolService`, `AcceptAsync` test asserts the new hook call.

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/enums/recruitment.enum.ts` — `PreBoardingSubmissionStatusEnum` new.
- `@core/interfaces/recruitment-management/{final-selection-pool,pre-boarding}.interface.ts` — new.
- `@core/services/recruitment/{final-selection-pool,pre-boarding-candidate}/*.service.ts` — new.
- `pages/final-selection-pool-management/**` — new module: `final-selection-pool-list` (table, batch-edit dialog, mark-joined action). Registered as a lazy route in `pages-routing.module.ts`.
- `pages/candidate-profile-management/pre-boarding-form/**` — new: candidate self-service form (emergency contact/insurance/bank flat sections + nominees `FormArray` repeater), Save Draft/Submit, read-only once locked. Registered in `candidate-profile-management.module.ts`, routed at `pre-boarding` in `candidate-profile-management-routing.module.ts`.
- `@core/constants/nav-menu-items.ts` — "Final Selection Pool" (Admin/HR, under Recruitment), "Pre-Boarding" (Candidate).

## Verification

- `dotnet build` clean; `dotnet test` — 666/669 passing, the 3 failures are the pre-existing documented `InternalJobBoardControllerTests` NRE baseline, unrelated. All new `FinalSelectionPoolServiceTests`/`PreBoardingServiceTests` pass, plus the extended `OfferLetterServiceTests.AcceptAsync` hook assertion.
- `dotnet ef database update` applied cleanly against local Postgres; new tables (`FinalSelectionPools`, `PreBoardingSubmissions`, `PreBoardingNominees`) confirmed created with expected columns/FKs/unique indexes.
- `npx tsc --noEmit` clean; `ng build` compiles clean — `final-selection-pool-management-module` and `candidate-profile-management-module` chunks both regenerated with the new components.
- Full end-to-end via `curl` against the running local stack (Keycloak + Postgres, HR/Admin test logins, a freshly registered candidate account with OTP temporarily disabled locally for the session then restored): applied a job application on the candidate's behalf, generated an offer letter as Admin, accepted it as the candidate — confirmed a `FinalSelectionPool` row appeared automatically on the HR list endpoint with the correct `JoiningDate` seeded from the offer and `preBoardingStatus: null`. As the candidate: GET auto-created a `Draft` submission; saved a partial draft and confirmed it persisted across a re-GET; submit with zero nominees was correctly rejected (`"At least one nominee is required before submitting."`); completed the form and submitted successfully, confirmed `Status` flipped to `Submitted` with `SubmittedAt` set; a further draft-save attempt was correctly rejected with the lock error. As HR: confirmed the pool list's `preBoardingStatus` flipped to `"Submitted"`, set a batch label + joining date via the update-batch endpoint and confirmed it persisted, marked the entry Joined and confirmed `HasJoined`/`JoinedAt` persisted on a fresh GET. Confirmed both `PreBoardingRequested` and `PreBoardingSubmitted` rows were written to `NotificationLogs` (no `EventTemplateMapping` configured yet, so logged rather than delivered — expected, same as every prior `RecruitmentEventEnum` addition).
- Browser UI not visually verified in this session (no browser-automation tool available) — API and build-level verification only, same caveat as EP-10 F1/F2.
