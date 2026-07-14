# US-050 — Filter Candidates by Job-Specific Criteria

## What

A live, ad-hoc filter panel on the ATS dashboard, scoped to a single selected vacancy, that narrows the visible applicant list by Education Level, Experience range, Skills, Location, Age range, and Source. Filters combine (all specified fields must match; Skills matches on ANY of the selected skills), apply in real time as the HR user changes them (no manual "Apply" click needed), show as removable chips, and persist for the browser session (sessionStorage) so they survive navigating away and back.

## Why

EP-06's prior stories (US-043/044/047) gave HR a *saved, named* filter that bulk-transitions matching candidates to Shortlisted status — useful for automation, but not for browsing. HR reviewing a vacancy still had no way to narrow the applicant list itself by candidate attributes (e.g. "show me only Bachelor's-and-above candidates with React experience in Dhaka") without opening each application individually. US-050 (Must Have, size M) closes that gap with a lightweight, non-destructive, per-vacancy browsing filter — distinct from the shortlist-filter feature, which still exists unchanged for its own purpose.

## Design notes

- **Scoped to one vacancy.** Candidate-attribute filters (Education/Experience/Skills/Location/Age) require a `JobPostingId` to be selected — enforced server-side (`JobApplicationService.ValidateAttributeFilterRequest`) and mirrored in the UI (the filter row only renders once a vacancy is picked). This keeps the underlying in-memory candidate-profile join bounded to a single vacancy's applications, the same tradeoff `ShortlistFilterEvaluationService` already makes for US-043/044.
- **Reused, not duplicated.** The candidate-fact derivation (age from DOB, total experience summed from work-experience rows, skill/education sets, address text) was extracted from `ShortlistFilterEvaluationService` into `CandidateFactService` (`Application/Services/CandidateFactService.cs`) so both features share one source of truth for these calculations.
- **Two query paths.** `JobApplicationService.GetDashboardPagedAsync`/`GetDashboardMatchingIdsAsync` keep the original pure-SQL path (unchanged, no regression) when no candidate-attribute filter is set, and fall to an in-memory join+filter+paginate path only when one is.
- **Skills = ANY, Education/Experience = minimum threshold.** Skills intentionally uses OR semantics (matches if the candidate has at least one selected skill) — a deliberate UX choice distinct from the existing saved-filter feature's ALL/AND semantics for `RequiredSkills`, which is unchanged.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Application/Services/CandidateFactService.cs` — new, extracted candidate-fact derivation shared by both filter features.
- `Application/Services/ShortlistFilterEvaluationService.cs` — updated to call the extracted service instead of its own private copy.
- `Application/Features/JobPostings/Models/JobApplicationAttributeFilterRequest.cs` — new flat filter DTO (scalar dashboard filters + the six new candidate-attribute fields).
- `Application/Services/JobApplicationService.cs` — `GetDashboardPagedAsync`/`GetDashboardMatchingIdsAsync` take the new filter object; added `MatchesAttributeFilter` (public, testable predicate), `ValidateAttributeFilterRequest`, `GetAttributeFilteredApplicationsAsync`.
- `Application/Interfaces/Services/IJobApplicationService.cs`, `Application/Interfaces/Repositories/IJobApplicationRepository.cs` — signature updates.
- `Infrastructure/Repositories/JobApplicationRepository.cs` — new `GetAllByJobPostingAndScalarFiltersAsync`.
- `Controllers/JobApplicationController.cs` — `GetDashboardPaged`/`GetDashboardMatchingIds` bind the new filter DTO instead of five loose query params.
- `SylviaNG.Recruitment.Tests/Services/JobApplicationServiceTests.cs` — unit tests for `MatchesAttributeFilter` (per-field + combined), the `JobPostingId`-required validator, and the fast-path/attribute-join dashboard flows.

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/interfaces/recruitment-management/job-application.interface.ts` — new `IAtsDashboardFilterParams`.
- `pages/application-tracking/application-tracking.module.ts` — added `InputNumberModule`, `MultiSelectModule`, `ChipModule`.
- `pages/application-tracking/ats-dashboard/ats-dashboard.component.ts` — new filter fields, debounced real-time apply (`filterChange$` + `debounceTime(400)`), `activeFilterChips`/`removeFilterChip`, sessionStorage persistence (`ats-dashboard-filters` key).
- `pages/application-tracking/ats-dashboard/ats-dashboard.component.html` / `.scss` — new candidate-attribute filter row (gated on a vacancy being selected) and active-filter-chips row.

## Verification

- `dotnet test` — 204/204 passing (including new US-050 tests; no regression to US-043/044's shared `CandidateFactService` logic).
- `ng build` (development config) — compiles clean. Production build hits a pre-existing 1MB initial-bundle budget failure unrelated to this change (confirmed via `git stash` + rebuild on the pre-change tree — identical failure).
- End-to-end via Playwright against local Docker Postgres/Keycloak + `dotnet run` + `ng serve`: seeded 4 candidates with distinct education/experience/skills/location/age on one vacancy, logged in as HR, and confirmed live: education-level, skills (ANY), location, and age-range filters each narrow the list correctly with no manual "Apply" click; chips appear per active filter and removing one reverts that field; filter state survives a full page reload (session persistence); Reset clears everything. Seed data removed afterward — no leftover DB state.
