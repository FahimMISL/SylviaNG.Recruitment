# US-003 — Update Profile Without Losing Application Data

## What

Once a candidate has at least one submitted job application, their `Email`, `Phone`, and `National ID` fields become read-only on the My Profile self-service form, with a lock icon and tooltip explaining why. All other profile fields (name, DOB, address, education, work experience, skills, etc.) remain freely editable at any time, and every save still shows the existing "Saved." confirmation. The lock is enforced server-side (400 on an attempted change) as well as reflected in the UI.

## Why

`JobApplication` intentionally has no FK to `CandidateProfile` — it stores its own immutable snapshot (`CandidateName`, `CandidateEmail`, `CandidatePhone`, ...) captured once at submission time, which already satisfies "past application data isn't retroactively changed" (AC1/AC2) for that snapshot itself. But the candidate's *own* self-service views (`GetMyApplicationsAsync`, `WithdrawMyApplicationAsync`) don't use that snapshot — they re-resolve the candidate's *current* profile email on every request and match it against `JobApplication.CandidateEmail`. Before this change, `CandidateProfileService.UpdateContactAsync` let a candidate freely change their email with no restriction, which meant: change your email once, and every application you'd already submitted instantly disappeared from "My Applications" and could no longer be withdrawn — a genuine data-loss bug matching the story title exactly, found during codebase investigation (not previously flagged in any ticket).

Locking the three identity-correlation fields once an application exists prevents this at the source, needs no schema migration, and is a small, contained change (Must Have, Size S).

## Design notes

- **Lock condition, not a stored flag.** "Has the candidate applied" is computed on demand via the existing `IJobApplicationRepository.GetByCandidateEmailAsync(entity.Email)` (already used by `GetProfileDetailAsync`) rather than a new persisted column — cheaper and always correct even if applications are added/withdrawn later.
- **Enforced in the service layer, not FluentValidation.** The check needs a DB read against the *current* entity state, which FluentValidation's synchronous per-request validators don't have access to; it lives in `CandidateProfileService` next to `GetCurrentProfileEntityAsync`, following the same pattern `JobApplicationService.ApplyStatusChangeAsync` already uses for business-rule validation (throwing `FluentValidation.ValidationException` directly from a service method).
- **Field-by-field, not whole-form.** Only a field that actually changed value triggers the check (compared to the entity's current value; phone via digit-only normalization so re-formatting the same number isn't treated as a change) — every other field on the same request still saves normally.
- **UI mirrors the same rule proactively.** `CandidateProfileResponse.HasSubmittedApplication` is computed once per `GetMyProfileAsync` call and drives `disable()`/`enable()` on the Email/Phone/NationalId controls, so the candidate sees the fields as read-only before attempting a save, not just after a rejected one. `getRawValue()` (already used by both section components) still includes disabled-control values on save, so an unlocked candidate's later save isn't affected.
- **Resume-parse prefill respects the lock.** `ContactSectionComponent.applyPrefill` (invoked when a candidate uploads a resume for AI parsing) now no-ops for a locked profile — otherwise `patchValue` would silently write a new email/phone into a disabled-looking field, and the eventual Save would fail with a confusing validation error.
- **Out of scope, flagged not fixed:** `JobApplicationService.GetAttributeFilteredApplicationsAsync` (ATS candidate-attribute filter, US-050) also joins live `CandidateProfile` by email — that's an intentional live-search feature (Recruitment_Features.md: "Boolean search across all candidate profile fields"), not a snapshot-integrity bug, so it's untouched. Locking identity fields post-application incidentally hardens this path too, since the email can no longer drift.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Application/Services/CandidateProfileService.cs` — `EnsureFieldNotLockedIfChangedAsync`/`HasSubmittedApplicationAsync`/`NormalizePhoneDigits` helpers; called from `UpdatePersonalInfoAsync` (NationalId) and `UpdateContactAsync` (Email, Phone); `GetMyProfileAsync` now sets `HasSubmittedApplication` on the response.
- `Application/Features/CandidateProfiles/Models/CandidateProfileResponse.cs` — new `HasSubmittedApplication` bool.
- `SylviaNG.Recruitment.Tests/Services/CandidateProfileServiceTests.cs` — lock-allowed/lock-blocked cases for Email, Phone (incl. reformatted-but-unchanged), NationalId, and the `HasSubmittedApplication` flag.

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/interfaces/recruitment-management/candidate-profile.interface.ts` — new `hasSubmittedApplication` on `ICandidateProfileResponse`.
- `pages/candidate-profile-management/my-profile/sections/contact-section/contact-section.component.ts` / `.html` — disables Email/Phone + lock tooltip when locked; `applyPrefill` skips when locked.
- `pages/candidate-profile-management/my-profile/sections/personal-info-section/personal-info-section.component.ts` / `.html` — disables NationalId + lock tooltip when locked.

## Verification

- `dotnet test` — 210/210 passing (11 new/updated in `CandidateProfileServiceTests`, no regressions elsewhere).
- `ng build` (development config) — compiles clean, including both edited templates (`pTooltip` already available via `SharedModule`, no new module import needed).
- Manual/logical walkthrough: a profile with zero applications can change Email/Phone/NationalId freely; once one application exists, changing any of the three (via the API) returns a 400 naming the field, while address/name/education/etc. saves still succeed; the UI renders those three fields disabled with a lock icon once `hasSubmittedApplication` is true.
