# US-058 — Candidate Completes an Online Auto-Assessed Exam + US-059 — Upload Manual Assessment Scores

## What

A candidate enrolled in an Online exam can start it from **My Applications**, answer MCQ (single/multiple), True/False, and Subjective questions within a countdown timer, and submit once. MCQ/True-False are auto-scored on submission (exact-set-match, all-or-nothing per question); Subjective answers are stored but left ungraded, flagged for HR review. Separately, HR can upload/override a candidate's score for any exam (auto-scored or offline) — a single-row form or a bulk XLSX/CSV upload against a per-exam template — which finalizes Subjective-containing exams and is the sole scoring path for in-person exams.

## Why

Bundled per senior-approved EP-07 sequencing (55+56 → **58+59** → 57 → 60 → 61) — both stories write the same `ExamEnrollment` fields (`Score`/`IsPassed`/scoring metadata), just from two different writers (the candidate's own auto-scored submission vs. HR's manual upload), so reviewing them together avoided splitting one coherent data model across two PRs. `Exam.QuestionGroupId` and the whole question bank (US-053/054) existed specifically as a forward reference for this slice.

## Scope decisions

- **Attempt status is derived, not stored.** `NotStarted`/`InProgress`/`Submitted` comes from `ExamEnrollment.StartedAt`/`SubmittedAt` nullability (computed in `ExamEnrollmentMapper`), not a persisted enum — same "no redundant state" instinct as US-055/056's seat-plan-generated marker.
- **All-or-nothing scoring, no partial credit.** McqMultiple awards full marks only if the selected option set exactly equals the correct set; no negative marking, no per-option partial scoring. Simplest defensible default given no AC specifies partial credit.
- **Subjective questions are never auto-scored.** `ExamAnswer.IsCorrect`/`MarksAwarded` stay null for Subjective; HR finalizes via the same US-059 manual-score-upload path (`ScoreSource` flips from `AutoScored` to `ManualUpload`, overwriting the provisional auto-computed total). No separate "grade this subjective answer" UI was built — out of scope, HR reviews the stored `AnswerText` off-system today and uploads a final number.
- **Single final submission, one page.** No sections/pagination concept exists on the data model, so AC4 ("cannot revisit closed sections") is satisfied trivially by rendering every question on one scrollable page; "cannot re-submit" is enforced server-side (`SubmittedAt != null` guard) regardless of what the frontend shows.
- **No server-side auto-submit job.** The client calls the same `submit` endpoint when its countdown timer hits zero as the manual "Submit" button does — matches the "no background job infrastructure exists" precedent from US-055/056's notification handling.
- **`ScoredByUserName` (string), not a numeric HR user id.** Keycloak's JWT carries no parseable numeric user-id claim (`JobPostingService.TryGetCurrentUserId` already documents this as "typically null"); reused `ICurrentUserService.GetCurrentUserName()`'s established attribution precedent (`ApplicationStatusHistory.ChangedByUserName`) instead, which actually resolves under this codebase's auth.
- **`Exam.ShowResultsToCandidate`** (new bool, default false, set at exam creation) gates AC6 — a submission confirmation + reference number (`EXM-{examId}-{enrollmentId}`) is always returned; Score/IsPassed are only included when this flag is on.
- **Score-upload template reuses `ExamQuestionImportSettings`** (US-054's file-size/extension policy config) rather than a new settings section — identical XLSX/CSV upload concern, no behavioral difference.
- **`ExamAnswer` is a new, genuine child entity** (not scalar columns on `ExamEnrollment`) — one row per answered question is a real 1-to-many, same precedent as `ExamQuestionOption`.
- **Out of scope, deliberately not built:** admit-card distribution / candidate-portal download (US-057, comes after this in the sequencing), a ranked results/rankings page or Excel export (US-060 — this bundle only adds Score/Pass/Scored-by columns to the existing HR roster), any `ApplicationStatus` auto-transition on pass/fail (no AC requires it, no existing hook), a dedicated subjective-answer grading UI, partial-progress autosave across a lost connection.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Domain/Entities/ExamEnrollment.cs` — added `StartedAt`, `SubmittedAt`, `Score`, `IsPassed`, `ScoreSource`, `ScoredAt`, `ScoredByUserName`, `Answers` nav.
- `Domain/Entities/Exam.cs` — added `ShowResultsToCandidate`.
- `Domain/Entities/ExamAnswer.cs` — new entity.
- `Domain/Enums/Enum.cs` — added `ScoreSourceEnum`.
- `Infrastructure/Configurations/ExamEnrollmentConfiguration.cs` — new column mappings; `ExamAnswerConfiguration.cs` — new.
- `Infrastructure/Data/ApplicationDBContext.cs` — added `ExamAnswers` DbSet.
- `Migrations/20260723035530_AddExamTakingAndScoring.cs` — new migration (additive only — 8 `AddColumn` + 1 `CreateTable`).
- `Application/Interfaces/Repositories/IExamQuestionRepository.cs`/`ExamQuestionRepository.cs` — added `GetActiveByQuestionGroupIdAsync`. `IExamEnrollmentRepository.cs`/`ExamEnrollmentRepository.cs` — added `GetByCandidateProfileIdAsync`, `GetByIdWithExamAndQuestionsAsync`. `IExamAnswerRepository.cs`/`ExamAnswerRepository.cs` — new.
- `Application/Interfaces/Services/IExamTakingService.cs` + `Application/Services/ExamTakingService.cs` — new (list/start/submit, scoring logic, ownership checks).
- `Application/Interfaces/Services/IExamScoreImportService.cs` + `Application/Services/ExamScoreImportService.cs` — new (US-054's `ExamQuestionImportService` pattern, mirrored for score templates).
- `Application/Services/ExamEnrollmentService.cs`/`IExamEnrollmentService.cs` — added `UploadScoreAsync` (single-row path).
- `Application/Features/ExamTaking/**` — new CQRS slice (`Queries/ExamTakingGetMyEnrollments`, `Commands/ExamTakingStart`, `Commands/ExamTakingSubmit`, `Models/*`).
- `Application/Features/ExamEnrollments/Commands/ExamScoreUpload/`, `Commands/ExamScoreBulkUpload/`, `Queries/ExamScoreImportTemplateDownload/`, `Models/ExamScoreUploadRequest.cs`, `ExamScoreBulkUploadResponse.cs`, `ExamScoreImportTemplateResponse.cs` — new.
- `Application/Features/ExamEnrollments/Models/ExamEnrollmentResponse.cs`, `Application/Mappings/ExamEnrollmentMapper.cs` — extended with scoring/attempt fields.
- `Application/Features/Exams/Models/ExamCreateRequest.cs`/`ExamResponse.cs`, `Application/Mappings/ExamMapper.cs` — added `ShowResultsToCandidate`.
- `Controllers/ExamTakingController.cs` — new, `recruitment/exam-taking`, `[Authorize(Roles="Candidate")]`.
- `Controllers/ExamController.cs` — added `score-upload-template`, `score-upload/bulk`, `enrollments/{id}/score` endpoints (Admin/HR only, existing class-level attribute).
- `Application/Extensions/DependencyInjection.cs`, `Infrastructure/Extensions/DependencyInjection.cs` — registered new services/repositories.
- `SylviaNG.Recruitment.Tests/Services/ExamTakingServiceTests.cs`, `ExamScoreImportServiceTests.cs` — new. `ExamEnrollmentServiceTests.cs` — extended for `UploadScoreAsync` + constructor update.

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/enums/recruitment.enum.ts` — added `ScoreSourceEnum`, `ExamAttemptStatusEnum`.
- `@core/interfaces/recruitment-management/exam-taking.interface.ts` — new. `exam-enrollment.interface.ts`, `exam.interface.ts` — extended.
- `@core/services/recruitment/exam-taking/exam-taking.service.ts` — new. `exam/exam.service.ts` — added score-upload/template/bulk methods.
- `pages/exam-attempt/` — new module (no `RoleGuard`, matching `my-applications`'s "any authenticated role, backend enforces ownership" convention): single component with a 3-phase state machine (instructions → in-progress question form with countdown timer → submitted/result), all questions on one page.
- `pages/pages-routing.module.ts` — added `exam-attempt` lazy route.
- `pages/my-applications/my-applications.component.ts`/`.html`/`.scss` — added an "Exams" section per application card (Start/Resume/View Status for Online, info-only for In-Person), fetched via a separate `ExamTakingService` call alongside the existing applications call.
- `pages/exam-management/exam-detail/exam-detail.component.ts`/`.html` — added Score/Pass/Scored-by roster column, per-row "Upload Score" dialog, "Bulk Upload Scores" dialog (copied from `exam-question-list`'s bulk-import dialog pattern), "Download Score Template" button.
- `pages/exam-management/schedule-exam/schedule-exam.component.ts`/`.html`, `exam-management.module.ts` — added "Show results to candidate immediately" checkbox (Online exams only), `CheckboxModule`.

## Verification

- `dotnet test` — 494/500 passing (26 new across `ExamTakingServiceTests`, `ExamScoreImportServiceTests`, `ExamEnrollmentServiceTests` additions, no regressions). The 6 pre-existing failures are unrelated and unchanged from before this feature: 3× `InternalJobBoardControllerTests` (known `ControllerContext.User` NRE gap) and 3× `AuthLoginSmokeTests` (need a live Keycloak realm, environmental).
- `dotnet ef migrations add AddExamTakingAndScoring` — additive only, verified after a false start: the first attempt (and a from-scratch sanity check on an unmodified `demo` tree) both produced a spurious full-schema rebuild; root cause was a stale compiled `ModelSnapshot` type surviving in `obj/`/`bin/` across branch switches (the snapshot source file is gitignored, so a stale `obj/`/`bin/` build artifact silently outlives it). Fixed with a full `rm -rf obj bin` + `dotnet restore` before regenerating — this is a sharper form of the "clean bin/obj before first EF migration" gotcha already on file, worth remembering as `obj`/`bin` staleness, not just the snapshot file itself. `dotnet ef database update` applied cleanly against local Postgres.
- `ng build` (development config) — compiles clean; `exam-attempt-module` (37.84 kB) is its own lazy chunk alongside the updated `exam-management-module`.
- End-to-end via direct API calls against local Docker Postgres/Keycloak + `dotnet run` (no browser-automation tool was available in this environment, so this substituted for a UI click-through; every request/response shape was cross-checked against the TypeScript interfaces field-for-field): registered a fresh Candidate account, applied to a fee-free job posting, HR moved it to Shortlisted, HR created an Online exam (`ShowResultsToCandidate=true`) against `QuestionGroupId=2` (5 active questions) and enrolled the application. As the candidate: fetched `GET exam-taking/enrollments` (status `NotStarted`), `POST .../start` (paper returned with no `IsCorrect` on any option, correct `DeadlineAt`), `POST .../submit` with 2 correct MCQ, 1 wrong, 1 unanswered, 1 correct True/False — scored `7.00/12` exactly as hand-computed, `IsPassed=true`, `resultsVisible=true`, reference `EXM-2-2`. Re-submitting the same enrollment correctly returned 400 "already been submitted." As HR: roster (`GET exam/2/enrollments`) showed the auto-score with `scoreSource=AutoScored`; single-row `PATCH .../score` overrode it to `ManualUpload` with `scoredByUserName=abir`; a score above `TotalMarks` was correctly rejected (400); downloaded the score-upload template (valid XLSX, 200 OK); bulk CSV upload of 3 rows (1 valid, 1 unknown-enrollment-id, 1 over-cap) returned `updatedCount=1, failedCount=2` with the expected per-row error messages.
