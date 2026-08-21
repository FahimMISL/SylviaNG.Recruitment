# US-024 — Check Eligibility Before Applying

## What

Real-time eligibility feedback on the job detail page: a logged-in Candidate sees whether they meet
a job posting's own eligibility criteria (minimum age, maximum age, minimum education level, minimum
experience, required district) before applying, and can still apply anyway after acknowledging a
re-shown warning at the confirmation step.

## Why

AC1 (a static eligibility summary listing the job's requirements) was already scaffolded during the
original EP-03 career-portal/internal-job-board UI commit, but it never checked the requirements
against anything — it just echoed the job posting's own fields back to the reader. AC2–AC4 close
that gap: a logged-in candidate should be told, specifically, whether *they* qualify, so they don't
waste time applying to roles they clearly don't meet (while still being free to apply — the check is
informational, not a hard block, per AC4).

## Design notes

- **Reused, not duplicated.** `CandidateFactService.BuildFacts` (already shared by the ATS dashboard's
  candidate-attribute filter, US-050) derives age/experience/education/address from a `CandidateProfile`
  — the new `JobEligibilityEvaluator` (`Application/Services/JobEligibilityEvaluator.cs`) reuses those
  facts and compares them against one job posting's own MinAge/MaxAge/MinEducationLevel/
  MinExperienceYears/RequiredDistrict fields, using the same comparison semantics as
  `JobApplicationService.MatchesAttributeFilter` (an unknown candidate age fails an age requirement).
  RequiredDistrict is matched as a case-insensitive substring of the candidate's address text, same
  approach as the existing ad-hoc `Location` filter — there's no discrete District field on
  `CandidateProfile`.
- **Candidate-only, not anonymous.** The new endpoint lives on `JobApplicationController`
  (`[Authorize(Roles = "Candidate")]`, same pattern as `my-applications`/`withdraw`), not on the
  `[AllowAnonymous]` `CareerPortalController` — `AllowAnonymous` at the controller level would have
  skipped authorization for every action under it, so a real "is this candidate logged in" check
  needed its own controller. The frontend only calls it when `AuthService.isAuthenticated()` and the
  role is `Candidate`; anonymous visitors and internal/HR users never see it and the static AC1
  summary is unaffected.
- **Informational, not a hard gate (AC4).** The apply forms re-show the unmet-requirements warning
  plus a required "I understand ... and wish to apply anyway" checkbox that gates the Submit button —
  the candidate can always proceed, they just can't miss the warning.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Application/Services/JobEligibilityEvaluator.cs` — new, pure comparison of a `JobPosting`'s criteria against `CandidateFactService.CandidateFacts`.
- `Application/Features/JobPostings/Models/JobEligibilityResponse.cs` — new `{ IsEligible, UnmetRequirements }`.
- `Application/Services/JobApplicationService.cs` / `Application/Interfaces/Services/IJobApplicationService.cs` — new `CheckEligibilityAsync(long jobPostingId)`.
- `Controllers/JobApplicationController.cs` — new `GET job-posting/{jobPostingId}/eligibility`, `[Authorize(Roles = "Candidate")]`.
- `SylviaNG.Recruitment.Tests/Services/JobEligibilityEvaluatorTests.cs` — new, per-criterion + combined-requirements coverage.
- `SylviaNG.Recruitment.Tests/Services/JobApplicationServiceTests.cs` — added `CheckEligibilityAsync` cases (not-found job posting, eligible, ineligible).

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/interfaces/recruitment-management/career-portal.interface.ts` — new `IJobEligibilityResponse`.
- `@core/services/recruitment/job-application/job-application.service.ts` — new `checkEligibility(jobPostingId)`.
- `career-portal/job-detail/job-detail.component.ts/.html` and `pages/internal-job-board/internal-job-detail/internal-job-detail.component.ts/.html` — call `checkEligibility` for logged-in Candidates after loading the posting; render an eligible (green) or ineligible (amber, with reasons) banner alongside the existing static summary.
- `career-portal/apply-form/apply-form.component.ts/.html` and `pages/internal-job-board/internal-apply-form/internal-apply-form.component.ts/.html` — `@Input() eligibilityResult`; re-show the warning + a required acknowledgement checkbox that gates Submit when ineligible (AC4).

## Verification

- `dotnet test` — 218/218 passing (52 in the two new/updated test files, no regressions).
- `npx tsc --noEmit` (frontend) — compiles clean.
- End-to-end against local Docker Postgres/Keycloak + `dotnet run` (port 5208) + `ng serve` (port 4600),
  driven with Playwright + verified via curl at the API layer:
  - Anonymous visit to a job with eligibility criteria set: static AC1 summary shown, no banner, no
    acknowledgement checkbox (screenshot confirmed).
  - Logged in as `sadia` (Candidate) against a job posting whose criteria her profile meets: green
    "You are eligible to apply for this position." banner (screenshot confirmed); API response
    `{isEligible:true, unmetRequirements:[]}`.
  - Same job posting with MinAge 30 / MinExperienceYears 5 (sadia's profile doesn't meet these): amber
    banner listing the specific unmet requirements; API response
    `{isEligible:false, unmetRequirements:["Minimum age 30 years","Minimum experience: 5 year(s)"]}`;
    apply form re-shows the warning, Submit is disabled until the acknowledgement checkbox is checked,
    then enables (screenshot confirmed).
  - Anonymous request to the new endpoint returns 401 (role-gated correctly).
  - Test job posting's eligibility fields reset to their original `null` values afterward — no leftover
    DB state.
