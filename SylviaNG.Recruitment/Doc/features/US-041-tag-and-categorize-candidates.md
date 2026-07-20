# US-041 — Tag and Categorize Candidates

## What

HR can add free-text tags to any candidate's profile (e.g. "Strong Communicator", "Leadership Potential") from the candidate detail page, with autocomplete suggestions drawn from tags already used elsewhere. Tags can be removed individually. Both the candidate list and the ATS dashboard gained a "Tags (any of)" multi-select filter that narrows results to candidates carrying at least one of the selected tags. Tags are HR/Admin-only end to end — never returned on the candidate's own `/me` profile.

## Why

US-041 (Should Have, S) — HR needed a way to quickly categorize and re-find candidates by ad-hoc attributes during bulk shortlisting, distinct from the structured Education/Experience/Skills/Location/Age filters already built for US-050. A `\bTag\b` grep across the whole backend confirmed this was fully greenfield (only mentioned in `USER_STORIES.md`), but the codebase already had every pattern needed to build it cheaply and consistently.

## Design notes

- **Mirrors `CandidateSkill`, not `SkillLibraryItem`.** `CandidateTag` is a simple free-text child collection (`CandidateProfileId`, `TagName`) exactly like `CandidateSkill` — no separate curated master table. AC2's "autocomplete based on previously used tags" is a `SELECT DISTINCT TagName` over the join table (`CandidateTagRepository.GetDistinctTagNamesAsync`, case-insensitive contains, alphabetical, capped at 20), not an admin-curated list like `SkillLibraryItem` — tags are meant to emerge organically from HR usage, not be pre-defined.
- **HR-owned, not candidate-owned.** Unlike `CandidateSkillService` (self-service via `ICurrentCandidateService`), `CandidateTagService` takes an explicit `candidateProfileId` on every method — HR is tagging *someone else's* profile. Every route is `[Authorize(Roles = "Admin,HR")]`, and `CandidateProfileResponse` (the candidate's own view) never gained a `Tags` field — that alone satisfies AC4.
- **ATS dashboard filtering reuses the exact `Skills` ANY-match shape.** `JobApplicationAttributeFilterRequest.Tags`, `CandidateFactService.CandidateFacts.TagNames`, and the new block in `JobApplicationService.MatchesAttributeFilter` (`filter.Tags.Any(tag => facts.TagNames.Contains(tag))`) are structurally identical to the existing `Skills` filter (US-050) — same file, same pattern, immediately next to it.
- **Candidate-list filtering is a repository-level `Any()`**, not the generic `PagedRequest.SearchTerm`/`SearchProperties` mechanism (that's a reflection-driven `Contains` scan across scalar string columns on `CandidateProfile` itself — tags live in a separate joined table, so they get their own `tags` query parameter threaded through `CandidateProfileController.GetPaged` → `CandidateProfileGetPagedQuery` → `CandidateProfileService.GetPagedAsync` → `ICandidateProfileRepository.GetPagedAsync`, kept out of the generic `PagedRequest` type since that's shared by unrelated list endpoints).
- **Frontend filter UI is a direct copy of US-050's `filterSkills` p-multiSelect** (ATS dashboard) — every touchpoint (`buildFilterParams`, `activeFilterChips`, `removeFilterChip`, session-storage save/restore) was duplicated verbatim for `filterTags`. The candidate list filter is the same shape, newly added there (that list had no candidate-attribute filtering before this).
- **Suggestions are loaded once, not per-keystroke, for the filter dropdowns** — `getTagSuggestions('')` on init feeds `p-multiSelect`'s own client-side `[filter]="true"` search, same as how `skillLibrary` already feeds the Skills multiselect. The add-tag `p-autoComplete` on the candidate detail page *does* call the endpoint per keystroke (`completeMethod`), matching the existing `skills-section.component.ts` pattern for a genuine type-ahead.
- **Known cap, disclosed, not silently swallowed**: the suggestion endpoint caps at 20 results. In a system with more than 20 distinct tags in use, the filter dropdowns and autocomplete won't surface every tag until narrowed by a search prefix — acceptable for an S-sized "Should Have," revisit if tag volume grows.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Domain/Entities/CandidateTag.cs` (new), `Domain/Entities/CandidateProfile.cs` (`Tags` nav).
- `Infrastructure/Configurations/CandidateTagConfiguration.cs` (new).
- `Application/Interfaces/Repositories/ICandidateTagRepository.cs` + `Infrastructure/Repositories/CandidateTagRepository.cs` (new) — `GetAllByCandidateProfileIdAsync`, `GetDistinctTagNamesAsync`.
- `Application/Interfaces/Services/ICandidateTagService.cs` + `Application/Services/CandidateTagService.cs` (new) — add/delete/list/suggest, duplicate rejection (case-insensitive, per candidate).
- `Controllers/CandidateProfileController.cs` — `GET/POST /{id}/tags`, `DELETE /{id}/tags/{tagId}`, `GET /tags/suggestions`; `GetPaged` gained a `tags` query param.
- `Application/Features/CandidateProfiles/{Commands/CandidateTagCreate,Commands/CandidateTagDelete,Queries/CandidateTagGetAll,Queries/CandidateTagSuggestions}/*` (new CQRS wrappers, mirroring `CandidateSkillCreate`/`Delete`/`GetAll`).
- `Application/Features/CandidateProfiles/Models/{CandidateTagResponse,CandidateTagCreateRequest,CandidateProfileDetailResponse}.cs`, `Application/Mappings/CandidateProfileMapper.cs`.
- `Application/Services/CandidateFactService.cs` (`TagNames`), `Application/Services/JobApplicationService.cs` (`MatchesAttributeFilter`), `Application/Features/JobPostings/Models/JobApplicationAttributeFilterRequest.cs` (`Tags`).
- `Application/Interfaces/Repositories/ICandidateProfileRepository.cs` / `Infrastructure/Repositories/CandidateProfileRepository.cs` / `Application/Interfaces/Services/ICandidateProfileService.cs` / `Application/Services/CandidateProfileService.cs` (`GetPagedAsync(request, tags)`), `Application/Features/CandidateProfiles/Queries/CandidateProfileGetPaged/*`.
- Migration `20260716103058_AddCandidateTags`.
- Tests: `CandidateTagServiceTests.cs` (new), `JobApplicationServiceTests.cs` (Tags cases + `Facts` helper extended), `CandidateProfileServiceTests.cs` (tags passthrough).

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/interfaces/recruitment-management/{candidate-profile.interface.ts,job-application.interface.ts}` — `ICandidateTagResponse`, `ICandidateTagCreateRequest`, `tags` on detail response and `IAtsDashboardFilterParams`.
- `@core/services/recruitment/candidate-profile/candidate-profile.service.ts` — `getTagSuggestions`, `getTags`, `addTag`, `deleteTag`.
- `pages/candidate-management/candidate-detail/candidate-detail.component.*` — Tags section (chips + remove + autocomplete add).
- `pages/candidate-management/candidate-list/candidate-list.component.*` — Tags multi-select filter.
- `pages/candidate-management/candidate-management.module.ts` — `AutoCompleteModule`, `MultiSelectModule`.
- `pages/application-tracking/ats-dashboard/ats-dashboard.component.*` — Tags multi-select filter, fully wired into the existing filter/chip/session-persistence plumbing.

## Out of scope (flagged, not fixed)

- Suggestion/filter-dropdown result cap of 20 (see Design notes) — no pagination on the tag-suggestion endpoint.
- No tag rename/merge tooling — a HR user who creates two near-duplicate tags ("Fast Learner" vs "Quick Learner") has no built-in way to consolidate them; out of scope for this story.

## Verification

- `dotnet test` — 214/214 passing (10 new: 7 `CandidateTagServiceTests`, 2 `MatchesAttributeFilter` Tags cases, 1 `GetPagedAsync` tags-passthrough).
- `ng build` (development config) — compiles clean.
- Logical walkthrough: HR adds "Strong Communicator" and "Leadership Potential" to a candidate, sees both chips immediately with a working remove button; typing "lead" in the autocomplete on a different candidate's page suggests "Leadership Potential"; both the candidate list and ATS dashboard "Tags" filter narrow correctly when selecting one of those tags; the candidate's own `/me` profile response has no `tags` field at any point.
