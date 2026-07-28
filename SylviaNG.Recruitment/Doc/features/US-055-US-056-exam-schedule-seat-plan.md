# US-055 — Schedule an Exam for Shortlisted Candidates + US-056 — Generate Seat Plan for an Exam

## What

HR schedules an `Exam` (title, date/time, duration, total/pass marks, in-person or online) against a job posting, then enrolls a set of that posting's shortlisted candidates into it. In-person exams are tied to one `ExamVenue`; online exams reference a `QuestionGroup` (the question set a later exam-taking feature, US-058, will consume — no exam-taking engine is built here). On enrollment, each candidate is automatically emailed their admit card (real SMTP) and gets a logged SMS notification (stub, no real gateway). Separately, HR can auto-generate a seat plan that distributes all enrolled candidates across the venue's active rooms up to each room's capacity, manually reassign any one candidate's room/seat afterward, and download the seat plan as PDF or Excel, or a single candidate's admit card as PDF.

## Why

Bundled per senior-approved EP-07 sequencing (55+56 → 58+59 → 57 → 60 → 61) — seat number is one field on the same `ExamEnrollment` row whether it's assigned by auto-generation (US-056 AC2) or written at enrollment time (US-055 AC3), so reviewing them as one change avoids splitting a single coherent data model across two PRs.

## Scope decisions

- **No separate `SeatPlan` entity.** `Exam.SeatPlanGeneratedAt` (nullable timestamp) is the only "has a plan run" marker; room/seat assignments live directly on `ExamEnrollment.ExamRoomId`/`SeatNumber`, populated by seat-plan generation or manual reassignment — same row, two possible moments.
- **"Exam hall" = one `ExamVenue` per exam**, seat-plan distribution spans that venue's active `ExamRoom`s only (only `ExamRoom` has `Capacity`). Cross-venue seat plans are out of scope.
- **Seat-plan regeneration is a full overwrite** — re-running "Generate Seat Plan" clears and reassigns every enrollment's room/seat from scratch, wiping prior manual reassignments. The frontend confirms before regenerating (not before the first generate).
- **Real SMTP email, stubbed SMS** (explicit user decision — SMS gateways cost money, out of scope). `ISmtpEmailService` short-circuits with a `Skipped`-free failure result (no connection attempt) when `SmtpSettings.IsEnabled=false` or `Host` is blank — safe by default for local/demo runs with no SMTP creds configured. `EmailNotificationStatus=Skipped` is reserved for "candidate has no email on file"; a disabled/unreachable SMTP server is recorded as `Failed` with the reason in `EmailFailureReason`, so HR can see *why* from the roster. `ISmsNotificationService` is a log-only stub (`LoggingSmsNotificationService`) — always "succeeds", no gateway call, no separate persisted audit entity (writes directly onto `ExamEnrollment.SmsNotificationStatus`/`SmsLoggedAt`, since no generic `AuditLog` infrastructure exists in this codebase).
- **A notification failure never blocks enrollment.** `ExamNotificationService.NotifyEnrollmentAsync` never throws (internal try/catch per channel); `ExamEnrollmentService.EnrollAsync` wraps the call again per enrollment as a second safety layer, so one candidate's mail-server hiccup can't fail the whole bulk enroll action.
- **No exam-taking/auto-scoring engine.** `Exam.QuestionGroupId` for online exams is a reference field only, consumed later by US-058.
- **Out of scope, deliberately not built:** `ExamUpdateCommand`/`ExamSetActiveStatusCommand` (no AC backing), any Score/Passed/Result field on `ExamEnrollment` (US-059/060), admit-card bulk distribution or candidate-portal download (US-057), generic `AuditLog` entity.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Domain/Entities/Exam.cs`, `ExamEnrollment.cs` — new entities; `Domain/Enums/Enum.cs` — added `ExamTypeEnum`, `NotificationStatusEnum`.
- `Infrastructure/Configurations/ExamConfiguration.cs`, `ExamEnrollmentConfiguration.cs` — new EF configurations.
- `Infrastructure/Data/ApplicationDBContext.cs` — added `Exams`/`ExamEnrollments` DbSets.
- `Migrations/AddExamAndExamEnrollment` — new migration (additive only — creates the 2 new tables + FKs/indexes).
- `Application/Interfaces/Repositories/IExamRepository.cs`, `IExamEnrollmentRepository.cs` + `Infrastructure/Repositories/ExamRepository.cs`, `ExamEnrollmentRepository.cs` — new.
- `Application/Interfaces/Services/IExamService.cs`, `IExamEnrollmentService.cs`, `IExamSeatPlanService.cs`, `IExamNotificationService.cs`, `ISmtpEmailService.cs`, `ISmsNotificationService.cs` + matching `Application/Services/*.cs` — new. `ExamSeatPlanService.GenerateExcelAsync` uses ClosedXML directly.
- `Application/Common/Email/EmailMessage.cs`, `Application/Common/Settings/SmtpSettings.cs` — new.
- `Infrastructure/Services/SmtpEmailService.cs` — new (MailKit; added as a new package via the `api.nuget.org` fallback source since the org proxy is unreachable on this machine).
- `Infrastructure/Documents/QuestPdfSeatPlanGenerator.cs`, `QuestPdfAdmitCardGenerator.cs` — new (QuestPDF, same pattern as `QuestPdfCvGenerator`).
- `Application/Features/Exams/`, `Application/Features/ExamEnrollments/` — new CQRS slices (Exams: Create, GetAllPaged, GetById. ExamEnrollments: EnrollCandidates, SeatPlanGenerate, ReassignSeat, GetByExam, SeatPlanDownloadPdf/Excel, AdmitCardDownloadPdf).
- `Application/Mappings/ExamMapper.cs`, `ExamEnrollmentMapper.cs` — new.
- `Controllers/ExamController.cs` (`[Route("recruitment/exam")]`, `[Authorize(Roles="Admin,HR")]`) — new.
- `Application/Extensions/DependencyInjection.cs`, `Infrastructure/Extensions/DependencyInjection.cs`, `appsettings.json` (disabled `Smtp` section) — updated.
- `SylviaNG.Recruitment.Tests/Services/ExamServiceTests.cs`, `ExamEnrollmentServiceTests.cs`, `ExamSeatPlanServiceTests.cs`, `SmtpEmailServiceTests.cs`, `Validators/ExamCreateValidatorTests.cs`, `ExamEnrollCandidatesValidatorTests.cs` — new unit tests.

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/enums/recruitment.enum.ts` — added `ExamTypeEnum`, `NotificationStatusEnum`.
- `@core/interfaces/recruitment-management/exam.interface.ts`, `exam-enrollment.interface.ts` — new.
- `@core/services/recruitment/exam/exam.service.ts` — new (CRUD, enroll, seat-plan generate/reassign, blob downloads via the existing `saveFileResponse` helper from `cv-bank.service.ts`).
- `pages/exam-management/` — new module: `exam-list/`, `schedule-exam/` (job-posting + exam-details form, then shortlisted-candidate picker), `exam-detail/` (roster, seat-plan generate/download, per-row reassign dialog and admit-card download), module + routing.
- `pages/pages-routing.module.ts` — added `exams` lazy route.
- `@core/constants/nav-menu-items.ts` — "Exams" nav item under Recruitment.
- `.claude/launch.json` (repo root) — added a `backend` dev-server config (`dotnet run`, port 8888) alongside the existing `frontend` entry, for browser-based verification.

## Verification

- `dotnet test` — 468/474 passing (72 new for this feature, all passing). The 6 pre-existing failures are unrelated: 3× `InternalJobBoardControllerTests` (known `ControllerContext.User` NRE gap) and 3× `AuthLoginSmokeTests` (need a live Keycloak realm, environmental).
- `dotnet ef database update` — migration applies cleanly against local Postgres, creating only `Exams`/`ExamEnrollments` (verified additive-only after a false start: an initial attempt against a snapshot deleted mid-session produced a spurious full-schema migration; recovered via `dotnet ef migrations remove` — which correctly rebuilt the snapshot from the last real migration's embedded model — then regenerated cleanly).
- `npm run build` (production) and `ng build` — compile clean.
- End-to-end via the browser against local Docker Postgres/Keycloak + `dotnet run` + `ng serve`: logged in as HR, scheduled an in-person "Written Aptitude Test" exam against the "Business Development Executive" posting, enrolled its one shortlisted candidate (Rakibul Islam) — roster showed `EmailNotificationStatus=Failed` (`EmailFailureReason="SMTP not configured"`, expected with no local SMTP creds) and `SmsNotificationStatus=Sent` (stub, confirmed logged: `[SMS-STUB] Would send SMS to 01700100005: ...`). Generated the seat plan (candidate assigned `Room 101-001` in `Room 101`), manually reassigned to `Room 101-005` and confirmed it persisted, downloaded Seat Plan PDF, Seat Plan Excel, and the candidate's Admit Card PDF (all `200 OK`, admit card ~44KB non-empty PDF). Zero browser console errors throughout.
