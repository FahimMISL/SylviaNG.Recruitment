# EP-15 F2: Scoped Access (HR "My Postings" + Candidate Audit)

## Epic

EP-15: Access Control & User Management. F2 applies F1's foundation to two specific access patterns.

## User stories

- **US-113 — Scoped Job Posting Access** (Must Have, M): originally spec'd as a dedicated Hiring Manager role scoped to postings they created. User dropped the `HiringManager` role mid-epic ("Hiring manager = HR") and, when asked how a global HR lockdown would coexist with HR's existing full system-wide access, picked: **add an additive "My Postings" view for HR, don't restrict anything existing.**
- **US-114 — Candidate Scope Audit** (Must Have, S): confirm candidate-facing endpoints are scoped to the requester's own identity; patch real gaps, add 403 tests.

## Why

Both are Must Have, natural follow-on once F1's UserAccount/permission foundation exists. Bundled since both are "verify/patch the existing access surface" work rather than new entities.

## Scope decisions (deviate from the literal brief — flagged for review)

1. **No new role.** Per user decision, US-113 does not add `HiringManager` back. HR keeps 100% of its existing access everywhere (~40 controllers unchanged); a new **`GET /recruitment/job-posting/my-postings`** endpoint is purely additive — it does not replace or gate anything else. The existing `GetAll`/`GetPaged` endpoints are untouched.
2. **`JobPosting.CreatedBy` had to actually be wired up first.** Found during implementation, not assumed: `Audit.CreatedBy`/`UpdatedBy` were never populated anywhere in the codebase — `JobPostingService`'s existing `TryGetCurrentUserId` tried `long.TryParse` on the JWT's `sub` claim, which is always a Keycloak GUID (or a username string on the hardcoded-auth scheme), so it always silently returned null. Fixed by resolving the claim through the new `IUserAccountRepository.GetIdByKeycloakUserIdAsync` (F1's local UserAccount mirror) instead of parsing it directly. Both `CreateAsync` (new) and `UpdateAsync` (existing, was silently broken) now stamp real values when the current user has a resolvable local UserAccount row.
3. **"My Postings" degrades to an empty list, not an error, when the current user has no local UserAccount row.** This covers the hardcoded-auth scheme (no Keycloak identity at all) and any real Keycloak user who predates EP-15 (invited before the UserAccount table existed) — both cases return `[]` rather than throwing, since "you have no tracked postings" is a legitimate, non-error state for those identities today.

## Real gap found during implementation (not an assumption) — fixed as part of this story

`JobApplicationController`'s generic `GetById`/`Update`/`Delete` actions (`GET/PUT/DELETE /recruitment/job-application/{id}`) had **no role restriction and no ownership check at all** — reachable by any authenticated user, including a Candidate, for any application id. Confirmed live: a Candidate token could `GET` another candidate's full application (PII, CV link, status), or `PUT`/`DELETE` it outright. Not something EP-15 introduced — pre-existing since these actions were added, apparently superseded by the later `dashboard/paged`/`{id}/detail` (Admin/HR) and `my-applications`/`my-applications/{id}/withdraw` (Candidate, already correctly ownership-checked via `JobApplicationService.WithdrawMyApplicationAsync`) endpoints without the original three ever being locked down or removed. Confirmed dead from the frontend (no `getById`/`update`/`delete` calls anywhere in `job-application.service.ts`) before restricting them to `[Authorize(Roles = "Admin,HR")]`, matching every other admin-facing action already on that controller.

## Key files

### Backend (`SylviaNG.Recruitment-master`)
- `Application/Services/JobPostingService.cs` — `CreateAsync`/`UpdateAsync` now stamp `CreatedBy`/`UpdatedBy` via the fixed resolver; new `GetMyPostingsAsync`
- `Application/Interfaces/Services/IJobPostingService.cs` — `GetMyPostingsAsync`
- `Application/Interfaces/Repositories/IJobPostingRepository.cs`/`Infrastructure/Repositories/JobPostingRepository.cs` — `GetByCreatedByAsync`
- `Application/Interfaces/Repositories/IUserAccountRepository.cs`/`Infrastructure/Repositories/UserAccountRepository.cs` — `GetIdByKeycloakUserIdAsync` (lightweight lookup)
- `Application/Features/JobPostings/Queries/JobPostingGetMyPostings/**` (new)
- `Controllers/JobPostingController.cs` — new `GET my-postings` action
- `Controllers/JobApplicationController.cs` — `[Authorize(Roles="Admin,HR")]` added to `GetById`/`Update`/`Delete` (IDOR fix)
- `SylviaNG.Recruitment.Tests/Smoke/JobApplicationScopeSmokeTests.cs` (new) — Candidate 403 on the three fixed routes, HR not-forbidden
- `SylviaNG.Recruitment.Tests/Services/JobPostingServiceTests.cs` — updated constructor, new `GetMyPostingsAsync` empty-state test

### Frontend (`sylviang.adminui.recruitment-main`)
- `src/app/@core/services/recruitment/job-vacancy/job-vacancy.service.ts` — `getMyPostings()`
- `src/app/pages/job-vacancy-management/job-vacancy-list/job-vacancy-list.component.ts/.html` — "My Postings Only" checkbox toggle, client-side paginated (the backend endpoint is unpaginated by design - a single user's own postings is a small result set)

## Verification

- Backend: `dotnet test` — 759/759 passing (5 new: 3 candidate-403 route cases, 1 HR-not-forbidden, 1 GetMyPostings-empty-state).
- Backend live end-to-end (curl, real Keycloak + Postgres): invited two independent HR users via F1's User Accounts; each created their own job posting; confirmed **HR Tester's `my-postings` returns only their own posting, HR Second's returns only theirs** (cross-checked, not a coincidental id match — first pass through this test had a false-positive from seed data sharing `UserAccountId=1`, caught and re-verified with a second independent user before trusting it). Confirmed Admin (no UserAccount row) gets `[]` from `my-postings` but unrestricted results from `GetAll` — additive claim holds. Confirmed the JobApplication IDOR fix live: Candidate token gets 403 on `GET/PUT/DELETE /job-application/{id}`, HR token does not (404, since no such id — correctly past the auth gate), Candidate's own `my-applications` endpoint still works (200).
- Frontend: `ng build` clean.
- **Not verified**: browser rendering/interaction of the "My Postings" toggle — no browser-automation tool available this session (same caveat as F1).

## Known follow-ups (not blocking, flagged for later)

- `CreatedBy`/`UpdatedBy` are now correctly wired for `JobPosting` specifically. Other entities' `Audit.CreatedBy` fields remain unpopulated (same pre-existing gap, out of scope here) — if a future feature needs "created by me" on a different entity, it will need the same fix applied there.
- Any Keycloak-authenticated HR/Admin user who existed before EP-15 (i.e. never went through F1's Invite User flow) has no local UserAccount row, so their existing job postings show `CreatedBy = null` and never appear in their own "My Postings" view until/unless a backfill step creates a UserAccount row for them.
