# US-005 — Internal Candidate Profile Pre-Population

## What

An internal employee's candidate profile is now automatically populated on first login from Core HR's synced `Employee` record: name, phone, department, and designation, matched by email. The profile is visibly tagged "Internal Candidate" (vs "External Candidate") everywhere it's shown, department/designation display read-only, and any later edit to the pre-populated Name/Phone is flagged to HR on the candidate detail view. The internal job board's apply form now pre-fills from the candidate's own profile instead of a blank manual-entry form, and internal applications require a PDF resume specifically (external still accepts PDF/DOC/DOCX).

## Why

US-005 (Must, M) was the last Must-Have gap in EP-01 — internal candidates had zero parity with external candidates despite the org already holding their data in Core HR. Investigation found real, partially-built infrastructure for this that no candidate-facing feature was using yet: an `Employee` entity synced via Kafka from Core HR, and a Core gRPC client (`ICoreGrpcClient`) with a batch master-data lookup that had exactly one implemented method (`GetSitesAsync`) and zero callers anywhere. The internal job board's apply form (`internal-apply-form.component.ts`) was a completely blank manual-entry form — no different from the external career-portal form — and `CandidateProfile` had no notion of "internal" at all.

## Design notes

- **Matched by email, not a Keycloak claim.** There's no employee-id claim in the Keycloak token (confirmed via `AuthenticationExtensions.cs` — only `realm_access` roles get copied) and no other identity bridge exists. `Employee` gained an `Email`/`Phone` column, captured from the Kafka employee-sync payload (previously ignored, matching the same defensive `TryGetProperty` style as the rest of `EmployeeEventConsumer`), and matching happens by comparing it to the JWT email claim — the same "soft join by email" pattern already documented in the ADR for `JobApplication`↔`CandidateProfile`.
- **Pre-population runs once, at first provisioning only.** `CurrentCandidateService.GetOrCreateCurrentProfileAsync`'s "no existing profile" branch is the only place this runs — never on subsequent logins, so it can't clobber a candidate's own later edits, and a candidate who logs in before their `Employee` row has synced via Kafka just gets a plain profile (identical to today's external flow) rather than an error.
- **`EmployeeId` presence *is* the internal/external distinction (AC4)** — no separate flag needed. `CandidateProfileResponse.IsInternal` is just `EmployeeId.HasValue`.
- **Department/Designation are read-only**, not editable form fields — deliberate scope decision (confirmed with the user) since they're Core-HR-owned org data, not something a candidate should freely retype. This keeps AC2's "edits are flagged" scope to the fields that already have editable inputs today: Full Name and Phone.
- **Drift detection via an immutable snapshot, not a mutable flag.** `PrepopulatedFullName`/`PrepopulatedPhone` are set once at provisioning and never touched again; `HasPrepopulatedFieldEdits` is computed on every read by comparing the live value to the snapshot (`CandidateProfileMapper.HasPrepopulatedFieldEdits`) — same "snapshot once, compare later" shape as US-003's identity-field lock, and can't go stale the way a boolean flag flipped imperatively could.
- **Department/Designation names are resolved via gRPC, best-effort.** `CandidateProfileService.ResolveOrgNamesAsync` wraps the new `ICoreGrpcClient.GetDepartmentsAndDesignationsAsync` call in try/catch — if the Core service is unreachable, the profile still loads fine with the names just null, matching the ADR's documented "best-effort side-effect" tolerance (resume text extraction, apply-on-behalf events) rather than letting an unrelated service outage break profile loading.
- **PDF-only for internal is a validator rule, not a new code path.** `JobApplicationSubmitValidator` already required *a* resume of an allowed type for everyone; the new rule narrows it to `.pdf` specifically `.When(x => x.Source == ApplicationSourceEnum.Internal)` — external stays unchanged.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Domain/Entities/Employee.cs` — new `Email`, `Phone`.
- `Domain/Entities/CandidateProfile.cs` — new `EmployeeId`, `DepartmentId`, `DesignationId`, `PrepopulatedFullName`, `PrepopulatedPhone`.
- `Infrastructure/Configurations/EmployeeConfiguration.cs`, `CandidateProfileConfiguration.cs` — column lengths/index for the new fields.
- `Infrastructure/Kafka/EmployeeEventConsumer.cs` — captures `email`/`phone` from the employee-sync payload.
- `Application/Interfaces/Repositories/IEmployeeRepository.cs` + `Infrastructure/Repositories/EmployeeRepository.cs` — new, `GetByEmailAsync`.
- `Application/Interfaces/Externals/ICoreGrpcClient.cs` + `Infrastructure/Services/CoreGrpcClient.cs` + `Application/Common/Models/CoreGrpcModels.cs` — new `GetDepartmentsAndDesignationsAsync`, mirroring the existing `GetSitesAsync`.
- `Application/Services/CurrentCandidateService.cs` — first-login Core HR match/pre-populate.
- `Application/Services/CandidateProfileService.cs` — `ResolveOrgNamesAsync`, wired into `GetMyProfileAsync`/`GetProfileDetailAsync`.
- `Application/Mappings/CandidateProfileMapper.cs` — `IsInternal`, `HasPrepopulatedFieldEdits`.
- `CandidateProfileResponse.cs`, `CandidateProfileDetailResponse.cs` — new fields.
- `Application/Features/JobPostings/Commands/JobApplicationSubmit/JobApplicationSubmitValidator.cs` — PDF-required-for-internal rule.
- Migration `20260716100304_AddInternalCandidatePrepopulation`.
- `SylviaNG.Recruitment.Tests/Services/CurrentCandidateServiceTests.cs` (new), `CandidateProfileServiceTests.cs`, `Validators/JobApplicationSubmitValidatorTests.cs`.

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/interfaces/recruitment-management/candidate-profile.interface.ts` — `isInternal`, `departmentName`, `designationName`, `hasPrepopulatedFieldEdits`.
- `pages/candidate-profile-management/my-profile/my-profile.component.html`/`.scss` — Internal/External badge, department/designation display, edit-flag notice.
- `pages/internal-job-board/internal-apply-form/internal-apply-form.component.ts`/`.html` — pre-fills from `CandidateProfileService.getMyProfile()`; resume input restricted to PDF only.
- `pages/internal-job-board/internal-job-board.constants.ts` — `RESUME_ALLOWED_EXTENSIONS` narrowed to `['.pdf']` (internal-only constant, external career-portal form unaffected).
- `pages/candidate-management/candidate-detail/candidate-detail.component.html`/`.scss` — same badge/flag for HR's view.

## Out of scope (flagged, not fixed)

- `candidate-list.component` (the HR candidate table) doesn't show the Internal/External badge — it's backed by `CandidateProfileSummaryResponse`, which doesn't carry `IsInternal`. Adding it there would mean widening the summary DTO and its mapper for a nice-to-have on a list view; the detail view (where AC4's "clear distinction" actually matters for a given candidate) already has it.
- Department/Designation editable-with-drift-tracking (as opposed to read-only display) was considered and explicitly declined — see Design notes.

## Verification

- `dotnet test` — 214/214 passing (10 new: `CurrentCandidateServiceTests` x3, `CandidateProfileServiceTests` x4, `JobApplicationSubmitValidatorTests` x3).
- `ng build` (development config) — compiles clean.
- Logical walkthrough: an `Employee` row with a matching email provisions a profile with `IsInternal=true`, Department/Designation resolved read-only, Name/Phone pre-filled; editing Name afterward flips `HasPrepopulatedFieldEdits`; no Employee match behaves exactly like an external candidate does today; internal apply rejects a non-PDF resume with a clear message and pre-fills from the profile; Core gRPC unreachable degrades to null department/designation names rather than failing profile load.
