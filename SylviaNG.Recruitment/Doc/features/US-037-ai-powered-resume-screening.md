# US-037 — AI-Powered Resume Screening

## What

Adds structured match details — matched skills as tags and an experience band — alongside each candidate's score in the AI/Manual auto-shortlisting ranked list.

## Why

US-037's ACs (score+explanation per candidate, per-vacancy trigger/re-run, persisted results that don't change unless re-triggered, HR-only visibility) turned out to already be fully satisfied by US-046 (AI-Powered Auto-Shortlisting), built the same day. Comparing the two stories' ACs directly:

| US-037 AC | Status before this change | How |
|---|---|---|
| AC1: analyzes CV/profile against job description | Done via US-046 | `IShortlistScoringService` |
| AC2: matched keywords/skill tags/experience band shown alongside score | **Gap** — only prose in `Explanation` | Closed by this change |
| AC3: plain-language explanation | Done via US-046 | `AutoShortlistResult.Explanation` |
| AC4: trigger for all applications, re-run | Done via US-046 | `AutoShortlistController POST run` |
| AC5: results stored, don't change unless re-triggered | Done via US-046 | `AutoShortlistRun`/`AutoShortlistResult` persistence |
| AC6: visible only to HR | Done via US-046 | `[Authorize(Roles = "Admin,HR")]` |

So this story is scoped as exactly that one gap-closure, not a fresh L-size build — confirmed with the user before starting.

## Design notes

- **Provider-agnostic, computed once in the orchestrator.** Matched skills and experience band are the same regardless of whether the Manual or Ai scorer produced the score — computing them separately inside each `IShortlistScoringService` implementation would duplicate logic and risk drift. New static `CandidateMatchAnalyzer` (`GetMatchedSkills`, `GetExperienceBand`) is called once per candidate in `AutoShortlistRunService.ScoreAllAsync`, alongside (not inside) the scoring call.
- **Reused, not duplicated.** `ManualShortlistScoringService.ScoreSkills` used to compute matched skills inline for its own point-scoring; refactored to call `CandidateMatchAnalyzer.GetMatchedSkills` instead, so the "which skills matched" logic has one home.
- **Experience bands**: 0-1 / 1-3 / 3-5 / 5-10 / 10+ years — a simple fixed bucketing of `CandidateFacts.TotalExperienceYears`, no configuration surface (not asked for by the AC, keeping this a small closure rather than a new configurable feature).
- Same branch, same PR as US-046 (`feature/ai-auto-shortlisting`) — this extends the same not-yet-committed entity, splitting it across branches would be unnecessary churn. Separate commit, so each story's change is traceable in history.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Application/Services/CandidateMatchAnalyzer.cs` — new static helper.
- `Application/Services/ManualShortlistScoringService.cs` — `ScoreSkills` refactored to reuse it.
- `Domain/Entities/AutoShortlistResult.cs` — added `MatchedSkills` (string, comma-separated), `ExperienceBand` (string).
- `Infrastructure/Configurations/AutoShortlistResultConfiguration.cs` — column config for the two new fields.
- `Migrations/20260714164205_AddAutoShortlistResultMatchDetails.cs` (+ Designer) — purely additive, no drop noise this time.
- `Application/Features/AutoShortlisting/Models/AutoShortlistResultResponse.cs` — `MatchedSkills` (`List<string>`), `ExperienceBand`.
- `Application/Mappings/AutoShortlistMapper.cs` — splits the stored comma-separated string back into a list.
- `Application/Services/AutoShortlistRunService.cs` — computes both fields per candidate in `ScoreAllAsync`, stores them on the entity.
- `SylviaNG.Recruitment.Tests/Services/CandidateMatchAnalyzerTests.cs` — new; 7 tests.

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/interfaces/recruitment-management/auto-shortlist.interface.ts` — `matchedSkills: string[]`, `experienceBand: string | null`.
- `pages/application-tracking/auto-shortlist-dialog/auto-shortlist-dialog.component.html/scss` — new "Match Details" column: experience-band label + skill-tag chips.

## Verification

- `dotnet test` — 234/234 passing (227 + 7 new `CandidateMatchAnalyzerTests`, no regressions).
- `dotnet ef database update` — purely additive `ALTER TABLE ... ADD COLUMN`, confirmed via generated migration SQL.
- Backend live via `curl`: seeded a candidate with 2 matching skills and 4 years' experience, ran shortlisting, confirmed `matchedSkills: ["C#", ".NET"]` and `experienceBand: "3-5 years"` in the response, both correct.
- `ng build` (development) — compiles clean.
- Frontend visual check via Playwright: seeded a candidate with 2 skills + 6 years' experience, ran shortlisting through the real UI, confirmed the "Match Details" column renders "5-10 years" + two skill-tag chips exactly matching the seed data. No leftover test data (checked directly in Postgres).
