# US-053 — Create and Manage Exam Questions

## What

A Question Bank module for HR/Recruiter: reusable **Question Groups** (topic/difficulty buckets) containing **Exam Questions** of four types — MCQ (single correct), MCQ (multiple correct), True/False, and Subjective. Each question carries marks, a difficulty level, and an optional explanation; MCQ/True-False questions carry a set of options with correctness flags. Questions can be searched, filtered (by group/type/difficulty/status), edited, and deactivated from a paged list. Question Groups have their own simple CRUD + activate/deactivate.

## Why

EP-07's exam/assessment stories (US-051/052, in flight on a separate branch) need a bank of reusable questions to draw from once exam scheduling (US-055+) lands. US-053 (Must Have, size L) builds that bank standalone, so question authoring isn't blocked on the assessment-workflow stories landing first.

## Design notes

- **CQRS/MediatR throughout**, not the older Service+Repository-only style — this is the established convention for every feature since `ShortlistFilters`/`HiringPipelines`. `QuestionGroup` and `ExamQuestion` each get their own controller (`QuestionGroupController`, `ExamQuestionController`), matching `ShortlistFilterController` standing alone as an aggregate root rather than nesting under a parent route — `ExamQuestion` needs flat, independently searchable/filterable/paged access with `QuestionGroupId` as just one optional filter, not a mandatory URL segment.
- **True/False reuses the Options collection** used by MCQ, rather than a separate boolean field — this keeps one "at least one correct answer marked" validation path for every non-Subjective type, with no per-type branching in `ExamQuestionMapper`. The frontend seeds the two locked True/False rows (`newQuestionOptionsForType` in `manage-exam-question.component.constants.ts`) when that type is picked; the backend validator enforces the shape generically (`ExamQuestionCreateValidator`/`ExamQuestionUpdateValidator`).
- **Subjective questions carry no options** (validator enforces zero) and use a separate `ModelAnswer` field distinct from the type-agnostic `Explanation` field.
- **No hard-delete** for `ExamQuestion` or `QuestionGroup` — AC5 says "deactivated," never "deleted," and a delete would risk orphaning future exam-attempt data once US-055+ lands. Only Create/Update/GetById/GetAllPaged(or GetAll)/SetActiveStatus exist. This is a deliberate deviation from `ShortlistFilter`'s Delete-command precedent.
- **Paged/filtered query** follows `JobPostingRepository.GetPaginatedByCircularTypesAsync`'s real precedent (extra `[FromQuery]` scalar params alongside `PagedRequest`, Where-chain in the repository before `ToPaginatedResultAsync`) rather than inventing a `PagedRequest` subclass.
- **Dashboard showed all-zero stats for HR/Admin during manual testing** (`DashboardService.GetRole()` picks an arbitrary role claim via `FindFirst(ClaimTypes.Role)` when a Keycloak user carries several, and can silently resolve to `Candidate`). Not fixed here — already fixed on the separate, already-pushed `fix/ai-resume-parsing` branch (pending merge to `dev`); fixing it again on this branch would just conflict with that PR. Noting it so it isn't mistaken for a regression introduced by this feature.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Domain/Entities/QuestionGroup.cs`, `ExamQuestion.cs`, `ExamQuestionOption.cs` — new entities.
- `Domain/Enums/Enum.cs` — added `QuestionTypeEnum`, `DifficultyLevelEnum`.
- `Infrastructure/Configurations/QuestionGroupConfiguration.cs`, `ExamQuestionConfiguration.cs`, `ExamQuestionOptionConfiguration.cs` — new EF configurations.
- `Infrastructure/Data/ApplicationDBContext.cs` — new `DbSet`s.
- `Application/Interfaces/Repositories/IQuestionGroupRepository.cs`, `IExamQuestionRepository.cs` + `Infrastructure/Repositories/QuestionGroupRepository.cs`, `ExamQuestionRepository.cs` — new repositories.
- `Application/Interfaces/Services/IQuestionGroupService.cs`, `IExamQuestionService.cs` + `Application/Services/QuestionGroupService.cs`, `ExamQuestionService.cs` — new services (duplicate-name guard, whole-Options-replace on update).
- `Application/Mappings/QuestionGroupMapper.cs`, `ExamQuestionMapper.cs` — new manual mappers.
- `Application/Features/QuestionGroups/**`, `Application/Features/ExamQuestions/**` (excluding the bulk-import/template pieces, see US-054) — Models/Commands/Queries for Create/Update/SetActiveStatus/GetById/GetAll/GetAllPaged/GetActiveLookup.
- `Controllers/QuestionGroupController.cs`, `ExamQuestionController.cs` — new controllers.
- `Application/Extensions/DependencyInjection.cs`, `Infrastructure/Extensions/DependencyInjection.cs` — new service/repository registrations.
- `Migrations/20260717052938_AddExamQuestionBank.cs` + `.Designer.cs` — new migration (`QuestionGroups`, `ExamQuestions`, `ExamQuestionOptions` tables).
- `SylviaNG.Recruitment.Tests/Services/QuestionGroupServiceTests.cs`, `ExamQuestionServiceTests.cs` — new unit tests.
- `SylviaNG.Recruitment.Tests/Validators/ExamQuestionCreateValidatorTests.cs` — new validator tests.

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/interfaces/recruitment-management/question-group.interface.ts`, `exam-question.interface.ts` — new DTOs.
- `@core/enums/recruitment.enum.ts` — added `QuestionTypeEnum`, `DifficultyLevelEnum`.
- `@core/services/recruitment/question-group/question-group.service.ts`, `exam-question/exam-question.service.ts` — new services.
- `pages/exam-question-management/**` — new module: `question-group-list/`, `manage-question-group/`, `exam-question-list/`, `manage-exam-question/` components + module/routing module. `manage-exam-question.component.html`'s per-option delete button and `exam-question-list.component.ts`'s pagination param key were fixed after manual testing (see Bugs found below).
- `pages/pages-routing.module.ts` — new lazy `exam-questions` route.
- `@core/constants/nav-menu-items.ts` — new "Question Groups" / "Exam Questions" nav entries.
- `pages/dashboard/dashboard.component.html` — new "Exam Questions" quick-link card (Admin/HR only), added after noticing it was the only sub-feature missing one relative to Recruitment/Hiring Pipelines/Internal Job Board.

## Bugs found and fixed during manual testing

- **Filter-dropdown CSS overlap**: the four `p-select` filters (group/type/difficulty/status) collapsed to content width and visually overlapped — missing `min-width` on the filter `p-floatlabel`s. Fixed via a `.filter-bar` SCSS class, same pattern `ats-dashboard.component.scss` already used for its own filter row.
- **Delete-option button invisible**: `manage-exam-question`'s per-option remove button used a PrimeNG `text`-variant danger button (`text="true"`) that was too low-contrast to see against the row background, making it look like there was no way to remove an accidentally-added option. Switched to the plain `class="text-danger"` icon-button pattern already used and visibly working in `exam-question-list`/`job-vacancy-list`'s row-action columns.
- **Paged list ignored the page you clicked**: `exam-question-list.component.ts` sent the page number as query param `pageNumber`, but the backend's `PagedRequest.Page` property binds from a query key of `page` — every request silently re-fetched page 1 regardless of which page was clicked (the paginator's own footer text is computed client-side from local state, so it looked correct even though the data never changed, which is what made this easy to miss). Fixed the query param key. **`job-vacancy-list.component.ts` sends the same wrong `pageNumber` key against the same `PagedRequest` shape and likely has the identical bug** — not fixed here (unrelated to this feature), flagged for a follow-up sweep across all paged list pages once other in-flight branches land.
- Two other issues surfaced during testing turned out to already be fixed on separate, already-pushed branches pending merge to `dev` — not touched here to avoid conflicting with those PRs: the dashboard's all-zero HR/Admin stats (`DashboardService.GetRole()`, fixed on `fix/ai-resume-parsing`) and the internal-apply-form not auto-filling name/email for a logged-in candidate (fixed on `feature/us005-internal-candidate-prepopulation`).

## Verification

- `dotnet test` — 232/232 passing (204 pre-existing + 28 new across `QuestionGroupServiceTests`, `ExamQuestionServiceTests`, `ExamQuestionCreateValidatorTests`, plus US-054's import/bulk-import tests).
- `dotnet ef migrations add AddExamQuestionBank` produced a purely additive migration (3 `CreateTable` + 5 `CreateIndex` in `Up()`, matching 3 `DropTable` in `Down()`) after regenerating a clean baseline snapshot — the gitignored `ApplicationDBContextModelSnapshot.cs` had gone stale from a sibling in-flight branch (`feature/assessment-workflow-stages`) and initially produced a migration that dropped that branch's tables; recovered via `git stash` to a clean `dev`-only tree, regenerated the snapshot, then re-applied this feature's changes.
- `dotnet ef database update` applied cleanly against local Postgres (`sylviang-postgres-dev`).
- `ng build` (development config) compiles clean; `exam-question-management-module` lazy chunk generated (~120 kB).
- Full manual walkthrough completed against Docker Postgres/Keycloak + `dotnet run` + `ng serve` with a real HR user: created a Question Group, created and edited MCQ (single/multiple), True/False, and Subjective questions, exercised each type's option-shape rules, deactivated a question and confirmed it drops out of the Active filter, confirmed search/filter/pagination all work correctly after the fixes above.
