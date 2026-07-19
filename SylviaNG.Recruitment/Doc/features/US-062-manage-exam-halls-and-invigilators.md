# US-062 — Manage Exam Halls and Invigilators

## What

An admin/HR-only registry of physical exam halls: Hall Name, Location/Address, Total Capacity, a list of assigned Invigilators (Employees), and a `NotifyInvigilatorsOnAssign` config flag. Halls can be created, edited, and deactivated/reactivated.

## Why

EP-07's exam-day logistics stories (US-055 Schedule Exam, US-056 Seat Plan, US-062 Exam Halls) all need a shared concept of a physical hall. US-062 (Must Have, size S) was picked up standalone since US-055/US-056 aren't built yet — no `Exam` entity exists in the codebase. This feature builds the hall/invigilator registry now, ready to be referenced by the exam-scheduling and seat-plan features when they land.

## Scope decisions (diverges from the literal US-062 ACs)

The user story's ACs assume an `Exam` entity and a notification system that don't exist yet elsewhere in this codebase either. Rather than build speculative infrastructure for unbuilt upstream features, this follows the same precedent already established in `PipelineStage.NotifyInterviewersOnAssign` (a stored config flag with no send logic yet, since EP-09 notifications aren't built):

- **AC3** ("hall linked to an exam when generating the seat plan") — out of scope. No `Exam`/seat-plan entity exists (US-055/056). `ExamHall` is built so it can be FK'd from those features later, and exposes a `GET /recruitment/exam-hall/lookup` active-list endpoint for that future UI to consume (mirrors the `QuestionGroup`/`HiringPipeline` lookup pattern).
- **AC4** ("invigilators notified of exam date/venue") — out of scope for the same reason (no notification infra exists anywhere in this repo). `NotifyInvigilatorsOnAssign` is stored as a config flag only, same as `PipelineStage.NotifyInterviewersOnAssign`.
- **AC5** ("capacity warning when assigning candidates near capacity") — out of scope. No seat-assignment mechanism exists yet (US-056). `TotalCapacity` is validated as `> 0` at creation; live capacity tracking arrives with the seat-plan feature.
- **AC1, AC2** — fully implemented: hall CRUD, invigilator assignment, deactivate/reactivate.

Invigilators are `Employee` records (synced from Core HR via Kafka) — the same entity `PipelineStageInterviewer.EmployeeId` already uses for interviewer assignment, not `StaffProfile` (which is Admin/HR's own account-settings profile, unrelated). The frontend captures invigilator IDs as a comma-separated free-text field, matching `manage-hiring-pipeline.component.ts`'s `interviewerEmployeeIds` pattern — there is no employee-lookup/search endpoint anywhere in this codebase to build a picker against.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Domain/Entities/ExamHall.cs`, `Domain/Entities/ExamHallInvigilator.cs` — new entities (join entity mirrors `PipelineStageInterviewer.cs`).
- `Infrastructure/Configurations/ExamHallConfiguration.cs`, `ExamHallInvigilatorConfiguration.cs` — new EF configurations.
- `Infrastructure/Data/ApplicationDBContext.cs` — added `ExamHalls`/`ExamHallInvigilators` DbSets.
- `Migrations/20260719053006_AddExamHall.cs` — new migration.
- `Application/Interfaces/Repositories/IExamHallRepository.cs`, `Infrastructure/Repositories/ExamHallRepository.cs` — new (mirrors `HiringPipelineRepository`).
- `Application/Interfaces/Services/IExamHallService.cs`, `Application/Services/ExamHallService.cs` — new. Validates name uniqueness, `TotalCapacity > 0`, and invigilator employee IDs against the `Employees` table (same shape as `HiringPipelineService.ValidateInterviewerIdsAsync`).
- `Application/Features/ExamHalls/` — new CQRS slice (Commands: Create/Update/SetActiveStatus, Queries: GetById/GetAll/GetActiveLookup), mirrors `Application/Features/QuestionGroups/`.
- `Application/Mappings/ExamHallMapper.cs` — new.
- `Controllers/ExamHallController.cs` — new, `[Route("recruitment/exam-hall")]`, `[Authorize(Roles = "Admin,HR")]`.
- `Application/Extensions/DependencyInjection.cs`, `Infrastructure/Extensions/DependencyInjection.cs` — registered the new service/repository.
- `SylviaNG.Recruitment.Tests/Services/ExamHallServiceTests.cs`, `SylviaNG.Recruitment.Tests/Validators/ExamHallCreateValidatorTests.cs` — new unit tests.

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/interfaces/recruitment-management/exam-hall.interface.ts` — new.
- `@core/services/recruitment/exam-hall/exam-hall.service.ts` — new.
- `pages/exam-hall-management/` — new module: `exam-hall-list/`, `manage-exam-hall/`, module + routing (mirrors `exam-question-management`).
- `pages/pages-routing.module.ts` — added `exam-halls` lazy route.
- `@core/constants/nav-menu-items.ts` — added "Exam Halls" nav item under Recruitment.

## Verification

- `dotnet test` — 215/215 passing (11 new for this feature).
- `dotnet ef database update` — migration applies cleanly against local Postgres, creating only `ExamHalls`/`ExamHallInvigilators` (no unrelated table changes).
- `ng build` (development config) — compiles clean.
- End-to-end via Playwright against local Docker Postgres/Keycloak + `dotnet run` + `ng serve`: logged in as HR, navigated to Exam Halls, created a hall ("Main Auditorium", capacity 100), confirmed it appears in the list as Active, deactivated it and confirmed the status flips to Inactive with an Activate action appearing. Zero browser console errors throughout.
