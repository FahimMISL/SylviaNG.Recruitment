# Major Subject — Dynamic Dropdown (Profile → Education)

## What

`CandidateEducation.MajorSubject` converted from free text to two admin-manageable dropdowns, selected by education level: a short fixed list (Science, Arts, Humanities, Commerce, Business Studies, Technical, Vocational) for SSC/HSC-equivalent degrees, and a searchable list of 19 common university majors (Computer Science & Engineering, Business Administration, Law, Medicine, etc.) for everything else. Both lists are managed by Admin/HR through the same Master Data Admin module used for Country/Degree/EducationBoard/University/Gender/MaritalStatus/Religion/BloodGroup. Each dropdown carries a frontend-only "Other (please specify)" option revealing a free-text input.

## Why

Direct continuation of the profile-dynamic-dropdowns initiative (feature/profile-dynamic-dropdowns, already merged into `demo/local-showcase`) — inconsistent free-text spelling made candidate data hard to filter. User-specified requirement, built following the same established pattern.

## Design decisions

- **Two separate flat lookup tables** (`MajorSubjectSscHsc`, `MajorSubjectUniversity`), not one table with a category column — matches this codebase's master-data philosophy (dedicated table per type, not a polymorphic enum engine). Each is an exact structural clone of `Gender`: `Id`+`Name` only, full CRUD (Create/Update/Delete/GetAll), duplicate-name guard, delete-in-use guard (`ResourceInUseException` when referenced by any `CandidateEducation`), `GetAll` open to any authenticated role, writes `[Authorize(Roles="Admin")]`.
- **"Other (please specify)" is frontend-only** — a static sentinel (`majorSubjectSscHscId`/`majorSubjectUniversityId` = `-1`) appended to each loaded list client-side, never a seeded database row. Selecting it reveals a text input bound to the shared `CandidateEducation.MajorSubjectOtherText` field. Keeps both lookup tables and the generic Master Data Admin form/list components completely unchanged (`MasterDataFieldType` stays `'text' | 'number'`).
- **`CandidateEducation` schema**: `MajorSubject` (string?) replaced by `MajorSubjectSscHscId` (long? FK), `MajorSubjectUniversityId` (long? FK), `MajorSubjectOtherText` (string?) — mirrors the existing `EducationBoardId`/`UniversityLibraryItemId` pair already on this entity (one populated depending on the same `Degree.Position` SSC/HSC-vs-university split the Board field already uses). Exactly one of the three is populated per row; no cross-field DB constraint, same trust level as the existing Board/University pair.
- **`CandidateEducation.MajorSubjectDisplay`** — a computed, non-mapped property (`MajorSubjectSscHsc?.Name ?? MajorSubjectUniversity?.Name ?? MajorSubjectOtherText`) resolving whichever field is set, used by CV PDF generation and CV Bank search text so both call sites changed by one line each.
- **Migration surprise (better than planned)**: EF detected the old `MajorSubject` and new `MajorSubjectOtherText` columns as type-compatible and generated a `RenameColumn` instead of drop+add — existing candidates' free-text majors are preserved as their "Other" value rather than lost, better than the anticipated data-loss tradeoff.
- **Resume-parsing passthrough unaffected structurally** — `AiResumeParsingService`/`HeuristicResumeParsingService` still return a free-text `majorSubject` guess (no model access to the real dropdown values); the frontend's "Use suggestion" flow gets a `matchMajorSubject()` best-effort substring match against whichever list applies (mirrors the existing `matchDegree()` helper) — no match just leaves the dropdown empty for manual pick.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Domain/Entities/MajorSubjectSscHsc.cs`, `MajorSubjectUniversity.cs` — new, clone `Gender.cs`.
- `Infrastructure/Configurations/MajorSubjectSscHscConfiguration.cs`, `MajorSubjectUniversityConfiguration.cs` — new, with `HasData` seeds (7 + 19 rows).
- `Application/Interfaces/{Repositories,Services}/IMajorSubject{SscHsc,University}{Repository,Service}.cs` + `Infrastructure/Repositories/`, `Application/Services/` implementations — new, clone the Gender vertical exactly.
- `Application/Features/MajorSubjectsSscHsc/**`, `Application/Features/MajorSubjectsUniversity/**` — new CQRS slices (Create/Update/Delete/GetAll).
- `Application/Mappings/MajorSubjectSscHscMapper.cs`, `MajorSubjectUniversityMapper.cs` — new.
- `Controllers/MajorSubjectSscHscController.cs` (`recruitment/major-subject-ssc-hsc`), `MajorSubjectUniversityController.cs` (`recruitment/major-subject-university`) — new.
- `Domain/Entities/CandidateEducation.cs` — field swap + `MajorSubjectDisplay` computed property + 2 new nav props.
- `Infrastructure/Configurations/CandidateEducationConfiguration.cs` — new FK configs (`DeleteBehavior.SetNull`), `Ignore(MajorSubjectDisplay)`.
- `Infrastructure/Data/ApplicationDBContext.cs` — 2 new DbSets.
- `Migrations/20260723044942_AddMajorSubjectLookups.cs` — new migration (2 `CreateTable` + seed `InsertData` + `RenameColumn` + 2 `AddColumn` + FKs/indexes).
- `Application/Features/CandidateProfiles/Models/CandidateEducationCreateRequest.cs`/`UpdateRequest.cs`/`Response.cs`, `Application/Mappings/CandidateProfileMapper.cs`, both `CandidateEducation{Create,Update}Validator.cs` — field swap.
- `Infrastructure/Documents/QuestPdfCvGenerator.cs`, `Application/Features/CvBank/Queries/CvBankSearch/CvBankSearchHandler.cs` — use `MajorSubjectDisplay`.
- `Infrastructure/Repositories/CandidateProfileRepository.cs` — added `ThenInclude(MajorSubjectSscHsc)`/`ThenInclude(MajorSubjectUniversity)` everywhere `Degree` is already `ThenInclude`d (CV PDF/search/bulk paths).
- `Application/Extensions/DependencyInjection.cs`, `Infrastructure/Extensions/DependencyInjection.cs` — registered the 2 new services/repositories.
- `SylviaNG.Recruitment.Tests/Services/MajorSubjectSscHscServiceTests.cs`, `MajorSubjectUniversityServiceTests.cs` — new (create/duplicate/update/delete/delete-in-use/get-all, 12 tests total).

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/interfaces/recruitment-management/candidate-profile.interface.ts` — `IMajorSubjectSscHscResponse`/`IMajorSubjectUniversityResponse` added; `ICandidateEducationCreateRequest`/`Response` field swap.
- `@core/services/recruitment/candidate-profile/candidate-profile.service.ts` — `getMajorSubjectsSscHsc()`, `getMajorSubjectsUniversity()`.
- `pages/master-data-management/master-data.config.ts` — 2 new entries (routes auto-generate from this).
- `@core/constants/nav-menu-items.ts` — 2 new "System Administration" subitems.
- `pages/candidate-profile-management/my-profile/sections/education-section/education-section.component.ts`/`.html` — conditional SSC/HSC (`p-select`) vs University (`p-select [filter]="true"`, searchable) dropdown driven by the existing `showBoardField` Position check, "Other" sentinel + conditional text input, `matchMajorSubject()` resume-suggestion helper, `useSuggestion`/`useAllSuggestions` updated.

## Verification

- `dotnet test` — 488/488 minus the same 6 pre-existing unrelated failures (3× `InternalJobBoardControllerTests` NRE gap, 3× `AuthLoginSmokeTests` needing a live Keycloak realm); all 12 new tests pass.
- `dotnet ef migrations add AddMajorSubjectLookups` — confirmed correct only after the sharper branch-switch snapshot-staleness gotcha bit again (see [[feedback_branch_aware_exploration]]): the on-disk gitignored snapshot still reflected an unrelated, unmerged branch's model from earlier in the session. Fixed via `migrations remove` (regenerates a snapshot from the actual checked-out branch's real migration history) + verifying with `grep` that no foreign entity names remained, then re-adding cleanly. `dotnet ef database update` applied without issue against local Postgres.
- `ng build` (development) compiles clean.
- End-to-end via direct authenticated API calls against the running local stack (no browser-automation tool available in this environment): logged in as HR, confirmed both lookup endpoints return the exact seeded lists (7 SSC/HSC, 19 university, alphabetically ordered); registered/reused a test Candidate, created 3 `CandidateEducation` entries covering all three paths (SSC-level with a picked subject, Bachelor-level with a picked subject, Bachelor-level with "Other" free text) and confirmed each round-trips correctly via `GET`; updated an entry from a University pick to Other text and confirmed the previously-set FK cleared; confirmed HR (non-Admin) gets `403 Forbidden` attempting a write to the lookup endpoints while `GET` succeeds; confirmed duplicate-name creation is rejected; confirmed CV Bank search (`POST /cv-bank/search`) and CV PDF download both succeed without error across candidates carrying the new fields (education summary and search haystack correctly resolve through `MajorSubjectDisplay`). Test education entries cleaned up after verification.
