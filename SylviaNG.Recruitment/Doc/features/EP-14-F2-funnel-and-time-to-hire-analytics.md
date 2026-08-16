# EP-14 F2: Recruitment Funnel Visualization + Time-to-Hire Analytics

## Epic

EP-14: Reports, Analytics & Dashboards. F1 (Overview Dashboard + Job Application Tracker) shipped 2026-07-29 and built the data-aggregation groundwork (`ApplicationStatusHistory`, `JobApplicationStageProgress`) other EP-14 features query against. This is F2.

## User stories

- **US-106 — Recruitment Funnel Visualization** (Must Have, M): funnel chart of candidate counts per pipeline stage, filterable by Job Posting/Department/Date Range/Candidate Type, showing absolute numbers + conversion rates, hover breakdown of pass/drop-off reasons, exportable as PNG + CSV.
- **US-107 — Time-to-Hire Analytics** (Should Have, M): days from Job Posted to Offer Accepted, average/min/max across filtered vacancies, per-stage average time breakdown, filterable by Department/Date Range, exportable.

## Why

Both stories are read-only analytics reports over data already captured by earlier epics (EP-06 application status transitions, EP-10 offer letters). Bundled into one branch/PR since both query the same underlying audit trail (`ApplicationStatusHistory`) and share the same aggregation-service shape.

## What already existed (reused, not rebuilt)

- `ApplicationStatusHistory` (Domain/Entities) — append-only audit trail of every real status transition, stamped in `JobApplicationService.ApplyStatusChangeAsync`/`WithdrawApplicationAsync`. Already fully populated in production data — no new stamping logic needed, unlike F1's `StageEnteredAt` which had to be added because `Audit.CreatedAt/UpdatedAt` were dead fields.
- `ApplicationStatusReason` — existing reason lookup (`Label`, `AppliesToStatus`), already linked via `ApplicationStatusHistory.ReasonId`/`Reason` nav property. Backs US-106 AC4's drop-off reason breakdown directly, no new entity.
- `JobPosting.PostingDate`, `OfferLetter.Status`/`DecisionAt` — time-to-hire start/end points, already present.
- `CandidateProfile.IsInternal` (computed) — backs US-106 AC2's "Candidate Type" filter, same in-memory join pattern F1 used for skill-filtering (`ICandidateProfileRepository.GetByEmailsAsync`).
- `PaymentReportService`/`PaymentReportController` (EP-17) — architectural template: plain service+controller (no MediatR), filter-scoped aggregation, synchronous CSV/export endpoint. Used instead of the Dashboard's MediatR-wrapped fixed-summary pattern since both new stories are multi-parameter filtered reports.

## Real gaps found during implementation (not assumptions)

- **`ApplicationStatusHistory` does not reliably log the `Applied` status.** A history row is only written on an *explicit* transition (`ApplyStatusChangeAsync`); a freshly-submitted application sitting at its default `Applied` status gets zero history rows unless a waiver rule matched at submission (the one place `SubmitAsync` stamps an initial row). So "reached Applied" is computed as every in-scope application counting by definition (`ApplicationStatus` defaults to `Applied` at creation), not via a `ToStatus == Applied` history lookup like every other funnel stage. Flagged in code comments on `AnalyticsReportService`.
- **Chart.js canvas doesn't inherit a plain wrapping `<div>`'s CSS height.** `<p-chart>`'s host element needs explicit `[style.height]="'100%'"`/`[style.width]="'100%'"` plus `position: relative` on its parent, or the canvas renders at a much smaller backing size than the intended container — found live via Playwright screenshot (bars rendered at 11px tall instead of filling a 352px container).
- **Chart.js category-axis `ticks.autoSkip` defaults to `true` and silently drops labels (not bars) when 7 categories don't fit the available height/width.** The funnel chart visually looked like only 4 of 7 stages existed (labels "Applied/Shortlisted/Interviewed/Hired" only) even though all 7 bars were correctly rendered underneath — confirmed via `chart.getDatasetMeta(0)` inspection during live verification. Fixed with `ticks: { autoSkip: false }` on the category axis for both the funnel and stage-duration charts, since both have a small fixed category count that should always display in full.

## Scope decisions (deviate from literal AC text — flagged for review)

1. **Funnel stage vocabulary = real `ApplicationStatusEnum`, not AC1's literal 7-name list.** No "Assessed" status exists anywhere in the schema. `JobApplicationStageProgress.StageName` is free-text per-`HiringPipeline`, not comparable across vacancies — structurally wrong for a cross-vacancy funnel. Funnel stages: `Applied → Screening ("Screened") → Shortlisted → InterviewScheduled ("Interview Scheduled") → Interviewed → Offered → Hired`. `Rejected`/`Withdrawn`/`AwaitingPayment`/`DuplicateDismissed` are terminal, not funnel rows.
2. **Department filter kept as a raw numeric ID (no name lookup); Hiring Manager filter dropped entirely.** No local Department table exists anywhere (EP-16 Core HR integration is out of scope); `JobPosting.DepartmentId` is a raw external `long?`, already filtered this way elsewhere (`IJobPostingRepository.GetPaginatedByCircularTypesAsync`). `HiringManager` doesn't exist as a field on any entity at all — AC4's hiring-manager filter has nothing to filter by.
3. **CSV export is a synchronous endpoint, not the async `ExportRequest` queue.** Funnel/TTH CSVs are a handful of aggregate rows (7 funnel stages, up to 7 stage-duration rows, 4 summary numbers) — the async queue exists for large per-row exports (candidate lists, CV ZIPs, tracker rows) and would be overengineering here. Matches `PaymentReportController.ExportReconciliation`'s existing sync precedent.
4. **Chart library: added `chart.js` via PrimeNG's `p-chart` wrapper; funnel rendered as a horizontal bar chart (decreasing widths), not a literal funnel shape.** Neither PrimeNG nor Chart.js has a native funnel chart type. A bar chart gets Chart.js's tooltip system for free (AC4 hover drop-off breakdown) and `getBase64Image()` for free (AC5 PNG export) with zero extra dependencies.
5. **Time-to-hire counted per accepted offer, not per vacancy.** A `JobPosting` with `NumberOfPositions > 1` can produce multiple accepted offers; averaging per-vacancy would undercount real hiring activity. One time-to-hire sample per accepted offer; `VacancyCount` (distinct `JobPostingId`) reported separately for context.

## Key files

### Backend (`SylviaNG.Recruitment-master`)
- `Application/Interfaces/Repositories/IApplicationStatusHistoryRepository.cs` (new) + `Infrastructure/Repositories/ApplicationStatusHistoryRepository.cs` (new)
- `Application/Interfaces/Repositories/IJobApplicationRepository.cs`/`Infrastructure/Repositories/JobApplicationRepository.cs` — `GetForAnalyticsScopeAsync`
- `Application/Interfaces/Repositories/IOfferLetterRepository.cs`/`Infrastructure/Repositories/OfferLetterRepository.cs` — `GetAcceptedForApplicationsAsync`
- `Application/Features/Analytics/Models/` (new) — `RecruitmentFunnelRequest/Response.cs`, `TimeToHireRequest/Response.cs`
- `Application/Interfaces/Services/IAnalyticsReportService.cs` (new) + `Application/Services/AnalyticsReportService.cs` (new)
- `Controllers/AnalyticsReportController.cs` (new)
- `SharedKernel/Utils/CsvWriter.cs` (new) — extracted from `ExportGenerationService.cs`'s private `WriteCsv`/`EscapeCsvField`, now shared by both services
- `Application/Extensions/DependencyInjection.cs`, `Infrastructure/Extensions/DependencyInjection.cs`
- `SylviaNG.Recruitment.Tests/Services/AnalyticsReportServiceTests.cs` (new, 10 tests)

### Frontend (`sylviang.adminui.recruitment-main`)
- `package.json` — added `chart.js`
- `src/app/@core/interfaces/recruitment-management/analytics.interface.ts` (new)
- `src/app/@core/services/recruitment/analytics/analytics.service.ts` (new)
- `src/app/pages/analytics/analytics.module.ts`, `analytics-routing.module.ts` (new, lazy-loaded at `/analytics`)
- `src/app/pages/analytics/recruitment-analytics/recruitment-analytics.component.ts/.html/.scss` (new)
- `src/app/pages/pages-routing.module.ts` — new `analytics` lazy route
- `src/app/@core/constants/nav-menu-items.ts` — new "Recruitment Analytics" sub-item under Recruitment

## Verification

- Backend: `dotnet test` — 748/748 passing (10 new tests: funnel Applied-count-with-no-history gap, conversion % incl. zero-division guard, drop-off reason grouping, candidate-type filter, CSV export shape, TTH average/min/max, no-accepted-offers case, stage-duration deltas, TTH CSV shape).
- Frontend: `tsc --noEmit` clean, `ng build` clean (`analytics-module` lazy chunk generated).
- Live end-to-end (both servers running, seeded demo data, Playwright): logged in as `abir`/HR, navigated to `/analytics/recruitment` — funnel chart renders all 7 stages with correct counts/conversion % against real `ApplicationStatusHistory` data (24→13→7→3→3→2→1), hover tooltip shows drop-off reasons, PNG export downloads a real image, CSV export downloads correct rows (verified header + all 7 stage rows plus a real recorded drop-off reason "Not enough experience"); time-to-hire cards show correct average/min/max (4.1/4/5 days) against real accepted-offer data, CSV export downloads correct summary rows. Zero browser console errors. Found and fixed two real rendering bugs during this pass (chart canvas not filling its container height; Chart.js category-axis `autoSkip` silently hiding 3 of 7 funnel stage labels) — both documented above.
