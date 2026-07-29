# US-046 — AI-Powered Auto-Shortlisting

## What

Ranks every applicant for a vacancy against the job posting's description/requirements, giving each a 0-100 score with a human-readable explanation (AC1/AC2). HR sets a cutoff score that auto-marks candidates pass/fail (AC3/AC4), can adjust the cutoff or override any individual decision after reviewing the ranked list (AC5), then applies the final decisions to move passing candidates to Shortlisted.

## Why

Must Have, Size L. Lets HR shortlist the top-N candidates for a vacancy without manually reviewing every CV.

## Design notes

- **One interface, two swappable implementations, one config value.** Per the senior's explicit architecture decision: `IShortlistScoringService` has `ManualShortlistScoringService` (deterministic, no external calls) and `AiShortlistScoringService` (Groq). `ShortlistScoring:Provider` = `"Manual"` | `"Ai"` picks one via a factory lambda in `Infrastructure/Extensions/DependencyInjection.cs`, the exact same shape as this codebase's existing `Database:Provider` switch. Flip one config value, no code change — verified live both directions this session.
- **Groq for the AI side** (user's choice — free tier). Client (`GroqClient`/`GroqSettings`) mirrors `KeycloakClient`/`KeycloakSettings` exactly: typed `HttpClient`, `IOptions<T>`, manual `JsonSerializer`/`JsonDocument`, custom `GroqUnavailableException` wrapping network failures, one retry on HTTP 429. API endpoint/model names verified live against Groq's own docs at build time (not from training-data memory) — `POST https://api.groq.com/openai/v1/chat/completions`, model `llama-3.1-8b-instant` by default.
- **Persisted runs, not transient responses.** `AutoShortlistRun` (+ child `AutoShortlistResult` rows) persists every "Run" so HR can revisit the ranked list, adjust the cutoff, or override decisions without re-scoring — re-running always creates a fresh run rather than mutating an old one. `CutoffScore` lives only on the run; `Passed`/`FinalIncluded` are computed at response-mapping time, never stamped per row, so adjusting the cutoff is an O(1) update.
- **One Groq call per candidate**, bounded concurrency (`SemaphoreSlim`, default 5 concurrent) — isolates a bad call to one candidate instead of risking a multi-candidate batch response the model could truncate or conflate. A per-candidate scoring failure never throws out of `ScoreAsync`; it's recorded as a failed result row, surfaced in the UI, and HR can still manually approve it via override.
- **No candidate profile → explicit failed row, not a silent skip.** Deliberate deviation from `ShortlistFilterEvaluationService`'s existing precedent (which silently drops unmatched applications) — AC1 says "each application," so a missing profile becomes `ScoringFailed=true` with a clear reason, never omitted from the list. Caught this during plan review: `CandidateFactService.BuildFacts(null)` returns a normal-shaped, all-empty `CandidateFacts`, not a detectable sentinel, so the missing-profile check happens in the orchestrator (which already has the raw `profile` from its join step) before the scorer is ever called — not something either scorer implementation could detect on its own.
- **Manual scorer**: 100-point weighted formula (Education 25 / Experience 25 / Skills 30 / Location 10 / Age 10), reusing `CandidateFactService.BuildFacts` — the same shared fact-deriver US-043/044/050 already use. A criterion the job posting doesn't specify scores as neutral (full points), not zero — "no requirement" isn't evidence against a candidate.
- **Closes a documented gap**: `ShortlistFilterCriterion.MinScreeningScore` existed in the schema/UI since US-043 but always evaluated `false`, with a code comment explicitly deferring to "AI resume screening is a separate, unbuilt story." `ShortlistFilterEvaluationService` now pulls the latest `AutoShortlistRun` score per application and evaluates the criterion for real. Verified live: threshold below the persisted score passes, threshold above it fails.
- **Migration hygiene**: generating the migration on this branch (cut from `dev`, which doesn't have the still-unmerged US-048/US-049 branches) scaffolded a spurious `DropTable("CandidateRecommendations")`. Hand-stripped, per this session's established pattern — confirmed `SavedSearches`/`CandidateRecommendations`/`AutoShortlistRuns`/`AutoShortlistResults` all coexist afterward.
- **Flat service, no MediatR** — `AutoShortlistRunService` (5 methods: run/get-latest/adjust-cutoff/override/apply), matching this session's established precedent (`SavedSearchService`, `CandidateRecommendationService`), not the older `ShortlistFilters` MediatR/CQRS style. Confirmed with the user before building.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Domain/Enums/Enum.cs` — added `HrOverrideDecisionEnum` (Approved/Rejected).
- `Domain/Entities/AutoShortlistRun.cs`, `AutoShortlistResult.cs` — new entities.
- `Infrastructure/Configurations/AutoShortlistRunConfiguration.cs`, `AutoShortlistResultConfiguration.cs`.
- `Infrastructure/Data/ApplicationDBContext.cs` — new `DbSet`s.
- `Migrations/20260714150839_AddAutoShortlistRun.cs` (+ Designer) — hand-edited per migration hygiene note above.
- `Application/Common/Settings/GroqSettings.cs`, `Application/Common/Exceptions/GroqUnavailableException.cs`.
- `Application/Interfaces/Services/IGroqClient.cs` + `Infrastructure/Services/GroqClient.cs`.
- `Application/Interfaces/Services/IShortlistScoringService.cs` (+ `CandidateScoringResult` record) + `Application/Services/ManualShortlistScoringService.cs` + `Application/Services/AiShortlistScoringService.cs`.
- `Application/Interfaces/Repositories/IAutoShortlistRunRepository.cs` + `Infrastructure/Repositories/AutoShortlistRunRepository.cs`.
- `Application/Interfaces/Services/IAutoShortlistRunService.cs` + `Application/Services/AutoShortlistRunService.cs` — orchestration.
- `Application/Features/AutoShortlisting/Models/` — Run/Result/Cutoff/Override/Apply DTOs (Apply reuses the existing `JobApplicationBulkStatusUpdateFailure`, not a duplicate type).
- `Application/Mappings/AutoShortlistMapper.cs`.
- `Controllers/AutoShortlistController.cs` — `POST run`, `GET {jobPostingId}/latest`, `PATCH {runId}/cutoff`, `PATCH result/{resultId}/override`, `POST {runId}/apply`.
- `Application/Services/ShortlistFilterEvaluationService.cs`, `Domain/Entities/ShortlistFilterCriterion.cs` — `MinScreeningScore` fix (see Design notes).
- `Middlewares/GlobalExceptionHandlerMiddleware.cs` — `GroqUnavailableException` → 503.
- `Application/Extensions/DependencyInjection.cs`, `Infrastructure/Extensions/DependencyInjection.cs` — DI + the Manual/Ai switch.
- `appsettings.json` — `ShortlistScoring`/`Groq` sections (no secret). `appsettings.Development.json` (gitignored) carries the real Groq API key locally.
- `SylviaNG.Recruitment.Tests/Services/ManualShortlistScoringServiceTests.cs`, `AiShortlistScoringServiceTests.cs`, `AutoShortlistRunServiceTests.cs` — new. `ShortlistFilterEvaluationServiceTests.cs` — updated for the `MinScreeningScore` fix.

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/enums/recruitment.enum.ts` — added `HrOverrideDecisionEnum`.
- `@core/interfaces/recruitment-management/auto-shortlist.interface.ts`, `@core/services/recruitment/auto-shortlist/auto-shortlist.service.ts` — new.
- `pages/application-tracking/auto-shortlist-dialog/` — new standalone component (own component, not inlined into the already-571-line `ats-dashboard.component.ts`, mirroring the `PipelineProgressTrackerComponent` precedent): ranked-list table (score/status/explanation/decision), cutoff `p-inputnumber` (no `p-slider` — none exists in this codebase; reuses the exact `minScreeningScore` input precedent), Approve/Reject/Reset override actions, Apply summary dialog.
- `pages/application-tracking/ats-dashboard/ats-dashboard.component.ts/html` — "AI Shortlist" button + dialog wrapper, gated the same way as the existing Shortlist Filter row.
- `pages/application-tracking/application-tracking.module.ts` — declared the new component (no new PrimeNG modules needed — `TableModule`/`InputNumberModule`/`DialogModule`/`TooltipModule` already imported).

## Verification

- `dotnet test` — 227/227 passing (no regressions).
- `dotnet ef database update` — confirmed new tables; confirmed sibling-branch tables (`SavedSearches`, `CandidateRecommendations`) survived intact.
- `ng build` (development config) — compiles clean.
- Backend end-to-end via `curl`, both providers: Ai (real Groq call — plausible score/explanation), Manual (deterministic score matching the hand-computed formula exactly, 88/100). No-profile application correctly fails with a clear reason. Cutoff adjustment recomputes pass/fail without rescoring. Override wins independent of score. Apply moves exactly the final-included set to Shortlisted. `MinScreeningScore` criterion now genuinely evaluates against the persisted score (verified both a passing and failing threshold).
- Frontend end-to-end via Playwright against the real UI + real Groq: selected a vacancy, opened "AI Shortlist," ran scoring (real Groq response rendered in the ranked list), adjusted cutoff (flips Pass/Fail without a new run), Apply Decisions → summary dialog (Total Shortlisted: 1) → dashboard row status updated to Shortlisted, confirmed via DB. No leftover test data (checked directly in Postgres both rounds).
