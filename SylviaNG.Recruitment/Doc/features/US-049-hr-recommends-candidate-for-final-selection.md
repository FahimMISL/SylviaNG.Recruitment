# US-049 — HR Recommends Candidate for Final Selection

## What

Lets HR recommend a shortlisted candidate for final selection, with a required written justification, from the pipeline tracker dialog on the ATS dashboard. The recommendation sits `Pending` until the Hiring Manager reviews it (Accept/Reject with a comment) from a dedicated review queue. The pipeline tracker always shows the application's current recommendation status.

## Why

AC1-5 in USER_STORIES.md — fast-tracks exceptional candidates to the offer stage without going through every pipeline stage, while keeping a Hiring Manager sign-off in the loop.

## Design notes

- **No separate Hiring Manager role.** This codebase's auth scheme has exactly three hardcoded roles (Admin/HR/Candidate) — no fourth "Hiring Manager" account. Per explicit user decision, the existing **Admin** role plays the Hiring Manager here: the review queue and Accept/Reject actions are Admin-only, layered on top of the controller's base `[Authorize(Roles = "Admin,HR")]` via a second method-level `[Authorize(Roles = "Admin")]` (ASP.NET Core requires *all* stacked `[Authorize]` attributes to pass, so this correctly narrows to Admin-only without duplicating the HR-visible endpoints).
- **Independent of pipeline stages.** `CandidateRecommendation` is its own entity, not folded into `JobApplicationStageProgress` — a recommendation's Pending/Accepted/Rejected lifecycle is orthogonal to stage progress, and doesn't touch `ApplicationStatusEnum`/the status transition matrix at all.
- **One Pending recommendation at a time per application** — creating a new one while one is already Pending is rejected (`ValidationException`); a prior Rejected/Accepted recommendation doesn't block a fresh one (re-recommend after rejection is allowed).
- **Menu-role-filter bug found and fixed.** `MenuService.loadFallbackMenu` only filtered *top-level* sidebar items by role — nested `subItems` (like this feature's Admin-only "Final Selection Recommendations" link) were never actually filtered, only decorated with an inert `roles` field. Made `filterByRole` recursive so per-role restriction on sub-items is enforced, not just cosmetic. Confirmed via Playwright: HR does not see the link even after expanding "Recruitment"; Admin does.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Domain/Enums/Enum.cs` — added `RecommendationStatusEnum` (Pending/Accepted/Rejected).
- `Domain/Entities/CandidateRecommendation.cs` — new entity.
- `Infrastructure/Configurations/CandidateRecommendationConfiguration.cs` — table `CandidateRecommendations`, FK to `JobApplications` (cascade delete), index on `(JobApplicationId, Status)`.
- `Infrastructure/Data/ApplicationDBContext.cs` — added `DbSet<CandidateRecommendation>`.
- `Migrations/20260714104742_AddCandidateRecommendation.cs` (+ Designer) — new migration. Hand-edited to strip an unrelated `DropTable("SavedSearches")`/recreate that EF scaffolded because this branch (cut from `dev`) doesn't yet have the still-unmerged US-048 PR in its model — see `Doc/features/US-048-*.md`. Standard parallel-feature-branch migration hygiene; doesn't affect the actual `CandidateRecommendations` schema change.
- `Application/Interfaces/Repositories/ICandidateRecommendationRepository.cs` + `Infrastructure/Repositories/CandidateRecommendationRepository.cs` — `GetLatestByJobApplicationIdAsync`, `GetPendingWithApplicationAsync`.
- `Application/Features/CandidateRecommendations/Models/` — Create/Review/Response/PendingListItem DTOs.
- `Application/Interfaces/Services/ICandidateRecommendationService.cs` + `Application/Services/CandidateRecommendationService.cs` — flat service (no MediatR), manual validation.
- `Application/Mappings/CandidateRecommendationMapper.cs`.
- `Controllers/CandidateRecommendationController.cs` — `GET/POST job-application/{id}/recommendation`, `GET candidate-recommendation/pending` (Admin-only), `PUT candidate-recommendation/{id}/review` (Admin-only).
- `Application/Extensions/DependencyInjection.cs`, `Infrastructure/Extensions/DependencyInjection.cs` — DI registration.
- `SylviaNG.Recruitment.Tests/Services/CandidateRecommendationServiceTests.cs` — new; 11 tests.

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/enums/recruitment.enum.ts` — added `RecommendationStatusEnum`.
- `@core/interfaces/recruitment-management/candidate-recommendation.interface.ts` — new.
- `@core/services/recruitment/candidate-recommendation/candidate-recommendation.service.ts` — new.
- `pages/application-tracking/pipeline-progress-tracker/pipeline-progress-tracker.component.ts/html/scss` — recommendation status panel + "Recommend for Final Selection" dialog (AC1/AC2/AC4).
- `pages/candidate-recommendation-management/` — new module: `candidate-recommendation-list` component (Hiring Manager's review queue, Accept/Reject with comments dialog, AC3/AC5), routing module with `RoleGuard` (Admin-only).
- `pages/pages-routing.module.ts` — new lazy route `candidate-recommendations`.
- `@core/constants/nav-menu-items.ts` — new Admin-only sidebar sub-item under Recruitment.
- `@core/services/menu.service.ts` — fixed `filterByRole` to recurse into `subItems` (see Design notes).

## Verification

- `dotnet test` — 215/215 passing (204 pre-existing on this branch's base + 11 new `CandidateRecommendationServiceTests`, no regressions).
- `dotnet ef database update` — confirmed `CandidateRecommendations` table created; confirmed the hand-edited migration did NOT drop `SavedSearches` (checked directly in Postgres).
- Backend end-to-end via `curl`: create (HR) → 200; duplicate-pending → 400; `GET latest` reflects Pending; HR `GET pending`/`PUT review` → 403; Admin `GET pending` → 200 with the row; Admin `PUT review` (Accept) → 200; `GET latest` reflects Accepted + reviewer/comments; queue empty afterward; re-review already-reviewed → 400.
- `ng build` (development config) — compiles clean.
- Frontend end-to-end via Playwright: HR submits a recommendation from the pipeline tracker dialog → panel shows "Final Selection: Pending"; HR does not see the "Final Selection Recommendations" nav link (0 count, confirmed after expanding Recruitment); Admin does see it and the correct pending card (candidate name, job title, justification, recommender); Admin accepts with a comment → toast, queue empty; pipeline tracker (viewed as HR again) shows "Final Selection: Accepted" with the Admin's name/comment. No leftover test data (checked directly in Postgres).
