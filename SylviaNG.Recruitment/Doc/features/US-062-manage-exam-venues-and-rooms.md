# US-062 — Manage Exam Venues and Rooms

## What

An admin/HR-only registry of physical exam venues (location) each containing one or more rooms. A venue has a Venue Name and Location/Address. Each room belongs to exactly one venue and has its own Room Name, Capacity, a list of assigned Invigilators (Employees), and a `NotifyInvigilatorsOnAssign` config flag. Both venues and rooms can be created, edited, and deactivated/reactivated independently.

## Why

This replaces the flat "Exam Hall" model (one hall = one name/location/capacity/invigilator-set) with a two-level Venue → Room structure, matching real exam-day logistics where one physical location contains several independently-sized, independently-staffed rooms. Seat-plan layout within a room is explicitly out of scope — Room is deliberately just Name + Capacity, ready for a future seat-plan feature to reference by `ExamRoomId`.

## Scope decisions

- **Invigilators assign at room level**, not venue level — real exam ops assign invigilator teams per room, not per building.
- **No seat-plan field on Room** — the seat-plan layout feature itself isn't being built; Room only exists as the structural unit that feature will attach to later.
- **`NotifyInvigilatorsOnAssign` moved from venue to room** — since invigilator assignment now happens at room level, the notify-on-assign config flag (stored only, no send logic — same precedent as `PipelineStage.NotifyInterviewersOnAssign` and the flag's original placement on the old `ExamHall` entity) follows it there.
- **No cascade-deactivate** — deactivating a venue does not auto-deactivate its rooms; each is managed independently, matching the minimal-scope precedent from the original Exam Hall feature.
- **Built directly on `dev`, no rename-in-place migration** — the prior "Exam Hall" implementation (`feature/exam-hall-invigilator-management`) was never merged to `dev`, so this feature is a fresh `AddExamVenueRoom` migration rather than a rename of an existing `ExamHalls` table.
- No `Exam` entity reference yet (US-055/US-056 aren't built) — `ExamRoomId` is exposed via `GET /recruitment/exam-venue/{id}` (embedded room list) for those future features to consume, same as the old `ExamHall` lookup pattern.

Invigilators are `Employee` records (synced from Core HR via Kafka), same as `PipelineStageInterviewer.EmployeeId` and the original Exam Hall feature — not `StaffProfile`. Captured as a comma-separated free-text field on the room form (no employee-lookup/search endpoint exists in this codebase to build a picker against).

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Domain/Entities/ExamVenue.cs`, `ExamRoom.cs`, `ExamRoomInvigilator.cs` — new entities.
- `Infrastructure/Configurations/ExamVenueConfiguration.cs`, `ExamRoomConfiguration.cs`, `ExamRoomInvigilatorConfiguration.cs` — new EF configurations.
- `Infrastructure/Data/ApplicationDBContext.cs` — added `ExamVenues`/`ExamRooms`/`ExamRoomInvigilators` DbSets.
- `Migrations/AddExamVenueRoom` — new migration (additive only — creates the 3 new tables).
- `Application/Interfaces/Repositories/IExamVenueRepository.cs`, `IExamRoomRepository.cs` + `Infrastructure/Repositories/ExamVenueRepository.cs`, `ExamRoomRepository.cs` — new.
- `Application/Interfaces/Services/IExamVenueService.cs`, `IExamRoomService.cs` + `Application/Services/ExamVenueService.cs`, `ExamRoomService.cs` — new. Room service validates venue exists, room-name uniqueness scoped to the venue, `Capacity > 0`, and invigilator employee IDs against the `Employees` table.
- `Application/Features/ExamVenues/`, `Application/Features/ExamRooms/` — new CQRS slices (Commands: Create/Update/SetActiveStatus, Queries: GetById/GetAll/GetActiveLookup for venue, GetById/GetAllByVenue for room).
- `Application/Mappings/ExamVenueMapper.cs`, `ExamRoomMapper.cs` — new.
- `Controllers/ExamVenueController.cs` (`[Route("recruitment/exam-venue")]`), `Controllers/ExamRoomController.cs` (`[Route("recruitment/exam-venue/{examVenueId}/room")]`) — new, `[Authorize(Roles = "Admin,HR")]`.
- `Application/Extensions/DependencyInjection.cs`, `Infrastructure/Extensions/DependencyInjection.cs` — registered the new services/repositories.
- `SylviaNG.Recruitment.Tests/Services/ExamVenueServiceTests.cs`, `ExamRoomServiceTests.cs`, `Validators/ExamVenueCreateValidatorTests.cs`, `ExamRoomCreateValidatorTests.cs` — new unit tests.

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/interfaces/recruitment-management/exam-venue.interface.ts`, `exam-room.interface.ts` — new.
- `@core/services/recruitment/exam-venue/exam-venue.service.ts`, `exam-room/exam-room.service.ts` — new.
- `pages/exam-venue-management/` — new module: `exam-venue-list/`, `manage-exam-venue/`, `exam-room-list/`, `manage-exam-room/`, module + routing.
- `pages/pages-routing.module.ts` — added `exam-venues` lazy route.
- `@core/constants/nav-menu-items.ts` — "Exam Venues" nav item under Recruitment.

## Verification

- `dotnet test` — 221/221 passing (10 new for this feature).
- `dotnet ef database update` — migration applies cleanly against local Postgres, creating only `ExamVenues`/`ExamRooms`/`ExamRoomInvigilators` (verified additive-only, no unrelated table changes — an earlier attempt against a stale local EF model snapshot briefly scaffolded spurious drops of unrelated tables; regenerated from a clean snapshot before applying).
- `ng build` (development config) — compiles clean.
- End-to-end via Playwright against local Docker Postgres/Keycloak + `dotnet run` + `ng serve`: logged in as HR, created a venue ("Main Campus", "123 University Ave"), added two rooms under it with different capacities and invigilator sets, confirmed the room list shows correct capacity/invigilator counts independently, deactivated one room and confirmed the venue and other room were unaffected. Zero browser console errors throughout.
