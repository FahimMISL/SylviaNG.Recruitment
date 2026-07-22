# Fix — Persistent Candidate Identity Link on Job Applications

## What

Adds a real foreign key, `JobApplication.CandidateProfileId → CandidateProfile`, resolved at submission time. Replaces the pattern of matching `JobApplication.CandidateEmail` against `CandidateProfile.Email` as a string comparison every time an application needs to be related to its candidate (My Applications, US-043 shortlist filter evaluation, CV Bank/Talent Pool resume reuse, marking a hired candidate internal, the US-003 identity-field lock).

## Why

Raised during senior demo-prep review, comparing this app's apply flow against how modern ATS platforms (Greenhouse/Lever/Workday) resolve identity. The email-string-match pattern was spread across five call sites and is fragile: a typo or case mismatch silently drops an application from "My Applications," and an email change orphans history entirely — which is *why* `Email`/`Phone`/`NationalId` are locked once a candidate has applied (see `CandidateProfileResponse.HasSubmittedApplication`'s doc comment). This fix removes the root cause instead of continuing to work around it.

## Design notes

- **No synthetic/placeholder identities.** `CandidateProfile.KeycloakSubjectId` is `IsRequired()` + unique-indexed — profile creation stays gated on real Keycloak auth, exactly as before. A guest with no existing profile simply gets `CandidateProfileId = null` on their application — explicit null, same reality as the previous implicit gap, nothing invented.
- **Resolved at submission time, not backfilled by wishful matching later.** `JobApplicationService.SubmitAsync`/`CreateAsync` resolve `CandidateProfileId` via `ResolveCandidateProfileIdAsync`: prefer the actually-authenticated submitter's own profile, else an existing profile matching the typed email, else null.
- **Role gate on "current user's own profile" — this was the one real bug caught during verification.** `ICurrentCandidateService.TryGetCurrentCandidateProfileIdAsync()` auto-provisions a `CandidateProfile` for *any* authenticated Keycloak subject regardless of role (pre-existing behavior of `GetOrCreateCurrentProfileIdAsync`, previously safe because it was only ever called from Candidate-role-restricted actions). Calling it unconditionally from `SubmitAsync`/`CreateAsync` — which HR/Admin also hit (apply-on-behalf, the plain `POST /job-application` create) — silently linked an HR staffer's own profile to an application they typed in on someone else's behalf. Caught live during manual verification (HR user `abir` submitting for "Rumana Chowdhury" got linked to `abir`'s own profile). Fixed by gating on `User.IsInRole("Candidate")` before ever calling the auto-provisioning path.
- **Claim-on-register, not a background job.** `CurrentCandidateService.GetOrCreateCurrentProfileAsync`, right after creating a brand-new profile, bulk-links any `JobApplication` rows where `CandidateProfileId IS NULL AND CandidateEmail == email` to the new profile — one deterministic moment (registration), not a cron/reconciliation job. Matches the existing "guest apply → nudge to create account" flow (`b713f7e`) exactly: the guest applies without friction, and the moment they act on that nudge, their history attaches.
- **Backfill migration for existing data.** The same migration that adds the column also runs a one-time `UPDATE` matching `JobApplication.CandidateEmail` to `CandidateProfile.Email` (case-insensitive) wherever exactly one profile matches — covers all pre-existing rows in one pass, then the FK is the only mechanism going forward.
- **`WithdrawMyApplicationAsync`'s ownership check is security-relevant, not just a join.** Strengthened, never weakened: `CandidateProfileId` match is authoritative when set; email-equality is the fallback only for a row that predates the FK or that this candidate submitted as a guest before registering. Verified live: a second candidate's token correctly gets 404 (not revealing existence) attempting to withdraw someone else's application.
- **Explicitly out of scope**, per the approved plan:
  - Duplicate detection (US-038, `AreDuplicates`/`GetDuplicatesAsync`) — operates on raw `JobApplication` pairs (email/phone/NationalId, each independently sufficient), not a `CandidateProfile` join. No correctness bug there; untouched. Could take `CandidateProfileId` as a stronger 4th signal later.
  - `GetAttributeFilteredApplicationsAsync` (US-043 bulk shortlist eval) — left as its existing email-bulk-resolve; not a correctness bug, just a perf opportunity not worth the scope creep here.
  - Relaxing the `Email`/`Phone`/`NationalId` edit lock — technically safe once the FK exists, but a separate UX decision.
- **Frontend: no changes.** Response DTOs (`JobApplicationResponse`, `CandidateProfileResponse`) needed no new fields — this is a backend data-integrity fix behind existing contracts.

## Files changed

**Backend only** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Domain/Entities/JobApplication.cs` — new nullable `CandidateProfileId` + `CandidateProfile?` nav.
- `Domain/Entities/CandidateProfile.cs` — new `JobApplications` collection nav.
- `Infrastructure/Configurations/CandidateProfileConfiguration.cs` — relationship declared from the principal side (`Restrict` delete behavior, matching `HiringPipelineConfiguration`'s convention).
- `Infrastructure/Configurations/JobApplicationConfiguration.cs` — index on `CandidateProfileId`.
- `Migrations/20260722053459_AddCandidateProfileIdToJobApplication.cs` — schema + one-time backfill `UPDATE`.
- `Application/Interfaces/Services/ICurrentCandidateService.cs`, `Application/Services/CurrentCandidateService.cs` — new `TryGetCurrentCandidateProfileIdAsync()` (role-gated), claim-on-register logic in `GetOrCreateCurrentProfileAsync`.
- `Application/Interfaces/Repositories/ICandidateProfileRepository.cs` + `Infrastructure/Repositories/CandidateProfileRepository.cs` — new `GetIdByEmailAsync`.
- `Application/Interfaces/Repositories/IJobApplicationRepository.cs` + `Infrastructure/Repositories/JobApplicationRepository.cs` — new `GetByCandidateAsync` (FK-first, email-fallback), `LinkUnclaimedApplicationsByEmailAsync`.
- `Application/Interfaces/Services/IJobApplicationService.cs`, `Application/Services/JobApplicationService.cs` — `ResolveCandidateProfileIdAsync` used by `SubmitAsync`/`CreateAsync`; `MarkCandidateInternalByEmailAsync` renamed `MarkCandidateInternalAsync` (FK-first); `GetMyApplicationsAsync`/`WithdrawMyApplicationAsync` swapped to `GetByCandidateAsync`, ownership check strengthened.
- `Application/Services/CandidateProfileService.cs` — `HasSubmittedApplicationAsync`/`GetProfileDetailAsync` swapped to `GetByCandidateAsync`.
- `Application/Services/TalentPoolService.cs` — `FastTrackAsync` passes its already-known `candidateProfileId` straight through instead of resolving by email.
- `Application/Features/JobPostings/Models/JobApplicationCreateRequest.cs`: no field added — `CreateAsync` instead gained an optional `candidateProfileId` parameter (internal-use override for `FastTrackAsync`), keeping the public request DTO unchanged.
- Tests: `CurrentCandidateServiceTests.cs` (claim-on-register, role gate — including the HR-bug regression test), `JobApplicationServiceTests.cs`, `CandidateProfileServiceTests.cs`, `TalentPoolServiceTests.cs` mock-signature updates. Also fixed unrelated pre-existing compile breaks in `CvBankSearchHandlerTests.cs`/`ShortlistFilterEvaluationServiceTests.cs`/`InternalJobBoardControllerTests.cs` (stale `PresentAddress`/`Gender` field references, missing constructor args) that were blocking the whole test assembly from building — confirmed via `git stash` these predate this change.

## Verification

- `dotnet build` — clean.
- `dotnet test` — 438 total, 435 passing. The remaining 3 failures (`InternalJobBoardControllerTests.GetAll_ShouldReturnOkWithPagedJobPostings`/`GetById_ShouldReturnOkWithJobPosting`/`Apply_...`) are pre-existing (`User.IsInRole` NullReferenceException from a missing `ControllerContext` in that test file's setup) — confirmed via `git stash` these fail identically without this change; left untouched, out of scope.
- Migration applied to local dev Postgres (`dotnet ef database update`) — schema + backfill both ran cleanly; spot-checked via `psql` that all pre-existing seeded applications with a matching registered candidate got linked, and the 3 pure walk-in applications (no profile ever created) correctly stayed `NULL`.
- Full live walkthrough against the running local backend:
  1. HR-authenticated create for a brand-new applicant email → `CandidateProfileId` correctly `NULL` (post-fix; was incorrectly linked to HR's own profile pre-fix, see bug note above).
  2. Registered that same email → `GetOrCreateCurrentProfileAsync`'s claim step backfilled the FK automatically; `GET candidate-profile/me` showed `hasSubmittedApplication: true` immediately.
  3. `GET job-application/my-applications` returned the claimed application.
  4. `PATCH job-application/my-applications/{id}/withdraw` succeeded as the owner.
  5. Same endpoint as a *different* candidate → `404` (ownership check holds, ownership not revealed).
