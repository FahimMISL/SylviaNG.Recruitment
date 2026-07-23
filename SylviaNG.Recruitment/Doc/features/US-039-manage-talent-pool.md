# US-039 — Manage Talent Pool

## What

Named talent pools HR can create and populate with promising candidates from anywhere in the
pipeline (candidate list, candidate profile). Each candidate profile shows which pools it belongs
to. A pool's detail page lists its members with their latest profile snapshot, supports removing
a candidate at any time, and lets HR multi-select pool members and fast-track them straight to
`Shortlisted` on a newly-opened vacancy in one action. The candidate list also gets a talent-pool
multi-select filter.

## Why

EP-05's ATS gives HR a way to work an open vacancy end-to-end, but a strong candidate who didn't
get the *current* role was previously a dead end — nothing recorded that they were worth
revisiting. US-039 (Should Have, size M) closes that gap: HR bookmarks a candidate into a named
pool once, and when a matching vacancy opens later, that candidate can be moved to Shortlisted
without re-running the whole application flow.

## Design notes

- **A different, narrower "talent pool" already exists on the unmerged `feature/cv-bank-boolean-search`
  branch** — a single flat, unnamed bucket (`CvBankController` `/recruitment/cv-bank/talent-pool`,
  entity `CandidateTalentPool`) built as a side effect of US-045's bulk-add-to-pool action. That
  branch is not on `dev` and this feature does not touch it. To avoid a naming collision when it
  eventually merges, this feature uses distinct entity/table names (`TalentPool` /
  `TalentPoolCandidate`, not `CandidateTalentPool`) — a known future reconciliation point between
  the two "talent pool" concepts, not resolved here.
- **No reverse nav property on `CandidateProfile`.** `TalentPoolCandidate` has FKs to both
  `TalentPool` and `CandidateProfile`, but `CandidateProfile` does not declare a
  `TalentPoolMemberships` collection. The profile-badge and paged-list filter both join through
  `ITalentPoolCandidateRepository` directly — the same loose-coupling precedent
  `CandidateProfileService.GetProfileDetailAsync` already uses for `ApplicationHistory`
  (`IJobApplicationRepository.GetByCandidateEmailAsync`), rather than an EF `Include`/`ThenInclude`.
- **Fast-track reuses the candidate's most recent resume on file, not a fresh upload.**
  `JobApplication` has no FK to `CandidateProfile` (career-portal applicants may not have a
  profile at all), and the existing "HR applies on behalf of a candidate" flow
  (`JobApplicationSubmitCommand`, US-034) requires a multipart CV upload — not usable for
  fast-tracking an existing profile HR is just re-surfacing. `TalentPoolService.FastTrackAsync`
  instead builds a plain `JobApplicationCreateRequest` from the candidate profile
  (name/email/phone) plus the `ResumeUrl` of that candidate's most recent prior
  `JobApplication` (matched by email, same precedent as US-038/US-040), then calls the existing
  `IJobApplicationService.CreateAsync` + `UpdateStatusAsync(..., Shortlisted)` — `Applied →
  Shortlisted` is already a legal direct transition, added in US-044 for exactly this kind of
  automation. A candidate with zero prior applications (no resume to reuse) is skipped, not
  failed, and counted in the response.
- **`JobApplicationCreateRequest` gained a `Source` field (defaults `Admin`).** Previously
  `JobApplicationService.CreateAsync`'s `ToEntity()` mapping left `Source` at the entity default
  (`External`), which was wrong both for `TalentPoolFastTrackCommand`'s new calls and for the
  existing plain `POST /recruitment/job-application` endpoint (an HR/internal tooling action, not
  a candidate self-submission). Fast-tracked applications now correctly show `Source = Admin` on
  the ATS dashboard.
- **Bulk-add skips duplicates and unknown ids** instead of failing the whole batch
  (`TalentPoolCandidateAddResponse.AlreadyInPoolCount` / `NotFoundCount`), mirroring the
  `AlreadyInPoolCount` pattern in the unmerged branch's `CvBankTalentPoolAddHandler`.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Domain/Entities/TalentPool.cs`, `TalentPoolCandidate.cs` — new entities.
- `Infrastructure/Configurations/TalentPoolConfiguration.cs`, `TalentPoolCandidateConfiguration.cs` — new EF configs.
- `Infrastructure/Data/ApplicationDBContext.cs` — new `DbSet`s.
- `Migrations/20260716145813_AddTalentPools.cs` — new migration (`TalentPools`, `TalentPoolCandidates` tables only).
- `Application/Interfaces/Repositories/ITalentPoolRepository.cs`, `ITalentPoolCandidateRepository.cs` + `Infrastructure/Repositories/` implementations.
- `Application/Interfaces/Services/ITalentPoolService.cs` + `Application/Services/TalentPoolService.cs` — new service (create/delete pool, add/remove candidate, get all/lookup/by-id, fast-track).
- `Application/Features/TalentPools/` — new CQRS feature (Commands: `TalentPoolCreate`, `TalentPoolDelete`, `TalentPoolCandidateAdd`, `TalentPoolCandidateRemove`, `TalentPoolFastTrack`; Queries: `TalentPoolGetAll`, `TalentPoolGetById`, `TalentPoolLookup`; Models; validators).
- `Application/Mappings/TalentPoolMapper.cs` — new mapper.
- `Controllers/TalentPoolController.cs` — new controller at `recruitment/talent-pool`, `[Authorize(Roles = "Admin,HR")]`.
- `Application/Features/CandidateProfiles/Models/CandidateProfileDetailResponse.cs` — new `TalentPools` field (AC2 badge).
- `Application/Mappings/CandidateProfileMapper.cs`, `Application/Services/CandidateProfileService.cs` — `ToDetailResponse`/`GetProfileDetailAsync` take/populate pool memberships; `GetPagedAsync` takes an optional `talentPoolIds` filter.
- `Application/Interfaces/Repositories/ICandidateProfileRepository.cs`, `Infrastructure/Repositories/CandidateProfileRepository.cs` — `GetPagedAsync` talent-pool filter (subquery join, no new include).
- `Application/Interfaces/Services/ICandidateProfileService.cs`, `Controllers/CandidateProfileController.cs`, `Application/Features/CandidateProfiles/Queries/CandidateProfileGetPaged/*` — plumb `talentPoolIds` through.
- `Application/Features/JobPostings/Models/JobApplicationCreateRequest.cs`, `Application/Mappings/JobPostingMapper.cs` — new `Source` field (see design notes).
- `Application/Extensions/DependencyInjection.cs`, `Infrastructure/Extensions/DependencyInjection.cs` — DI registration.
- `SylviaNG.Recruitment.Tests/Services/TalentPoolServiceTests.cs` — new (11 tests: create/duplicate-name, add/skip-existing/skip-not-found, remove/not-found, get-by-id/not-found, fast-track happy path + already-applied + no-resume-on-file + unknown-vacancy).
- `SylviaNG.Recruitment.Tests/Services/CandidateProfileServiceTests.cs` — updated constructor for the new repository dependency.

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/interfaces/recruitment-management/talent-pool.interface.ts` — new.
- `@core/services/recruitment/talent-pool/talent-pool.service.ts` — new.
- `@core/interfaces/recruitment-management/candidate-profile.interface.ts` — `ICandidateProfileDetailResponse.talentPools`.
- `pages/talent-pool-management/` — new lazy-loaded module: `talent-pool-list` (create/delete pools, candidate counts) and `talent-pool-detail` (member table with checkboxes, remove-candidate, "Fast-track to Vacancy" dialog with an Open-vacancy picker and a result summary).
- `pages/candidate-management/candidate-list/` — Talent Pool multi-select filter chip; per-row "Add to Talent Pool" action + dialog.
- `pages/candidate-management/candidate-detail/` — Talent Pools badge section (chips, mirrors the existing Skills chip styling), linking to the pool detail page.
- `pages/pages-routing.module.ts`, `@core/constants/nav-menu-items.ts` — `/talent-pools` route + "Talent Pools" nav entry under Recruitment (Admin/HR), same shape as the `/cv-bank` addition.

## Verification

- `dotnet build` — solution builds clean.
- `dotnet test` — 215/215 passing (11 new `TalentPoolServiceTests`, no regressions).
- `ng build --configuration development` — compiles clean; new `talent-pool-management-module` lazy chunk (51.81 kB) present.
- End-to-end via `dotnet run` + `npm start` against local Docker Postgres/Keycloak, driven directly through the API as the admin user: created a pool, added two candidates to it (one with a prior application/resume on file, one without), confirmed the pool badge appears on `GET /candidate-profile/{id}` for both, confirmed `GET /talent-pool/{id}` returns both with their profile snapshot, confirmed `GET /candidate-profile/paged?talentPoolIds=` narrows the list correctly, fast-tracked both to an Open vacancy in one call and got back `fastTrackedCount: 1, skippedCount: 1` (the no-resume candidate correctly skipped), confirmed the resulting `JobApplication` shows `applicationStatus: Shortlisted` and `source: Admin` on the ATS dashboard, fast-tracked a candidate with an existing application to a *different* Open vacancy and got `source: Admin` there too, removed a candidate from the pool and confirmed it dropped off the pool detail response, deleted the pool and confirmed the list returned empty. All seed/test data created during verification was removed afterward.
