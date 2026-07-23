# US-060 — View Assessment Results and Rankings

## What

The exam roster already showed `Score`/`IsPassed` per candidate (built in US-058/US-059). It now
also supports: sort by score/name (default score descending), a pass/fail filter, an Excel export
of the results, and a bulk action to move selected passing candidates to a HR-chosen hiring
pipeline stage.

## Why

Continuing the senior-approved EP-07 sequencing (55+56 → 58+59 → 57 → 60 → 61). Reuses the
existing roster page rather than a new one — the same table already had every column this story
needs, just no sort/filter/export/bulk-move.

## Scope decisions

- **No new Results page.** `exam-detail.component`'s existing roster `p-table` gained
  `pSortableColumn`/`p-sortIcon` on Name and Score (default `sortField="score"`,
  `sortOrder="-1"`), a client-side Pass/Fail dropdown filter over the already-loaded enrollments
  (trivial dataset size — no backend filter endpoint needed), and row selection.
- **"Sort by exam date" (AC3) is a no-op in practice** — this page is scoped to one exam, so every
  row shares the same date. Implementing it would mean turning this into a cross-exam results page,
  which is out of scope for a Size-S story reusing the existing per-exam roster.
- **No Exam-to-PipelineStage link exists** to auto-detect "the next stage" for AC5. Assessment
  stages were folded generically into `HiringPipeline`/`PipelineStage.MaxMarks` (see
  `merge-assessment-workflow-into-hiring-pipeline.md`) with no FK back from `Exam`. Consistent with
  `JobApplicationStageProgressService`'s existing manual-transition design (US-042: HR always picks
  the stage/status explicitly, no auto transition graph) — HR picks the target stage from a
  dropdown (the exam's job posting's hiring pipeline stages), and a new
  `BulkAdvanceToStageAsync(jobApplicationIds, pipelineStageId)` sets that stage to `InProgress` for
  each, provisioning missing progress rows first (same logic `GetByJobApplicationIdAsync` already
  uses to auto-provision on first fetch).
- **Passing-only enforced server-side.** The frontend disables selection checkboxes for
  non-passing rows, but `ExamEnrollmentService.BulkMoveToStageAsync` re-validates every given
  enrollment's `IsPassed` before delegating to the stage-progress bulk-advance, rejecting the whole
  batch if any isn't a pass.
- **Bug fixed during verification**: a freshly-provisioned `JobApplicationStageProgress` row (just
  added via `AddRangeAsync`/`AddAsync`, not yet saved) was also being passed to `Update()` in the
  same call, which EF Core rejects ("has a temporary value while attempting to change the entity's
  state to Modified" — you can't move an `Added` entity to `Modified` before it has a real
  identity-column value). Fixed by only calling `Update()` on rows that already existed before the
  call (`JobApplicationStageProgressId != 0`); a newly-added row's property changes ride along in
  its own pending `INSERT`.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Application/Interfaces/Services/IExamSeatPlanService.cs` +
  `Application/Services/ExamSeatPlanService.cs` — new `GenerateResultsExcelAsync(examId)`
  (ClosedXML, same shape as the existing `GenerateExcelAsync` seat-plan export).
- `Application/Interfaces/Services/IJobApplicationStageProgressService.cs` +
  `Application/Services/JobApplicationStageProgressService.cs` — new
  `BulkAdvanceToStageAsync(jobApplicationIds, pipelineStageId)` + private
  `EnsureProgressRowAsync` helper (provisions the full stage set if the application has none yet,
  or just the one missing target row if it already has others).
- `Application/Interfaces/Services/IExamEnrollmentService.cs` +
  `Application/Services/ExamEnrollmentService.cs` — new
  `BulkMoveToStageAsync(examId, examEnrollmentIds, pipelineStageId)` (passing-status validation,
  then delegates to the stage-progress service). New dependency on
  `IJobApplicationStageProgressService`.
- New CQRS: `Application/Features/ExamEnrollments/Queries/ExamResultsExportExcel/`,
  `Application/Features/ExamEnrollments/Commands/ExamResultsBulkMoveStage/`.
  `Application/Features/ExamEnrollments/Models/ExamResultsBulkMoveStageRequest.cs` — new.
- `Controllers/ExamController.cs` — `GET {examId}/results/export/excel`,
  `POST {examId}/results/bulk-move-stage`.
- `SylviaNG.Recruitment.Tests/Services/ExamSeatPlanServiceTests.cs` — new Excel-export test.
  `JobApplicationStageProgressServiceTests.cs` — new `BulkAdvanceToStageAsync` tests (provision,
  add-single-missing-row, update-existing-row, no-pipeline, unknown-stage).
  `ExamEnrollmentServiceTests.cs` — new `BulkMoveToStageAsync` tests (all-passing delegates
  correctly, one non-passing rejects the whole batch, unknown enrollment 404s).

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/interfaces/recruitment-management/exam-enrollment.interface.ts` — new
  `IExamResultsBulkMoveStageRequest`.
- `@core/services/recruitment/exam/exam.service.ts` — `downloadResultsExcel`,
  `bulkMoveResultsToStage`.
- `pages/exam-management/exam-detail/exam-detail.component.ts`/`.html` — Pass/Fail filter, "Export
  Results" button, sortable Name/Score columns (default score-descending), row selection
  (disabled for non-passing rows) + "Move to Stage" bulk-action bar. Resolves the exam's job
  posting's `hiringPipelineId` (`JobVacancyService`) then its stages
  (`HiringPipelineService.getById`) to populate the target-stage dropdown.

## Verification

- `dotnet test` — 510/516 passing (only the same 6 pre-existing unrelated failures as US-057).
- `ng build` — compiles clean.
- End-to-end via the browser (Playwright-driven Chromium): as HR, uploaded a score (75/100, pass
  marks 40) for the one enrolled candidate on "Written Aptitude Test," confirmed the Pass/Fail
  filter narrowed the roster to that row, clicked "Export Results" and got a valid `.xlsx`
  download. Selected the passing candidate's checkbox, picked a target stage ("Screening") from
  the dropdown, clicked "Move to Stage" — first attempt hit a 500 (the EF tracking bug described
  above); after the fix, the same action succeeded with a "Moved 1 candidate(s) to the selected
  stage" toast, and the backend log showed 3 `JobApplicationStageProgress` rows correctly
  `INSERT`ed (the full active-stage set, auto-provisioned since the candidate had none yet) with no
  exception. Re-ran the full backend test suite after the fix to confirm no regressions.
