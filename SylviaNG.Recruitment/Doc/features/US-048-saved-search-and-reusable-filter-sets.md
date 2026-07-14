# US-048 — Saved Search and Reusable Filter Sets

## What

Lets an HR/Recruiter save the ATS dashboard's current filter combo (job posting, status, source, dates, education, experience range, skills, location, age range — the same 12-property snapshot the dashboard already round-trips to `sessionStorage`) under a name, reload it later via a dropdown, and edit/rename/delete it. Personal by default; can be marked shared so the whole HR team sees it too.

## Why

HR currently re-enters the same filter combo every session — `ats-dashboard.component.ts` persists it to `sessionStorage`, but only within a tab session, not across logins or shareable with teammates. US-048 (Should Have, size S) turns that same snapshot into a named, persisted, optionally-team-shared bookmark. Natural follow-on to US-050 (ad-hoc criteria filter panel).

## Design notes

- **JSON blob column, not relational criteria rows.** Unlike `ShortlistFilter` (US-043), whose `Criteria` collection is evaluated server-side against candidates, a saved search is never evaluated — it's replayed verbatim into the dashboard's existing GET query params. A single `FilterJson` text column avoids inventing a parallel relational schema with zero server-side benefit; the backend treats it as opaque.
- **Per-owner mutate restriction on an otherwise-shared-visible resource.** First ownership concept in this codebase (`ShortlistFilter`/`HiringPipeline` are global, no ownership). `SavedSearch.OwnerUserName` (resolved via `ICurrentUserService.GetCurrentUserName()`, same attribution pattern as the rest of the app) gates Update/Delete; a new `ForbiddenException` (403) enforces it, with an Admin-role bypass via the new `ICurrentUserService.IsInRole(string)` method. Visibility (the `/lookup` endpoint) is separate from mutate rights: current user's own + everyone's shared searches are visible, but only the owner (or an Admin) can edit/delete.
- **Per-owner name uniqueness, not global.** Unique index on `(OwnerUserName, Name)` — two HR users can each have a search of the same name.
- **Flat service, no MediatR.** Mirrors `JobApplicationStageProgressService`/`Controller` (the codebase's existing no-MediatR precedent), not `ShortlistFilterService`'s Commands/Queries wrapping — this feature has no business logic complex enough to warrant it.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Domain/Entities/SavedSearch.cs` — new entity (`SavedSearchId`, `Name`, `OwnerUserName`, `IsShared`, `FilterJson`).
- `Infrastructure/Configurations/SavedSearchConfiguration.cs` — table `SavedSearches`, unique `(OwnerUserName, Name)` index.
- `Infrastructure/Data/ApplicationDBContext.cs` — added `DbSet<SavedSearch> SavedSearches`.
- `Migrations/20260714100722_AddSavedSearch.cs` (+ Designer) — new migration, applied to local dev Postgres.
- `Application/Interfaces/Repositories/ISavedSearchRepository.cs` + `Infrastructure/Repositories/SavedSearchRepository.cs` — `ExistsByNameForOwnerAsync`, `GetVisibleAsync`.
- `Application/Features/SavedSearches/Models/` — `SavedSearchCreateRequest`, `SavedSearchUpdateRequest`, `SavedSearchLookupResponse`.
- `Application/Interfaces/Services/ISavedSearchService.cs` + `Application/Services/SavedSearchService.cs` — Create/Update/Delete/GetVisibleLookup, manual validation (no `AbstractValidator<T>`, following `HiringPipelineService.ValidateInterviewerIdsAsync`'s style), ownership enforcement.
- `Application/Common/Exceptions/ForbiddenException.cs` — new; wired into `Middlewares/GlobalExceptionHandlerMiddleware.cs` → 403.
- `Application/Interfaces/Services/ICurrentUserService.cs` / `Application/Services/CurrentUserService.cs` — added `IsInRole(string role)`.
- `Application/Mappings/SavedSearchMapper.cs` — `ToEntity`, `ToLookupResponse`.
- `Controllers/SavedSearchController.cs` — `GET lookup`, `POST`, `PUT {id}`, `DELETE {id}`, `[Authorize(Roles = "Admin,HR")]`.
- `Application/Extensions/DependencyInjection.cs`, `Infrastructure/Extensions/DependencyInjection.cs` — DI registration.
- `SylviaNG.Recruitment.Tests/Services/SavedSearchServiceTests.cs` — new; 10 tests covering create/duplicate-name/no-resolvable-owner/not-found/forbidden-update/admin-bypass/forbidden-delete/delete/visible-lookup.

**Frontend:** not yet implemented this round — frontend `dev` branch doesn't exist yet in `sylviang.adminui.recruitment-main` (per-user instruction, deferred until it's created). The dropdown/dialog UI (`ats-dashboard.component.ts/html`, new `saved-search.interface.ts`/`saved-search.service.ts`) will land in a follow-up once that's ready.

## Verification

- `dotnet test` — 224/224 passing (214 pre-existing + 10 new `SavedSearchServiceTests`, no regressions).
- `dotnet ef database update` — confirmed `SavedSearches` table + unique `(OwnerUserName, Name)` index created on local dev Postgres.
- End-to-end via `curl` against local Docker Postgres/Keycloak + `dotnet run`, logged in as `abir`/HR: create → 200 (id returned); duplicate name for same owner → 409; `GET /lookup` reflects it with `isOwner: true`; update (rename + share) → 200, reflected in lookup; delete → 200, lookup empty afterward. Forbidden/Admin-bypass paths verified via unit tests only — no second HR account available locally to exercise cross-user 403 over HTTP.
