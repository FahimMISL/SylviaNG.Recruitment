# US-054 — Bulk Import Exam Questions

## What

A downloadable XLSX template and a bulk-import flow on the Exam Questions list page: HR picks a target Question Group and uploads an XLSX or CSV file, the backend validates every row, imports the valid ones, and returns a summary (total/imported/failed counts plus a per-row error message for anything skipped).

## Why

Builds directly on US-053's question bank — authoring questions one at a time doesn't scale for a real exam bank of dozens/hundreds of questions. US-054 (Should Have, size M) closes that gap.

## Design notes

- **Reuses `DocumentFormat.OpenXml`**, already a dependency (used elsewhere by `ResumeParsingService` for `.docx` parsing) — no new NuGet package. Template generation uses `SpreadsheetDocument.Create` with `InlineString` cells (no shared-string-table plumbing needed for writing); reading resolves cells through `WorkbookPart.SharedStringTablePart` where the source file uses shared strings.
- **`.csv` support** (also named in AC1) is a small hand-rolled, quote-aware line splitter — not worth a second dependency for one format.
- **Target group is chosen once in the UI**, not per-row — a required dropdown next to the file picker, applied to the whole batch. Simpler than a per-row `QuestionGroupName` column and its own group-not-found error case; the template has 14 columns already (`QuestionText, QuestionType, DifficultyLevel, Marks, Option1Text..Option4IsCorrect, Explanation, ModelAnswer`).
- **Row-level validation mirrors `ExamQuestionCreateValidator`'s type-shape rules** (MCQ needs ≥2 options, single needs exactly 1 correct, True/False needs exactly the 2 named options with 1 correct, Subjective needs none) but is implemented separately in `ExamQuestionImportService.ParseRow` since it operates on raw spreadsheet strings rather than a typed request — same conceptual rules, different code path by necessity.
- **Valid rows are batched**: the whole file is parsed first, then all valid rows are saved in one `AddRangeAsync` + one `SaveChangesAsync` (AC3 — bad rows never block good ones, and a bad row's error message concatenates all its problems into one entry keyed by the Excel row number, header = row 1).
- **Template download is genuinely new backend territory** — no prior `File()`/`FileContentResult` precedent existed anywhere in this codebase; `JobPostingAttachment` only ever returned a `downloadUrl` string. `GET /recruitment/exam-question/import-template` returns `IActionResult` via `File(bytes, contentType, fileName)`.
- **Frontend blob download is also new territory** — no prior "fetch bytes + trigger browser save" utility existed; added via `responseType: 'blob'` + `URL.createObjectURL` + a temporary `<a download>` click, no new npm package (no `file-saver` in this project).
- Bulk import is a dialog on the Exam Questions list page, not a separate route — a single-shot transient action, not a persistent record worth its own URL/breadcrumb.
- **`QuestionType` accepts a small alias table alongside the raw enum name** (`ExamQuestionImportService.QuestionTypeAliases`) — HR filling the sheet by hand naturally types what they see in the "Add Question" form's dropdown ("MCQ (Single Correct)") rather than the enum name ("McqSingle") the Instructions sheet documents. Found via a real filled template during manual testing; both forms are now accepted.

## Bugs found and fixed during manual testing

Three real, non-obvious bugs surfaced only once a real HR user tried the actual download → fill → upload round-trip in a browser, all now fixed:

1. **Binary responses were being corrupted by a pre-existing, unrelated backend middleware.** `Middlewares/ResponseWrappingMiddleware.cs` unconditionally captured every 2xx response body, read it as UTF-8 text, and re-serialized it as JSON (this is how the frontend's `ApiResponse<T>` wrapper gets applied — it's in-process middleware, not an external gateway as originally assumed while planning). For `GET import-template`'s binary XLSX body, reading it as text and rewrapping it as JSON corrupted the bytes and desynced `Content-Length` from what was actually written, surfacing to the browser as a raw network error. Fixed by skipping the wrap when the response carries a `Content-Disposition` header or a non-JSON/non-text `Content-Type` — passes the bytes through unchanged. This is a general fix (any future file-download endpoint benefits), not something specific to this feature, but it was only found because this feature was the first to add one.
2. **`ExamQuestionImportService.GetCellText` never handled inline-string cells.** `GenerateTemplate()` writes header cells as OpenXml inline strings (`t="inlineStr"`), and Excel preserves that same cell type for data rows edited in place rather than converting them to the shared-string table. `GetCellText` only ever checked `cell.CellValue` (populated for numbers/booleans and shared-string references) — inline-string cells have no `CellValue` at all, so every text column (`QuestionText`, `QuestionType`, `DifficultyLevel`, options, `Explanation`, `ModelAnswer`) silently read as empty while numeric/boolean columns (`Marks`, `Option*IsCorrect`) worked fine. This produced "field is required" errors on every row of a correctly-filled real template. Fixed by checking `cell.DataType == CellValues.InlineString` first and reading `cell.InlineString.Text.Text`. Confirmed against a real user-filled template downloaded from this feature and re-uploaded: 10/10 rows imported after the fix, 0/10 before.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Application/Common/Settings/ExamQuestionImportSettings.cs` — new file-size/extension policy options, bound from the new `ExamQuestionImport` `appsettings.json` section.
- `appsettings.json` — new `ExamQuestionImport` section.
- `Infrastructure/Extensions/DependencyInjection.cs` — `services.Configure<ExamQuestionImportSettings>(...)`.
- `Application/Interfaces/Services/IExamQuestionImportService.cs` + `Application/Services/ExamQuestionImportService.cs` — template generation (`GenerateTemplate`) and import parsing (`ImportAsync`, XLSX + CSV row readers, per-type validation, `ExamQuestionMapper.ToEntity` reuse for valid rows).
- `Application/Features/ExamQuestions/Models/ExamQuestionBulkImportRowError.cs`, `ExamQuestionBulkImportResponse.cs`, `ExamQuestionImportTemplateResponse.cs` — new DTOs.
- `Application/Features/ExamQuestions/Commands/ExamQuestionBulkImport/{Command,Handler,Validator}.cs` — new command (carries `QuestionGroupId` + `IFormFile`, mirrors `JobPostingAttachmentUploadCommand`'s `IFormFile` handling).
- `Application/Features/ExamQuestions/Queries/ExamQuestionImportTemplateDownload/{Query,Handler}.cs` — new query.
- `Controllers/ExamQuestionController.cs` — `GET import-template`, `POST bulk-import` endpoints (shared file with US-053's CRUD endpoints on the same controller).
- `Application/Extensions/DependencyInjection.cs` — `IExamQuestionImportService` registration.
- `Middlewares/ResponseWrappingMiddleware.cs` — fixed to pass binary/file responses through unchanged instead of corrupting them (see Bugs found below). Pre-existing file, general-purpose fix, not new to this feature.
- `SylviaNG.Recruitment.Tests/Services/ExamQuestionImportServiceTests.cs` — new unit tests (valid rows, missing-field row, no-correct-option row, mixed valid/invalid batch, unknown group, template round-trip).
- `SylviaNG.Recruitment.Tests/Validators/ExamQuestionBulkImportValidatorTests.cs` — new validator tests (file presence/extension/size, missing group).

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/interfaces/recruitment-management/exam-question.interface.ts` — `IExamQuestionBulkImportRowError`, `IExamQuestionBulkImportResponse` (shared file with US-053's question DTOs).
- `@core/services/recruitment/exam-question/exam-question.service.ts` — `downloadImportTemplate()` (blob), `bulkImport()` (FormData, mirrors `job-vacancy-attachment.service.ts`'s upload pattern).
- `pages/exam-question-management/exam-question-list/exam-question-list.component.ts` + `.html` + `.scss` — "Download Template" button, "Bulk Import" button opening a `p-dialog` (group picker, file input, submit, results summary table of row errors). Bulk-import file picker uses the existing `.upload-dropzone` global utility class (icon + label, matches `manage-job-vacancy`'s attachment upload) rather than a one-off plain box. Dialog action buttons got `pb-3` after the bottom edge looked cramped against the dialog border in testing.

## Verification

- `dotnet test` — included in the same 232/232 pass as US-053 (shared test run; import-specific tests listed above, covering valid/invalid/mixed rows, unknown group, and a template-generation round-trip through `SpreadsheetDocument.Open` to catch column drift between the writer and the parser).
- `ng build` compiles clean (part of the same build as US-053, same module).
- Full manual browser round-trip completed against Docker Postgres/Keycloak + `dotnet run` + `ng serve`: downloaded the template, confirmed it opens as a genuine XLSX with the expected 14 columns across a `Questions` and an `Instructions` tab; a real user filled 10 rows by hand and uploaded via the UI, which surfaced bugs 1 and 2 below (both fixed and reverified via a direct API call against the same real file: 10/10 imported, 0 errors).
