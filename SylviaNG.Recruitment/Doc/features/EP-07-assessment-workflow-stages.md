# EP-07 — Assessment Workflow & Stage Configuration (US-051, US-052)

## What

HR can create a named, reusable **Assessment Workflow** made up of one or more ordered **stages** (Written Test, Aptitude Test, Psychometric Test, Group Discussion, Practical Assessment). Each stage carries a type, max marks, pass marks, duration, and a mandatory/optional flag, and stages are reordered via drag-and-drop. Workflows can be activated/deactivated independently of their stages, and optionally linked to a job posting/vacancy so HR can indicate which assessment process a given opening will use.

## Why

EP-07 (Assessment & Evaluation Workflow, US-051–062) is entirely new ground for this system — no Assessment/Exam/Question entities existed before this feature. This is the foundational slice: every later EP-07 story (question bank, exam scheduling, seat plans, online auto-assessed exams, manual score upload, results/rankings) attaches to a workflow and its stages, so the workflow+stage data model and CRUD had to land first, ahead of anything that consumes it.

## Design notes

- **Reusable template, not 1:1 with a job posting.** Modeled on `HiringPipeline`'s precedent (reusable parent, `JobPosting.AssessmentWorkflowId` nullable FK, `DeleteBehavior.Restrict`) rather than `ShortlistFilter`'s (no stored FK at all) — US-051 AC5 explicitly requires "multiple workflows configured and assigned to different job types," which needs a real, queryable relationship.
- **`AssessmentWorkflowId` on `JobPosting` is optional**, unlike `HiringPipelineId` which is required on new job postings. This is a brand-new link with no existing business mandate to make every posting reference a workflow.
- **`StageTypeEnum` is a closed enum** (`WrittenTest`, `AptitudeTest`, `PsychometricTest`, `GroupDiscussion`, `PracticalAssessment`), stored as a string column — a deliberate departure from `PipelineStage.StageType`'s free-form string convention, since US-052's stage types are a fixed, known set rather than admin-defined text.
- **Manual activate/deactivate only (US-052 AC4 scope decision).** AC4 says "the workflow is activated when the job moves to the Assessment stage," but this codebase has no existing hook for that: domain events are declared on entities but never dispatched anywhere (dead code — `ApplicationDBContext.OnModelCreating` explicitly excludes them from persistence and nothing consumes them), and neither `JobStatusEnum` nor `ApplicationStatusEnum` has an "Assessment" concept. Building real event-driven auto-activation would mean adding dispatch plumbing from scratch — out of scope for this slice. Shipped instead: a manual `PATCH .../{id}/active` toggle mirroring `HiringPipelineSetActive`. Automatic activation is a follow-up story once pipeline-stage-progress is ready to drive it.
- **Whole-collection replace on update.** Editing a workflow clears and rebuilds its `Stages` collection from the request (renormalizing `DisplayOrder` to a clean 0..n sequence), the same approach `ShortlistFilterService`/`HiringPipelineService` use — no other entity references `AssessmentStageId` yet, so diffing add/edit/remove/reorder wasn't worth the complexity.
- **Delete guard.** Deleting a workflow still referenced by any `JobPosting` throws `ResourceInUseException` (409), same pattern as `HiringPipelineService.DeleteAsync`.
- **Drag-and-drop reuses `@angular/cdk/drag-drop`** (`DragDropModule`, `CdkDragDrop`, `moveItemInArray`, `cdkDropList`/`cdkDrag`/`cdkDragHandle`) exactly as `hiring-pipeline-management` already does — no new library introduced.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Domain/Entities/AssessmentWorkflow.cs`, `AssessmentStage.cs` — new.
- `Domain/Enums/Enum.cs` — added `StageTypeEnum`.
- `Domain/Entities/JobPosting.cs` — added nullable `AssessmentWorkflowId` FK + `AssessmentWorkflow?` nav.
- `Infrastructure/Configurations/AssessmentWorkflowConfiguration.cs`, `AssessmentStageConfiguration.cs` — new. `JobPostingConfiguration.cs` — added `AssessmentWorkflowId` index.
- `Application/Features/AssessmentWorkflows/` — new CQRS slice: `Commands/{Create,Update,Delete,SetActive}`, `Queries/{GetById,GetAll,GetActiveLookup}`, `Models/*`.
- `Application/Interfaces/Repositories/IAssessmentWorkflowRepository.cs`, `Infrastructure/Repositories/AssessmentWorkflowRepository.cs` — new.
- `Application/Interfaces/Services/IAssessmentWorkflowService.cs`, `Application/Services/AssessmentWorkflowService.cs` — new.
- `Application/Mappings/AssessmentWorkflowMapper.cs` — new (manual mapping, no AutoMapper, matching every other feature).
- `Application/Extensions/DependencyInjection.cs`, `Infrastructure/Extensions/DependencyInjection.cs` — registered the new service/repository.
- `Controllers/AssessmentWorkflowController.cs` — new, `recruitment/assessment-workflow` (GetAll, GetActiveLookup, GetById, Create, Update, Delete, SetActive).
- `Application/Features/JobPostings/Models/JobPostingCreateRequest.cs`/`UpdateRequest.cs`/`JobPostingResponse.cs`, `Application/Mappings/JobPostingMapper.cs` — added optional `AssessmentWorkflowId`/`AssessmentWorkflowName`.
- `Application/Services/JobPostingService.cs`, `Infrastructure/Repositories/JobPostingRepository.cs` — include `AssessmentWorkflow` nav alongside `HiringPipeline` in the existing GetById/GetAll/GetPaginated queries.
- `Migrations/20260717013237_AddAssessmentWorkflow.cs` — new tables `AssessmentWorkflows`/`AssessmentStages`, `JobPostings.AssessmentWorkflowId` column + FK (Restrict).
- `SylviaNG.Recruitment.Tests/Services/AssessmentWorkflowServiceTests.cs` — new unit tests (create/duplicate-name, update stage-replace, delete + delete-guard, set-active).

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/enums/recruitment.enum.ts` — added `StageTypeEnum`.
- `@core/interfaces/recruitment-management/assessment-workflow.interface.ts` — new.
- `@core/services/recruitment/assessment-workflow/assessment-workflow.service.ts` — new.
- `pages/assessment-workflow-management/` — new module: `assessment-workflow-management.module.ts`, `-routing.module.ts`, `assessment-workflow-list/*`, `manage-assessment-workflow/*` (incl. `.component.constants.ts` for `StageTypeOptions`/`newStage`/`StageTypeLabels`).
- `pages/pages-routing.module.ts` — lazy route for `assessment-workflow`.
- `@core/constants/nav-menu-items.ts` — sidebar entry "Assessment Workflows".
- `@core/interfaces/recruitment-management/job-vacancy.interface.ts`, `pages/job-vacancy-management/manage-job-vacancy/manage-job-vacancy.component.ts`/`.html` — added optional Assessment Workflow dropdown (populated via `getActiveLookup()`, no required validator).

## Verification

- `dotnet test` — 211/211 passing (7 new `AssessmentWorkflowServiceTests`, no regressions).
- `ng build` (development config) — compiles clean; `assessment-workflow-management` bundles as its own lazy chunk.
- End-to-end via Playwright against local Docker Postgres/Keycloak + `dotnet run` + `ng serve`, logged in as `abir`/HR: created a workflow with 3 stages, confirmed it appears in the list; toggled active/inactive without touching stages; edited the workflow (removed one stage, added another) and saved — stage count and data persisted correctly; created a job vacancy selecting the workflow via the new optional dropdown; confirmed the workflow list then shows "used by 1 job posting" and its Delete button is disabled (delete-guard); created and deleted a second, unlinked workflow successfully. No console errors during the run.
