# EP-13 Feature 1 — Candidate List Export + Async Export Request Queue (US-100, US-104)

## What

A generalized async export-request queue (US-104: `ExportRequest` table, `ExportRequestWorker` background service, retention sweep, EP-09 notification hook) with its first consumer, a filtered candidate-list export (US-100: one row per matched application, Excel or CSV, sourced from the exact same filter the ATS Dashboard already uses). HR/Admin queues an export from the ATS Dashboard filter bar, gets notified when it's ready (in-app bell + email leg), and downloads it from a new "Export Requests" list page.

## Why

First of 5 EP-13 stories, next epic in the approved roadmap after EP-12. Bundled 100+104 because 104's queue is the async engine 100 (and any future large export) runs on — building 100 as a synchronous download first and retrofitting a queue under it later would mean redoing the download endpoint twice.

Branched off `demo/local-showcase`, not `dev` — confirmed via `git ls-tree -r origin/dev` on both repos that dev has zero notification/export/download/admit-card infrastructure (empty match for `notif|export|cvbank|download|zip|admit`), the same recurring stack-gap every EP-09-and-later feature has hit. PR-to-dev link given after push regardless, per standing convention.

## Design decisions

- **Reuses `JobApplicationService.GetDashboardMatchingIdsAsync(JobApplicationAttributeFilterRequest filter)`** as the export's entire filter engine — the exact same filter DTO and validated matching-ids path the ATS Dashboard and bulk-shortlist actions already use (status/source/date range + education/experience/skills/location/age/tags). The export endpoint takes this DTO directly, no new filter shape.
- **Filtering happens synchronously at request time, not in the worker.** `ExportRequestService.RequestCandidateListExportAsync` calls `GetDashboardMatchingIdsAsync` inline (cheap, indexed, already-tested), stores the resulting `List<long>` as `ExportRequest.JobApplicationIdsJson`, and only defers the expensive part — hydrating full candidate/application data and writing the workbook — to `ExportRequestWorker`. This also means a bad filter (e.g. candidate-attribute filters without `JobPostingId`) fails fast with a 400 at request time instead of silently queuing and failing later, and a request always exports exactly what matched when it was queued (no re-run-the-filter race against later data changes).
- **`ExportRequest.Content` is a Postgres `bytea` column, not a file path.** No blob storage exists in this codebase yet (the MinIO swap is separately deferred), and every prior bulk-file feature (CV Bank zip/excel, admit card zip) generates in-memory and streams back synchronously with no persistence. Since this feature's whole point is deferring generation, the bytes have to land somewhere until downloaded — a column is the simplest option needing no new infra.
- **Polling `BackgroundService`, not a message queue.** Kafka exists in this codebase but is fully disabled (`AddHostedService<EmployeeEventConsumer>()` is commented out — no broker reachable). `EmployeeEventConsumer` is still the right shape to copy for a singleton `BackgroundService` needing scoped DB access (`_serviceProvider.CreateScope()` per tick) — `ExportRequestWorker` polls for `Pending` rows every 5s, processes oldest-first (batch of 5), and does the retention sweep (delete `Completed`/`Failed` rows past `ExpiresAt`, 7-day default) on the same tick. One worker, not two, since both are just "look at the table periodically."
- **Notification reuses the existing EP-09 pipeline.** New `RecruitmentEventEnum.ExportRequestReady`/`ExportRequestFailed`, dispatched via `INotificationDispatchService.DispatchAsync` with `AdminHrEmail` set to the requester's email. This writes a `NotificationLog` row and surfaces in the existing bell (`GetUnreadForAdminHrAsync` is scoped to recipient-type `AdminHr` globally, not per-address — same shared-inbox convention every other AdminHr notification already uses). No new bell/polling UI needed.
- **`ICurrentUserService` gained `GetCurrentUserEmail()`** — it previously only exposed `GetCurrentUserName()` (preferred_username). New method follows the exact `ClaimTypes.Email`/`"email"` claim fallback pattern `CurrentCandidateService`/`AccountSettingsService` already use.
- **Excel/CSV generation copies `CvBankCvBulkExportExcelHandler`'s shape** (fixed header row, one row per candidate, child collections flattened to delimited strings) but sourced from `JobApplication` rows joined to `CandidateProfile` via `ICandidateProfileRepository.GetByIdsWithDetailsAsync` — candidate facts prefer the linked profile (richer: gender, DOB, education, skills) and fall back to the `JobApplication`'s own snapshot fields (`CandidateName`/`Email`/`Phone`) for guest applicants with no profile yet. CSV is a plain manually-escaped delimited write (no new package — ClosedXML only handles xlsx).
- **No new permission scoping** — `[Authorize(Roles = "Admin,HR")]` on the new controller, matching `CvBankController`/the ATS dashboard action.

## A real bug found live (and fixed)

`ExportGenerationService`'s first cut sorted with `.OrderBy(a => a.CandidateName, StringComparer.OrdinalIgnoreCase)` **before** materializing the query — EF Core can't translate a custom `IComparer` into SQL, so every export failed with `The LINQ expression ... could not be translated`. The mocked-queryable unit tests didn't catch it (an in-memory `IQueryable` from `List<T>.AsQueryable()` has no translation step to fail), but the live curl verification did (`ExportRequestId 1` came back `Failed` with the translation error). Fixed by moving `.ToList()` before the `OrderBy` — sorting in-memory after materialization, the same safe order `CvBankCvBulkExportExcelHandler`'s equivalent sort already uses. `ExportRequestId 2` (post-fix) completed cleanly with the same filter.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Domain/Entities/ExportRequest.cs` — new.
- `Domain/Enums/Enum.cs` — `ExportTypeEnum`, `ExportFormatEnum`, `ExportRequestStatusEnum` new; `RecruitmentEventEnum.ExportRequestReady`/`ExportRequestFailed` appended.
- `Infrastructure/Configurations/ExportRequestConfiguration.cs` — new. `Infrastructure/Data/ApplicationDBContext.cs` — new `DbSet<ExportRequest>`.
- `Migrations/20260727103929_AddExportRequest.cs` — new migration.
- `Application/Interfaces/Repositories/IExportRequestRepository.cs` + `Infrastructure/Repositories/ExportRequestRepository.cs` — new.
- `Application/Interfaces/Services/{IExportRequestService,IExportGenerationService}.cs` + `Application/Services/{ExportRequestService,ExportGenerationService}.cs` — new. `Application/Services/CurrentUserService.cs`/`Application/Interfaces/Services/ICurrentUserService.cs` — `GetCurrentUserEmail()` added.
- `Application/Features/ExportRequests/**` — CQRS: Create/GetPaged/Download, full new vertical. `Application/Mappings/ExportRequestMapper.cs` — new.
- `Infrastructure/BackgroundServices/ExportRequestWorker.cs` — new `BackgroundService`.
- `Controllers/ExportRequestController.cs` — new.
- `Application/Extensions/DependencyInjection.cs`, `Infrastructure/Extensions/DependencyInjection.cs` — registered new services/repository + `services.AddHostedService<ExportRequestWorker>();` (live, unlike the disabled Kafka one).
- `SylviaNG.Recruitment.Tests/Services/{ExportRequestServiceTests,ExportGenerationServiceTests}.cs` — new.

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/enums/recruitment.enum.ts` — `ExportRequestStatusEnum`, `ExportFormatEnum` new.
- `@core/interfaces/recruitment-management/export-request.interface.ts` — new.
- `@core/services/recruitment/export-request/export-request.service.ts` — new.
- `pages/application-tracking/ats-dashboard/` — Export Format select + "Export Candidate List" button added to the filter bar, posting `buildFilterParams()` (the same `IAtsDashboardFilterParams` the dashboard/bulk-shortlist already build) to the new endpoint.
- `pages/export-request-management/export-request-list/**` — new module: status-filterable list, 15s polling while any row is Pending/Processing (same interval-polling idiom the header notification bell uses), download button once Completed. Registered as a lazy route (`export-requests`) in `pages-routing.module.ts`.
- `@core/constants/nav-menu-items.ts` — "Export Requests" added under Recruitment (Admin/HR).

## Verification

- `dotnet build` clean; `dotnet test` — 695/698 passing, the 3 failures are the pre-existing documented `InternalJobBoardControllerTests` NRE baseline, unrelated. New `ExportRequestServiceTests`/`ExportGenerationServiceTests` (12 tests) all pass.
- `dotnet ef database update` applied cleanly against local Postgres; `ExportRequests` table confirmed created with expected columns/indexes (`Status`, `ExpiresAt`).
- `npx tsc --noEmit` clean; `ng build` compiles clean — confirmed the export-request strings/route are present in the built lazy chunks.
- Full end-to-end via `curl` against the running local stack (Keycloak + Postgres, HR login `abir`/`abir123`): queued a candidate-list export with an empty filter (Xlsx) — first attempt surfaced the `OrderBy`/EF-translation bug live (`Failed` with the exact LINQ-translation error), fixed, rebuilt, requeued — completed in ~4s with `rowCount: 24`, downloaded and confirmed a valid `.xlsx` (ClosedXML-readable, correct 12-column header row, real candidate data with profile-preferred fields). Repeated with `format: Csv` — completed, downloaded, confirmed correct comma-escaping (`"C#, SQL, Angular"` quoted). Confirmed an invalid filter (`skills` without `jobPostingId`) returns `400 Validation failed` immediately, no row queued. Confirmed both completed requests produced an `ExportRequestReady` `NotificationLog` row targeting the requester's real email (`abir@sylviang.local`, resolved via the new `GetCurrentUserEmail()`), `Skipped`/"No active template mapping" — expected, same as every prior `RecruitmentEventEnum` addition with no `EventTemplateMapping` configured yet.
- Frontend dev server confirmed serving (port 4600) with the new bundle; browser UI not visually verified in this session (no browser-automation tool available) — API and build-level verification only, same caveat as several prior features.
