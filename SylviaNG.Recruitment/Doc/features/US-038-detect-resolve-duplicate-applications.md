# US-038 — Detect and Resolve Duplicate Applications

## What

Automatic detection of applications to the *same vacancy* that likely belong to the same person applying through different channels (e.g. an agency submission and a direct application), matched on email, national ID, or phone number. Detected groups surface in a dedicated "Duplicates" view (reached from the ATS Dashboard, per vacancy) with a side-by-side comparison table. HR picks one application as primary; the rest are dismissed with a `DuplicateDismissed` status, retained (not deleted) with an audit-trail note pointing back at the primary application.

## Why

EP-05's ATS epic explicitly calls out duplicate detection (agency vs. direct applicant) as a gap. The existing safeguard (`JobApplicationConfiguration`'s unique index on `(CandidateEmail, JobPostingId)`, enforced via `DuplicateException` in `JobApplicationService.CreateAsync`/`SubmitAsync`) only catches an *exact* repeat email on the same vacancy — that's US-033's "prevent re-applying with the same account" concern. It does nothing for the cross-channel case this story targets: an agency enters the candidate's details slightly differently (typo'd email, or no account at all) while the candidate also applies directly with their own email — same phone or NID, different email, currently invisible to HR as anything other than two unrelated applications.

## Design notes

- **Scope is per-vacancy, not cross-vacancy.** AC1 explicitly says "already submitted to the same vacancy" — this is narrower than general candidate deduplication across the whole ATS. Detection only ever compares applications sharing a `JobPostingId`.
- **Additive, not a replacement.** The pre-existing exact-email-same-posting hard block stays untouched. This feature fills the gap it leaves (NID/phone matches, and near-miss email variants across channels) without touching that path.
- **No new service class.** Detection and resolution logic (`GetDuplicatesAsync`/`ResolveDuplicatesAsync`) live directly on `JobApplicationService`, which already owns the status-transition machinery (`LegalStatusTransitions`, `EnsureLegalStatusTransition`, the private `ApplyStatusChangeAsync`, `ApplicationStatusHistory`) that duplicate resolution needs to reuse verbatim for its audit trail — a separate service would have had to duplicate all of that.
- **Union-find grouping, not pairwise flags.** Applications are grouped transitively: if A matches B on phone and B matches C on NID, all three land in one group even though A and C share neither field directly — matching the "email, NID, or phone" wording literally rather than requiring one consistent match key per group.
- **Detection is computed on demand, not persisted.** No new table for "flagged duplicate" records — `GetDuplicatesAsync` recomputes groups from live `JobApplication` rows each call (mirrors the existing lightweight in-memory-filter pattern `GetAttributeFilteredApplicationsAsync` already uses for US-050). `ResolveDuplicatesAsync` re-derives the group server-side rather than trusting client-supplied IDs, so HR can't dismiss an application that isn't actually part of a detected duplicate set.
- **`DuplicateDismissed` is a terminal status**, reachable from any active stage (same reachability as `Withdrawn`), but *not* exposed as a manual "next status" option in the generic per-application or bulk status-change dropdowns (`ApplicationStatusTransitions` on the frontend keeps it mapped to an empty transition array) — it's only reachable through the dedicated Duplicates resolve action, which is what records the audit note referencing the kept primary application.
- **`CandidateNationalId` is new** on `JobApplication` — it didn't exist anywhere on this entity before, needed as a match key per AC1. Added to the submit/apply-on-behalf flow (optional field) since that's the channel this feature targets.
- **Phone normalization** (`Application/Common/Utilities/PhoneNormalizer.cs`) strips non-digits and compares on the trailing 9 digits, so `+880 171-234567` and `01712-34567` still match despite differing country-code/leading-zero formatting. No such utility existed anywhere in the codebase before this.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Domain/Entities/JobApplication.cs` — new `CandidateNationalId` property.
- `Domain/Enums/Enum.cs` — new `ApplicationStatusEnum.DuplicateDismissed` value.
- `Infrastructure/Configurations/JobApplicationConfiguration.cs` — column config for the new property.
- `Application/Common/Utilities/PhoneNormalizer.cs` — new, phone-number equality normalization.
- `Application/Features/JobPostings/Models/JobApplicationDuplicateResponse.cs` — new `JobApplicationDuplicateItemResponse`/`JobApplicationDuplicateGroupResponse`.
- `Application/Features/JobPostings/Models/JobApplicationDuplicateResolveRequest.cs` — new.
- `Application/Features/JobPostings/Models/JobApplicationSubmitRequest.cs`, `JobApplicationCreateRequest.cs` — new `CandidateNationalId` field.
- `Application/Features/JobPostings/Commands/JobApplicationSubmit/JobApplicationSubmitValidator.cs` — validation rule for the new field.
- `Application/Mappings/JobPostingMapper.cs` — `ToDuplicateItemResponse`, `CandidateNationalId` threaded through the existing `ToEntity` mappings.
- `Application/Interfaces/Services/IJobApplicationService.cs`, `Application/Services/JobApplicationService.cs` — `GetDuplicatesAsync`, `ResolveDuplicatesAsync`, updated `LegalStatusTransitions`.
- `Controllers/JobApplicationController.cs` — `GET job-posting/{jobPostingId}/duplicates`, `PATCH duplicates/resolve` (Admin/HR only).
- `Migrations/20260716052654_AddCandidateNationalIdToJobApplication.cs` — new column, applied to the local dev database.
- `SylviaNG.Recruitment.Tests/Services/JobApplicationServiceTests.cs` — grouping by email/phone/NID, exclusion of already-dismissed applications, resolve happy-path + audit note, and the group-integrity validation check.
- `SylviaNG.Recruitment.Tests/Validators/JobApplicationSubmitValidatorTests.cs` — NID max-length rule.

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/enums/recruitment.enum.ts` — `DuplicateDismissed` added to `ApplicationStatusEnum`.
- `@core/interfaces/recruitment-management/job-application.interface.ts` — `IJobApplicationDuplicateGroup`/`Item`/`ResolveRequest`.
- `@core/services/recruitment/job-application/job-application.service.ts` — `getDuplicates`, `resolveDuplicates`, `candidateNationalId` added to `applyOnBehalf`.
- `pages/application-tracking/duplicate-applications/` — new component (side-by-side comparison table per group, radio-select primary, confirm-and-resolve action).
- `pages/application-tracking/application-tracking-routing.module.ts` — `duplicates/:jobPostingId` route.
- `pages/application-tracking/application-tracking.module.ts` — declares the new component, adds `RadioButtonModule`.
- `pages/application-tracking/application-status-transitions.constants.ts` — `DuplicateDismissed` mapped to no manual transitions (dedicated-flow-only).
- `pages/application-tracking/ats-dashboard/ats-dashboard.component.ts`/`.html` — "View Duplicates" button, scoped to the selected vacancy.
- `pages/application-tracking/apply-on-behalf/apply-on-behalf.component.ts`/`.html` — National ID field.

## Verification

- `dotnet test` — 213/213 passing, including all new duplicate-detection and validator tests; no regressions.
- `dotnet ef migrations add` initially picked up a stale local `ApplicationDBContextModelSnapshot.cs` (an untracked/gitignored file left over from a different branch checked out earlier in the session) and proposed dropping unrelated tables/columns that aren't part of `dev`. Removed the migration, deleted the stale snapshot, and regenerated — the final migration only adds `CandidateNationalId`. Applied cleanly to the local dev Postgres database (`dotnet ef database update`), confirmed via the generated SQL (`ALTER TABLE "JobApplications" ADD "CandidateNationalId" ...` only).
- Frontend: `tsc --noEmit` and the Angular AOT compile both pass with zero errors. The production `ng build` fails on a bundle-size budget (1.41 MB vs 1 MB max) — confirmed via `git stash -u` + rebuild against the unmodified `dev` tree that this is pre-existing and identical in size, not caused by this change.
- Did **not** perform a logged-in browser/API walkthrough of the authenticated endpoints — that requires an HR/Admin Keycloak session this environment doesn't have credentials for, and obtaining one wasn't attempted. Recommended manual check before merge: submit two applications to the same open vacancy with different emails but the same phone (one External, one via Apply on Behalf), open the ATS Dashboard, select that vacancy, click "View Duplicates," confirm both appear side-by-side matched on phone, mark one primary and dismiss the other, and confirm the dismissed one shows `Duplicate Dismissed` in its status-history audit trail on the Application Detail page.
