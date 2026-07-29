# EP-09 Feature 3 — Notification Log + In-App Bell (US-078, US-079)

## What

The final feature of EP-09 (Candidate Communication & Notifications). Delivers the two remaining
user stories:

- **US-078 — View Notification Delivery Logs**: HR/Recruiter can see every notification the system
  has sent (recipient, channel, event, delivery status), filter the list (date range, channel,
  event, delivery status), retry a failed send, and export the filtered list to Excel.
- **US-079 — In-App Notification Bell for HR Users**: a bell icon in the admin UI header shows an
  unread-count badge, opens a dropdown of the 10 most recent unread notifications on click, marks
  items as read on open (with a "mark all as read" action), and polls every 30 seconds for new
  unread notifications.

## Why

Feature 2 (dispatch + bulk-notify + OTP login) already built the `NotificationLog` table — one row
per (event, recipient) dispatch attempt — specifically so this feature wouldn't need new dispatch
plumbing, only a read/query layer and UI on top of data that already exists. This closes out EP-09.

## Design notes

- `NotificationLog` gained three columns: `IsRead`/`ReadAt` (read-state tracking for the bell, which
  didn't exist before) and `RenderedBody` (the dispatch service already computed this at render time
  but only persisted `RenderedSubject` — now both are stored so a `Failed` row can be **retried** by
  resending the already-rendered content, with no dependency on the original placeholder values,
  which were never persisted).
- The HR-facing log view (US-078) shows **every** `RecipientType` — both candidate-facing and
  internal AdminHr sends — because HR needs to verify communications regardless of audience. The
  bell (US-079) is scoped to `RecipientType = AdminHr` only, since it's an HR/Admin-facing widget.
- Retry only operates on `Failed` rows (guarded by a `FluentValidation.ValidationException`,
  matching this codebase's manual-validation convention for flat services) — `Skipped` rows (no
  active template mapping existed at dispatch time) are a different, non-retryable outcome.
- The bell's "link to relevant page" (US-079 AC3) resolves to `/applications/{jobApplicationId}`
  uniformly — every AdminHr-relevant event carries a `JobApplicationId` except `AccountCreatedOtp`,
  which is candidate-only and never appears in an AdminHr-scoped feed, so no per-event routing table
  was needed.
- Real-time updates (US-079 AC5) use a 30-second RxJS polling interval, not a new WebSocket/SignalR
  transport — no such transport exists anywhere else in this backend yet, and polling a lightweight
  `/unread-count` endpoint is proportionate for a Should-Have story.

## Files created/changed

Backend (`SylviaNG.Recruitment`):
- `Domain/Entities/NotificationLog.cs` — added `IsRead`, `ReadAt`, `RenderedBody`.
- `Application/Services/NotificationDispatchService.cs` — persists `RenderedBody` at render time.
- `Application/Interfaces/Repositories/INotificationLogRepository.cs`,
  `Infrastructure/Repositories/NotificationLogRepository.cs` — filtered queryable, unread-for-AdminHr,
  unread-count, bulk mark-all-read.
- `Application/Interfaces/Services/INotificationLogService.cs`,
  `Application/Services/NotificationLogService.cs` — filtering/pagination, Excel export (ClosedXML,
  same pattern as `CvBankCvBulkExportExcelHandler`), retry, mark-read, mark-all-read.
- `Application/Mappings/NotificationLogMapper.cs` — entity → response, including a derived
  `RecipientName` (candidate full name, or "Admin / HR" for internal rows).
- `Application/Features/NotificationLogs/**` — MediatR Queries (`GetAll`, `GetUnread`,
  `GetUnreadCount`, `ExportExcel`) and Commands (`Retry`, `MarkAsRead`, `MarkAllAsRead`).
- `Controllers/NotificationLogController.cs` — `recruitment/notification-log`, `Authorize(Roles =
  "Admin,HR")`.
- `Application/Extensions/DependencyInjection.cs` — registers `INotificationLogService`.
- `Migrations/*_AddNotificationLogReadAndBodyTracking.cs` — adds the three new columns.
- `SylviaNG.Recruitment.Tests/Services/NotificationLogServiceTests.cs` — retry guard/success/failure,
  mark-read idempotency, mark-all-read, unread count/list mapping.

Frontend (`sylviang.adminui.recruitment-main`):
- `src/app/@core/interfaces/recruitment-management/notification-log.interface.ts`,
  `src/app/@core/services/recruitment/notification-log/notification-log.service.ts`.
- `src/app/pages/notification-management/notification-log-list/*` — US-078 log list page (filter
  bar, lazy-paginated table, retry action, Excel export), registered as route
  `notification-log-list` with `roles: [Admin, HR]` (the other pages in this module are Admin-only).
- `src/app/pages/notification-management/notification-management-routing.module.ts`,
  `notification-management.module.ts` — route + declaration registration.
- `src/app/shell/components/notification-bell/*` — US-079 bell widget (badge, polling, dropdown,
  mark-read/mark-all-read).
- `src/app/shell/components/header/header.component.ts`, `.html` — mounts the bell for Admin/HR
  only via a `showNotificationBell` getter (`authService.getRole()`); `src/app/shell/shell.module.ts`
  registers the new component.

## Known gap

`SylviaNG.Recruitment.Tests/Services/AuthServiceTests.cs` and `JobApplicationServiceTests.cs` were
already failing to compile before this feature (missing constructor args from Feature 2's OTP-login
work) — this blocks `dotnet test` for the whole project, including the new
`NotificationLogServiceTests.cs`. Not fixed here (out of scope); the new test file was verified to
compile cleanly on its own via a targeted build.

## Verification

Built + live-tested 2026-07-24: real API calls (login as `abir`/HR, exercised every endpoint against
real `NotificationLog` rows produced by Feature 2's dispatch pipeline — filters, mark-read,
mark-all-read, retry guard rails, Excel export) plus a headless-browser pass (bell renders for HR,
badge/panel/mark-all-read work, log list page renders real rows with correct formatting) with zero
console errors. Verified US-078's 4 ACs and US-079's 5 ACs individually.
