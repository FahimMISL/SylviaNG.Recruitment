# Merge Assessment Workflow into Hiring Pipeline

## What

Assessment Workflow (`AssessmentWorkflow`/`AssessmentStage`, EP-07 US-051/US-052) has been removed as a separate feature. Assessment is now just another kind of `PipelineStage` inside a `HiringPipeline`: a stage that happens to be an assessment (Written Test, Aptitude Test, etc.) simply has `MaxMarks`/`PassMarks` filled in, and reuses the existing `EstimatedDurationMinutes` field for exam duration. Every other stage leaves those two fields null. `JobPosting.AssessmentWorkflowId` is gone — a job posting now references only `HiringPipelineId`.

## Why

Hiring Pipeline and Assessment Workflow were two nearly-identical CRUD verticals: same reusable-template pattern, same drag-and-drop stage builder, same whole-collection-replace/delete-guard service logic — just duplicated across two entities, two tables, two top-level menu items, and two unrelated dropdowns on the job vacancy form. `AssessmentStage` was already a strict subset of `PipelineStage`'s fields with zero functional coupling elsewhere (no candidate-progress tracking, no exam-question wiring). The duplication added configuration complexity for HR/Admin with no corresponding benefit, so the two were folded into one.

## Design notes

- **`PipelineStage.StageType` stays free-text**, not a closed enum. Converting it to a closed enum (matching the old `StageTypeEnum`) would have regressed the deliberate "admins can define custom stage types without a code change" flexibility `PipelineStage` already had. Instead, the 4 assessment type names not already covered (`WrittenTest`, `AptitudeTest`, `PsychometricTest`, `PracticalAssessment`) were added to `PipelineStageTypes.Suggested` as extra suggestions (`GroupDiscussion` was already present).
- **No "is this an assessment" flag.** `MaxMarks`/`PassMarks` are just two more optional fields shown on every stage card in the pipeline builder — HR fills them in only for stages that are actually assessments, leaves them blank otherwise. Deliberately no categorization mechanism, to keep the merge simple.
- **`DurationMinutes` was not re-added as a new field.** The old `AssessmentStage.DurationMinutes` is functionally the same thing as `PipelineStage.EstimatedDurationMinutes` (both minutes, both describing how long the stage takes) — reused instead of duplicated.
- **New validation**: if both `MaxMarks` and `PassMarks` are set on a stage, `PassMarks` must be `<= MaxMarks` (mirrors the old `AssessmentStage` client-side check), enforced in both `HiringPipelineCreateValidator`/`HiringPipelineUpdateValidator` and the frontend `stageIsInvalid()`.
- **`StageTypeEnum` was deleted** — its only consumers were `AssessmentStage`/`AssessmentStageRequest`/`AssessmentStageResponse`, all removed in this change.
- **No data-preservation migration.** This was dev/demo-stage data with no production users. The migration (`MergeAssessmentWorkflowIntoHiringPipeline`) drops `AssessmentWorkflows`/`AssessmentStages` and `JobPostings.AssessmentWorkflowId` outright — a job posting that had both a `HiringPipelineId` and a separate `AssessmentWorkflowId` cannot be auto-merged into one coherent stage order without human judgment anyway, so no script attempts it.
- Deleted entirely: `AssessmentWorkflow`/`AssessmentStage` entities, EF configs, repository, service, mapper, controller, the whole `Application/Features/AssessmentWorkflows/` CQRS slice, the frontend `assessment-workflow-management` module, its nav-menu entry and route, and the second dropdown on the "manage job vacancy" form.

## Files touched

- `Domain/Entities/PipelineStage.cs` — added `MaxMarks`/`PassMarks`.
- `Domain/Entities/JobPosting.cs` — removed `AssessmentWorkflowId`/`AssessmentWorkflow` nav.
- `Domain/Enums/Enum.cs` — removed `StageTypeEnum`.
- `Application/Common/Constants/PipelineStageTypes.cs` — added 4 assessment type suggestions.
- `Application/Mappings/HiringPipelineMapper.cs`, `Application/Services/HiringPipelineService.cs` (`DuplicateAsync` clone), `Application/Features/HiringPipelines/Models/PipelineStageRequest.cs`/`PipelineStageResponse.cs`, `HiringPipelineCreateValidator.cs`/`HiringPipelineUpdateValidator.cs` — `MaxMarks`/`PassMarks` passthrough + cross-field rule.
- `Application/Mappings/JobPostingMapper.cs`, `Application/Features/JobPostings/Models/*` — removed `AssessmentWorkflowId`/`AssessmentWorkflowName`.
- Migration `MergeAssessmentWorkflowIntoHiringPipeline`.
- Frontend: `hiring-pipeline.interface.ts`, `manage-hiring-pipeline.component.{ts,html,constants.ts}`, `manage-job-vacancy.component.{ts,html}`, `nav-menu-items.ts`, `pages-routing.module.ts`. Deleted `assessment-workflow-management/` module, its interface, and its service.
