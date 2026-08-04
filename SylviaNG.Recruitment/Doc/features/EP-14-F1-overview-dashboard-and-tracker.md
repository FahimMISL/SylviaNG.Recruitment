# EP-14 F1: Recruitment Overview Dashboard + Job Application Tracker Report

## Epic

EP-14: Reports, Analytics & Dashboards — real-time metrics, funnel/time-to-hire analytics, source/interview analytics on top of the recruitment data already captured across EP-01–EP-09/EP-17.

## User stories

- **US-105 — Recruitment Overview Dashboard** (Must Have, L): real-time metric cards across all active vacancies (open postings, applications, interviews, pending approvals, offers pending acceptance), with trend indicators, click-through navigation, auto-refresh, and admin-configurable widget visibility.
- **US-109 — Job Application Tracker Report** (Must Have, M): centralized tabular view of every application across all vacancies, with current stage, days-in-stage, staleness highlighting, inline status update, and Excel export.

## Why

Foundational data-aggregation layer other EP-14 features (F2 funnel/time-to-hire, F3 source/interview analytics) build queries against. Bundled into one branch/PR since both stories are Must Have and share the same underlying aggregation work.

## What already existed (reused, not rebuilt)

- `DashboardController`/`DashboardService`/`DashboardSummaryResponse` — already had 3 of US-105's metrics (open postings, total applications, active pipelines).
- `JobApplicationController.GetDashboardPaged` + Angular `AtsDashboardComponent` (US-035/050) — already the exact "centralized tabular view of every application" US-109 asks for. Extended in place rather than building a second page/route (confirmed with user during planning).
- `JobApplicationStageProgress`, `PipelineStage.SlaDays`, `OfferLetter.Status/DecisionAt`, `Interview.ScheduledStartAt/Status` — covered most of the new data needs, with gaps below.

## Real gaps found during implementation (not assumptions)

- **`JobApplicationStageProgress`/`Audit.CreatedAt`/`UpdatedAt` are never populated anywhere in this codebase** — no interceptor, no manual stamp. "Days in Current Stage" (US-109 AC1/AC2) required a new `StageEnteredAt` field, stamped on the `Pending/other → InProgress` transition (mirrors how `CompletedAt` is already stamped on `→ Completed`). "Last Updated" in the tracker uses `StageEnteredAt` too, not `UpdatedAt` (confirmed dead throughout).
- `PipelineStageId` on `JobApplicationStageProgress` is a soft reference (pipelines get edited in place) — `ManualApprovalRequired` and `SlaDays` are snapshotted onto the progress row (`RequiresManualApproval`, `SlaDaysSnapshot`) at provision time, same reasoning as the pre-existing `StageName`/`DisplayOrder` snapshot.
- PaginationExtensions' generic reflection-based sort only works on native `JobApplication` properties. Stage-derived tracker columns (Stage/DaysInCurrentStage/AssignedHR/IsStale) can't be reached that way, so sorting/filtering by those columns (or `StaleOnly`) routes through a new in-memory path in `JobApplicationService`, mirroring the existing candidate-attribute-filter in-memory path.

## Scope decisions (deviate from literal AC text — flagged for review)

1. **US-105 AC5** ("Admin can configure which widgets are shown… (EP-15)"): built a **minimal version now** — a `DashboardWidgetConfig` table with Admin-only per-role visibility toggles for the 5 widget keys, not full EP-15 RBAC (EP-15 isn't built yet). User-approved during planning.
2. **US-109 "Assigned HR"**: no HR-assignment field exists anywhere in the schema, and no story ever added one. Proxied via `JobApplicationStageProgress.LastUpdatedByUserName` on the application's current-stage row (whoever last touched it). User-approved during planning.
3. **US-105 AC2 trend indicators**: only computed for genuinely append-only counters (`TotalApplications` via `AppliedDate`). The 3 mutable point-in-time metrics (Open Vacancies, Pending Approvals, Offers Pending Acceptance) show a plain current count with **no** trend arrow — there's no history table to reconstruct "what was true a week ago" for mutable state, and a fabricated number would be actively misleading.
4. **"Pending Approvals" definition**: stage-progress rows where `Status == InProgress && RequiresManualApproval` (candidate currently sitting at a stage that needs manual approval to advance), not `Status == Pending`.
5. **Staleness threshold** (US-109 AC2): per-stage `PipelineStage.SlaDays` (snapshotted as `SlaDaysSnapshot`) when the stage has one configured, falling back to a new global `ApplicationSetting.DefaultStaleDaysThreshold` (nullable, unset by default) otherwise.
6. **`StageEnteredAt` backfill**: the migration stamps `UtcNow` on every currently-`InProgress` row at deploy time (existing in-flight applications start their "days in stage" counter at 0), rather than leaving it null — avoids the tracker looking broken for every pre-existing application on day one, especially since this gets demoed.
7. **Tracker export (AC5)**: a new `JobApplicationTrackerExport` type on the existing async `ExportRequest` queue (same precedent as `CandidateListExport`/`BulkCvZip` for a potentially large, row-per-application dataset), with its own column set (Vacancy/Candidate/Stage/Status/Last Updated/Days in Stage/Stale/Assigned HR) rather than extending the candidate-list export — the two serve different purposes.
8. **AC3 click-through** ("clicking a metric card navigates to the relevant filtered list"): implemented with real query-param filtering where the target page already has (or cheaply gained) a matching filter field — Open Vacancies → job vacancy list (unfiltered, that page has no status filter UI yet), Total Applications → ATS dashboard (unfiltered - it's the "all applications" view), Upcoming Interviews → interview list **filtered** `status=Scheduled` (added `ActivatedRoute` query-param read to `InterviewListComponent`), Pending Approvals → ATS dashboard (unfiltered - no such filter exists), Offers Pending Acceptance → offer letter list (unfiltered - that page only filters by `jobApplicationId` today). Filtering all 5 cards would need new filter capabilities on 2 pages that don't have any today - out of scope for this bundle.

## Key files

### Backend (`SylviaNG.Recruitment-master`)
- `Domain/Entities/JobApplicationStageProgress.cs` — `RequiresManualApproval`, `SlaDaysSnapshot`, `StageEnteredAt`
- `Domain/Entities/DashboardWidgetConfig.cs` (new), `ApplicationSetting.cs` — `DefaultStaleDaysThreshold`
- `Application/Mappings/PipelineProgressMapper.cs` — stamps new snapshot/transition fields
- `Application/Services/JobApplicationStageProgressService.cs` — `BulkAdvanceToStageAsync` stamps `StageEnteredAt`
- `Application/Services/DashboardService.cs`, `Features/Dashboard/Models/DashboardSummaryResponse.cs`, `DashboardWidgetKeys.cs`, `DashboardWidgetConfigModels.cs`
- `Application/Services/DashboardWidgetConfigService.cs` (new) + `Controllers/DashboardWidgetConfigController.cs` (new)
- `Application/Services/JobApplicationService.cs` — `GetDashboardPagedAsync`/`GetDashboardMatchingIdsAsync` extended with `AttachStageProgressInfoAsync`, stage-derived sort, `StaleOnly`
- `Application/Features/JobPostings/Models/JobApplicationDashboardResponse.cs`, `JobApplicationAttributeFilterRequest.cs` — tracker fields, `StaleOnly`
- `Infrastructure/Repositories/JobApplicationRepository.cs`, `JobApplicationStageProgressRepository.cs`, `InterviewRepository.cs`, `OfferLetterRepository.cs` — new count/lookup methods
- `Application/Services/ExportGenerationService.cs`, `ExportRequestService.cs` + `Application/Features/ExportRequests/Commands/ExportRequestCreateJobApplicationTracker/` (new) + `Controllers/ExportRequestController.cs`
- `Infrastructure/BackgroundServices/ExportRequestWorker.cs` — new export-type case
- `Domain/Enums/Enum.cs` — `ExportTypeEnum.JobApplicationTrackerExport`
- `Infrastructure/Configurations/DashboardWidgetConfigConfiguration.cs`, `Infrastructure/Data/ApplicationDBContext.cs`
- `Application/Extensions/DependencyInjection.cs`, `Infrastructure/Extensions/DependencyInjection.cs`
- `Migrations/20260729045809_AddDashboardMetricsAndTrackerFields.cs`
- `SylviaNG.Recruitment.Tests/Services/JobApplicationStageProgressServiceTests.cs`, `JobApplicationServiceTests.cs`, `ExportGenerationServiceTests.cs`, `DashboardWidgetConfigServiceTests.cs` (new)

### Frontend (`sylviang.adminui.recruitment-main`)
- `src/app/@core/interfaces/dashboard.interface.ts`, `src/app/@core/services/dashboard/dashboard.service.ts`
- `src/app/pages/dashboard/dashboard.component.ts/.html/.scss` — trend cards, 5-min auto-refresh + manual reload, click-through, Admin widget-visibility toggle table
- `src/app/@core/interfaces/recruitment-management/job-application.interface.ts` — tracker fields, `staleOnly`
- `src/app/pages/application-tracking/ats-dashboard/ats-dashboard.component.ts/.html/.scss` — new columns, stale-row highlight, `staleOnly` filter, deep-link query params, inline status-update dialog, "Export Tracker" button
- `src/app/pages/application-tracking/ats-dashboard/ats-dashboard.component.constants.ts`
- `src/app/@core/services/recruitment/export-request/export-request.service.ts`
- `src/app/pages/interview-management/interview-list/interview-list.component.ts` — reads `status` query param for the Upcoming Interviews deep link

## Verification

- Backend: `dotnet test` — 738/738 passing (28 new tests for stage-progress stamping, tracker attach/sort/stale-filter, tracker export, widget-config service).
- Frontend: `tsc --noEmit` clean, `ng build` clean.
- Live end-to-end (both servers running, seeded demo data): logged in as `abir`/HR and `admin`/Admin, confirmed via API + Playwright screenshots — dashboard shows all 5 metric cards with correct values, trend arrow on Total Applications (+50%), 5-min auto-refresh wired, Admin-only widget-visibility table renders/toggles and immediately changes what HR sees; ATS dashboard shows Stage/Last Updated/Days in Stage/Assigned HR columns populated correctly for an application with real stage-progress data, `staleOnly` filter and "Export Tracker" button present and functional (queued export reached `Completed` status with correct row count/filename via the async worker), inline status-update dialog opens pre-filled with candidate/current status.
