# US-045 — Advanced Boolean Search Across CV Bank

## What

A CV Bank search screen for HR/Admin that runs Boolean queries (`AND`/`OR`/`NOT`, parentheses, `"quoted phrases"`, implicit `AND` between bare terms) across every active candidate profile's fields *and* their submitted resume text, with optional structured filters (minimum education level, experience range, location, candidate type), relevance-ranked results, and a bulk "Add to Talent Pool" action on the result set. A small Talent Pool list page lets HR view/remove what's been collected.

## Why

HR could previously only browse candidates via a plain name/email search (US-009). US-045 (Should Have, size L) asks for real recruiter-grade search: Boolean operators, full CV content, and filters, so HR can proactively mine the whole candidate pool for any role rather than only reviewing people who already applied to a specific vacancy.

## Design notes

- **Branch-content correction.** Initial exploration happened on `fix/ai-resume-parsing` (not yet merged into `dev`), which has a richer resume pipeline (`CandidateDocumentTypeEnum.Resume`, persisted resume documents) that `dev` doesn't have. On `dev`, a candidate's actual CV file lives on `JobApplication.ResumeUrl` (set in `JobApplicationService.SubmitAsync`), not on the candidate profile. CV text extraction hooks there instead of on `CandidateDocument`.
- **No "CV content" text was persisted anywhere before this story.** Added `JobApplication.ResumeExtractedText` (nullable, `text`), populated best-effort (try/catch, never fails the submission) at submit time via a new `IResumeParsingService.ExtractRawTextAsync` — refactored out of `ResumeParsingService.ParseAsync`'s existing PDF/DOCX dispatch so both share one extraction path.
- **"Candidate type" filter has no backing field on `CandidateProfile`.** Per product decision, it's derived from the candidate's `JobApplication.Source` history (Internal/External/Admin), matched by email, batch-loaded once (no N+1). A candidate who's never applied simply won't match any candidate-type filter value — documented behavior, not a bug.
- **"Talent pool" didn't exist.** Added a minimal `CandidateTalentPool` table (candidate + audit columns) as a flat "saved for later" bucket. The "or shortlist" half of the original AC5 wording was deliberately dropped — real shortlisting requires an existing application to a specific vacancy, which CV Bank candidates may not have.
- **In-memory, full-scan evaluation — same precedent as `ShortlistFilterEvaluationService`.** The candidate pool is bounded, so `CvBankSearchHandler` loads every active profile (`ICandidateProfileRepository.GetAllActiveWithDetailsAsync`), builds `CandidateFactService.BuildFacts` (reused as-is) plus a haystack string per candidate, and evaluates Boolean queries with plain case-insensitive `Contains` — consistent with the existing `PaginationExtensions.ApplySearch` semantics elsewhere in the app. No SQL-side Boolean translation.
- **Malformed queries are a 400, not a 500.** `BooleanQueryParser.Parse` throws `FormatException`/`ArgumentException` on bad syntax (unbalanced parens, etc); `CvBankSearchHandler` wraps these into `FluentValidation.ValidationException` so `GlobalExceptionHandlerMiddleware`'s existing mapping turns them into a clean 400 with the parser's message, instead of falling through to the generic 500 handler. Caught during manual verification, not in the original draft.
- **Migration hygiene note.** The first `dotnet ef migrations add` run picked up stale `bin`/`obj` build artifacts from a previous `fix/ai-resume-parsing`-era build and generated spurious `DropTable` statements for unrelated `AutoShortlistRuns`/`AutoShortlistResults` tables that have no corresponding entities anywhere in this checkout. Caught before committing; fixed by cleaning `bin`/`obj` across the solution and regenerating — the committed migration only contains `ResumeExtractedText` + `CandidateTalentPools`.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Domain/Entities/JobApplication.cs` — new `ResumeExtractedText`; `Domain/Entities/CandidateTalentPool.cs` — new entity.
- `Infrastructure/Configurations/JobApplicationConfiguration.cs`, `CandidateTalentPoolConfiguration.cs` (new) — column/index mapping.
- `Migrations/20260716041223_AddCvBankSearchSupport.cs` — `ResumeExtractedText` column + `CandidateTalentPools` table.
- `Application/Interfaces/Services/IResumeParsingService.cs` / `Infrastructure/Services/ResumeParsingService.cs` — new `ExtractRawTextAsync`, `ParseAsync` refactored to reuse it.
- `Application/Services/JobApplicationService.cs` — `SubmitAsync` best-effort extracts + persists `ResumeExtractedText`.
- `Application/Interfaces/Repositories/ICandidateProfileRepository.cs` / `Infrastructure/Repositories/CandidateProfileRepository.cs` — new `GetAllActiveWithDetailsAsync`.
- `Application/Interfaces/Repositories/ICandidateTalentPoolRepository.cs` / `Infrastructure/Repositories/CandidateTalentPoolRepository.cs` — new.
- `Application/Common/BooleanQuery/BooleanQueryParser.cs` — new recursive-descent Boolean query parser/evaluator.
- `Application/Features/CvBank/` — new feature folder: `Models/` (search request/result, talent-pool add/entry DTOs), `Queries/CvBankSearch`, `Queries/CvBankTalentPoolGetAll`, `Commands/CvBankTalentPoolAdd`, `Commands/CvBankTalentPoolRemove`.
- `Controllers/CvBankController.cs` — new, `recruitment/cv-bank`, `Admin,HR` only.
- `Infrastructure/Extensions/DependencyInjection.cs` — registers `ICandidateTalentPoolRepository`.
- Tests: `BooleanQueryParserTests.cs`, `CvBankSearchHandlerTests.cs`, `CvBankTalentPoolAddHandlerTests.cs` (new), `JobApplicationServiceTests.cs` (extraction success/failure cases added, constructor updated).

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/interfaces/recruitment-management/cv-bank.interface.ts`, `@core/services/recruitment/cv-bank/cv-bank.service.ts` — new.
- `pages/cv-bank-management/` — new module: `cv-bank-search/` (query box, filter row, `p-table` with checkbox selection + relevance column, bulk "Add to Talent Pool"), `talent-pool-list/` (list + remove with confirm dialog).
- `pages/pages-routing.module.ts`, `@core/constants/nav-menu-items.ts` — new `/cv-bank` route + nav entry (Admin/HR).

## Verification

- `dotnet test` — 233/233 passing (includes all new US-045 tests, no regressions).
- `ng build` — compiles clean; the pre-existing 1MB initial-bundle budget failure is unrelated (confirmed via `git stash` + rebuild on the pre-change tree — identical failure/size both with and without this feature).
- End-to-end against local Docker Postgres + `dotnet run`, real seeded data (11 candidate profiles), logged in as the hardcoded Admin fallback account: applied the new migration live; confirmed a quoted-phrase query (`"data science"`) matches only the one candidate with that in their education; an `OR` query matches multiple; `educationLevel=Master` isolates the one Master's-degree holder; a malformed query (`(react AND node`) returns 400 with a helpful message (not 500 — bug found and fixed during this pass); Talent Pool add correctly reports added vs. already-in-pool counts on a repeat add, list/remove both work.

## Known gaps

- `CandidateTalentPool.CreatedAt` (and every other entity's `Audit.CreatedAt` in this app) is never auto-stamped — there's no global SaveChanges hook that sets it, only a UTC-normalization interceptor. Pre-existing, app-wide gap, not introduced by this story; the Talent Pool list's "Added" column will show blank until that's addressed separately.
- Candidate-type filter only matches candidates who have at least one `JobApplication` on record (see design notes above).
