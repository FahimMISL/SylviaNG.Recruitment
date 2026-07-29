# EP-14 F3: Candidate Source Analytics + Interview Analytics Export

## Epic

EP-14: Reports, Analytics & Dashboards. F1 (Overview Dashboard + Job Application Tracker) and F2 (Funnel + Time-to-Hire) shipped 2026-07-29. F3 is the third and final feature of the epic.

## User stories

- **US-108 — Candidate Source Analytics** (Should Have, S): breakdown of applications by source (Career Portal, BDJobs, LinkedIn, Internal, Employee Referral, Agency), filterable by Vacancy/Date Range/Employment Type, each segment showing total/shortlisted/hired/conversion rate, exportable to Excel.
- **US-110 — Interview Analytics Export** (Could Have, M): per-panelist stats (interviews conducted, average score, recommendation distribution), score histogram, filterable by Vacancy/Department/Date Range, exportable to Excel for committee review.

## Why

Both stories are read-only analytics reports reusing F2's `AnalyticsReportController`/`AnalyticsReportService` architecture and the same `/analytics/recruitment` page. Bundled into one branch/PR since both are additive report sections on that existing page.

## Real gaps found during implementation (not assumptions)

- **No single field covers US-108's named source channels.** `ApplicationSourceEnum` only has `External`/`Internal`/`Admin` (the application's submission channel), while "BDJobs", "LinkedIn", "Employee Referral", "Agency" etc. live in the existing admin-managed `ReferralSource` lookup (`JobApplication.ReferralSourceId`, nullable, declared at apply time for EP-17 fee-waiver matching). Resolved by grouping on `ReferralSource.Name` when set, falling back to a label derived from `ApplicationSourceEnum` when not (`External` → "Career Portal", `Internal` → "Internal", `Admin` → "Agency/Direct"). No schema change needed for US-108 itself.
- **US-110 AC1's "recommendation distribution" (Recommended/Not Recommended/On Hold) had no backing data at all.** `InterviewEvaluation` (EP-08 US-067) only ever captured `Score`+`OverallComments` — there was no panelist-intent signal to aggregate, and no threshold-based derivation exists (`Scorecard`/`ScorecardCriterion` weights are per-criterion, not a pass/fail cutoff). Added an explicit, optional `EvaluationRecommendationEnum? Recommendation` field to `InterviewEvaluation`, captured on the existing evaluate-interview submit/update flow — a real signal from the panelist (via HR), not a derived proxy. One additive nullable-column migration (`AddInterviewEvaluationRecommendation`); pre-existing evaluations have `null` and are excluded from the three recommendation counts but still counted in `InterviewsConducted`/`AverageScore`.
- **Excel, not CSV, for these two exports.** Both ACs explicitly say "Excel" (unlike F2's funnel/time-to-hire CSVs). Used ClosedXML — already a project dependency via `InterviewEvaluationService.ExportResultsExcelAsync` — to produce real `.xlsx` workbooks instead of stretching F2's `CsvWriter` precedent.

## Scope decisions

1. **Shortlisted/Hired counts use the application's current `ApplicationStatus`, not a full funnel-history walk.** US-108 AC3 only asks for a point-in-time snapshot per source, unlike F2's funnel which tracks "ever reached" per stage across history. Simpler and sufficient: `ShortlistedCount` = current status at or past `Shortlisted` in the existing `FunnelOrder`; `HiredCount` = current status exactly `Hired`.
2. **Interview Analytics score histogram is fixed at 5 bands (0-20/21-40/41-60/61-80/81-100) over `WeightedScore`**, matching AC2's "histogram" ask without introducing configurable bucketing — mirrors F2's "small fixed category count, never thin out" chart precedent (`autoSkip: false`).
3. **Panelist names resolved via `IEmployeeRepository.GetByIdAsync` per distinct `EmployeeId`** (a handful of panelists per scope) rather than adding a new bulk-lookup repository method — matches the existing single-lookup pattern already used in `InterviewService`/`InterviewRoundConfigService` for the same `Employee` entity.
4. **`WeightedScore` is recomputed in `AnalyticsReportService`, not reused from `InterviewEvaluationMapper`.** The mapper's `DeriveWeightedScore` operates on the already-mapped response DTO shape for a single evaluation; the analytics service aggregates across many evaluations from raw entities, so the same Σ(Score/MaxScore × Weight) / Σ(Weight) × 100 formula is duplicated at the entity level (documented in a code comment pointing back to the mapper as the source of truth).

## Key files

### Backend (`SylviaNG.Recruitment-master`)
- `Domain/Enums/Enum.cs` — `EvaluationRecommendationEnum`
- `Domain/Entities/InterviewEvaluation.cs` — `Recommendation` field
- `Migrations/20260729093856_AddInterviewEvaluationRecommendation.cs` (new)
- `Application/Features/InterviewEvaluations/Models/` — `InterviewEvaluationSubmitRequest.cs`, `InterviewEvaluationUpdateRequest.cs`, `InterviewEvaluationResponse.cs` — `Recommendation` field added
- `Application/Mappings/InterviewEvaluationMapper.cs`, `Application/Services/InterviewEvaluationService.cs` — wired `Recommendation` through submit/update/export
- `Infrastructure/Repositories/JobApplicationRepository.cs` — `GetForAnalyticsScopeAsync` now includes `ReferralSource`
- `Application/Interfaces/Repositories/IInterviewEvaluationRepository.cs`/`Infrastructure/Repositories/InterviewEvaluationRepository.cs` — new `GetForAnalyticsScopeAsync`
- `Application/Features/Analytics/Models/` (new) — `CandidateSourceAnalyticsRequest/Response.cs`, `InterviewAnalyticsRequest/Response.cs`
- `Application/Interfaces/Services/IAnalyticsReportService.cs` + `Application/Services/AnalyticsReportService.cs` — new `GetCandidateSourceAnalyticsAsync`/`ExportCandidateSourceAnalyticsExcelAsync`/`GetInterviewAnalyticsAsync`/`ExportInterviewAnalyticsExcelAsync`
- `Controllers/AnalyticsReportController.cs` — new `candidate-source`, `candidate-source/export`, `interview-performance`, `interview-performance/export` endpoints
- `SylviaNG.Recruitment.Tests/Services/AnalyticsReportServiceTests.cs` — 8 new tests

### Frontend (`sylviang.adminui.recruitment-main`)
- `src/app/@core/enums/recruitment.enum.ts` — `EvaluationRecommendationEnum`
- `src/app/@core/interfaces/recruitment-management/analytics.interface.ts` — new request/response interfaces
- `src/app/@core/interfaces/recruitment-management/interview-evaluation.interface.ts` — `recommendation` field
- `src/app/@core/services/recruitment/analytics/analytics.service.ts` — new endpoint methods
- `src/app/pages/analytics/recruitment-analytics/recruitment-analytics.component.ts/.html` — two new report sections (Candidate Source pie chart + table, Interview Analytics panelist table + histogram chart), employment-type filter
- `src/app/pages/interview-management/evaluate-interview/evaluate-interview.component.ts/.html` — Recommendation dropdown
- `src/app/pages/interview-management/interview-results/interview-results.component.html` — Recommendation badge

## Verification

- Backend: `dotnet test` — 754/754 passing (8 new tests: source grouping by `ReferralSource.Name` with fallback labels, shortlisted/hired/conversion computation, employment-type filter, per-panelist average/recommendation-count aggregation, missing-employee-record fallback name, 5-band score histogram bucketing).
- Frontend: `tsc --noEmit` clean, `ng build` clean.
- Live end-to-end (Docker Postgres + local Keycloak + both servers, logged in as `abir`/HR via Playwright): `/analytics/recruitment` renders both new sections against real seeded data — Candidate Source pie chart + table (Career Portal 15/5/1/6.7%, Agency/Direct 6/0/0/0%, Internal 3/1/0/0%); Interview Analytics panelist table + histogram. Updated an existing evaluation via the API to add a `Recommended` value and submitted a brand-new evaluation through the UI with an `OnHold` value (scores 9/8, weighted 86%) — both flowed through correctly: `interview-performance` now reports `interviewsConducted: 2`, `averageScore: 81`, `recommendedCount: 1`, `onHoldCount: 1`, histogram correctly bucketed 76→61-80 and 86→81-100. The `OnHold` badge and 76%/86% weighted scores render correctly on the interview-results page. Both Excel exports downloaded as valid `.xlsx` workbooks (verified by reading them back with the `xlsx` library) with data matching the on-screen numbers exactly. Zero browser console errors throughout.

## Unrelated environment note (not part of this feature)

During verification, the local `sylviang-keycloak-dev` Docker container failed to start with a Windows port-bind permission error — a pre-existing Windows/Hyper-V dynamic port exclusion (`netsh interface ipv4 show excludedportrange`) covering 8060-8159 that blocks anything from binding `0.0.0.0:8082`, unrelated to this branch. Fixed by snapshotting the container (`docker commit`, preserving the realm/user data) and re-running it on host port `18082` instead; `appsettings.Development.json`'s `Keycloak:Authority` (gitignored, local-only) and `start.bat` (outside both repos, not shared) were updated to match. The real login flow was re-verified against Keycloak on the new port before continuing.
