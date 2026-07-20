# US-039 (follow-up) — Link Talent Pool to Job Vacancy

## What

Talent pools can now optionally be linked to the job vacancy they were created for. HR can set the
link on create, change or clear it later, filter the pool list by vacancy, and the "Fast-track to
Vacancy" dialog now pre-selects the pool's linked vacancy instead of always starting blank.

## Why

The original US-039 talent pool feature is deliberately vacancy-agnostic — a pool is just a named
bucket of candidates HR wants to revisit later, with no record of *which* vacancy prompted it. In
practice HR usually does have a specific role in mind when creating a pool, and losing that context
made it hard to answer "which candidates are for which job vacancy" days or weeks later. This adds
an optional link that preserves the original vacancy-agnostic use case (the field is nullable) while
letting HR record intent when there is one.

## Design notes

- **`JobPostingId` is nullable and `OnDelete(DeleteBehavior.SetNull)`, not `Restrict`.**
  `Repository<T>.Delete` (`SharedKernel/Generic/Repository.cs`) is a hard delete, so a mandatory/`Restrict`
  FK would permanently block deleting any `JobPosting` a pool ever linked to. A pool is still valid
  with no vacancy, so losing the link on delete is the correct behavior, not a regression.
- **`TalentPoolUpdate` is a new command — none existed before this.** Full-replacement request
  (`Name` + `JobPostingId`), matching this codebase's existing `HiringPipelineUpdate`/`JobPostingUpdate`
  convention of resending the whole editable surface, rather than a narrower "just relink" endpoint.
  This also lets HR rename a pool, a small bonus consistent with "editable afterward."
- **`ExistsByNameAsync` gained an `excludeId` parameter** (optional, defaults `null`) so `UpdateAsync`
  can check name uniqueness without the pool's own current name tripping the duplicate check.
- **`JobPostingTitle` is populated via a real EF navigation (`TalentPool.JobPosting`) + `.Include()`**
  in both repository read methods, matching how other `JobPosting`-adjacent entities in this codebase
  expose their related titles, rather than a decoupled ad-hoc join.
- **Filter-by-vacancy on the list page reuses the existing `GET /job-posting` list client-side**
  (same call the Fast-track dialog already makes), not a new "vacancies with pools only" endpoint —
  kept deliberately narrow in scope.
- **Migration was hand-authored, not tool-generated.** `demo/local-showcase`'s migration history has
  pre-existing drift between several feature branches' migrations (each was built against a different
  prior model state before being merged in as sibling files) — running `dotnet ef migrations add`
  produced a migration that tried to recreate the entire schema instead of a 3-statement column add.
  The migration's accompanying `.Designer.cs` (and the regenerated `ApplicationDBContextModelSnapshot.cs`,
  gitignored) are still tool-generated and correctly reflect the full current model; only the
  migration's own `Up()`/`Down()` body was replaced by hand with the minimal `AddColumn`/`CreateIndex`/
  `AddForeignKey` (and reverse) needed for this change. Verified by applying it to the local dev
  database - only those 3 statements ran, nothing else touched.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Domain/Entities/TalentPool.cs` — `JobPostingId`, `JobPosting` nav (forward-only).
- `Infrastructure/Configurations/TalentPoolConfiguration.cs` — index + `SetNull` FK.
- `Migrations/20260720175606_AddTalentPoolJobPostingLink.cs` (+ `.Designer.cs`) — hand-authored body, see design notes.
- `Application/Features/TalentPools/Models/TalentPoolCreateRequest.cs`, new `TalentPoolUpdateRequest.cs`, `TalentPoolResponse.cs`, `TalentPoolDetailResponse.cs` — `JobPostingId`/`JobPostingTitle`.
- `Application/Features/TalentPools/Commands/TalentPoolUpdate/` — new command (Command/Handler/Validator).
- `Application/Interfaces/Services/ITalentPoolService.cs` + `Application/Services/TalentPoolService.cs` — `CreateAsync` validates `JobPostingId`; new `UpdateAsync`; `GetAllAsync` takes optional `jobPostingId` filter.
- `Application/Interfaces/Repositories/ITalentPoolRepository.cs` + `Infrastructure/Repositories/TalentPoolRepository.cs` — `ExistsByNameAsync` exclude-self; `GetAllWithCandidateCountAsync`/`GetByIdWithCandidatesAsync` include `JobPosting`, optional vacancy filter.
- `Application/Mappings/TalentPoolMapper.cs` — map the two new fields.
- `Controllers/TalentPoolController.cs` — `GetAll` gains `jobPostingId` query param; new `PUT /{talentPoolId}`.
- `Application/Features/TalentPools/Queries/TalentPoolGetAll/` — query gains `JobPostingId`.
- `SylviaNG.Recruitment.Tests/Services/TalentPoolServiceTests.cs` — 8 new tests (create with/without valid vacancy, update rename/link/unlink/duplicate-name-excludes-self/unknown-pool, filtered get-all).

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/interfaces/recruitment-management/talent-pool.interface.ts` — `jobPostingId`/`jobPostingTitle` on response/detail types, new `ITalentPoolUpdateRequest`.
- `@core/services/recruitment/talent-pool/talent-pool.service.ts` — `getAll(jobPostingId?)`, new `update()`.
- `pages/talent-pool-management/talent-pool-list/` — job vacancy picker in create/edit dialog (now edit-capable), filter-by-vacancy dropdown, vacancy subtitle on each pool row.
- `pages/talent-pool-management/talent-pool-detail/` — Fast-track dialog pre-selects the pool's linked vacancy; vacancy subtitle near the header.

## Verification

- `dotnet build` — solution builds clean.
- `dotnet test` — 429/429 passing (8 new `TalentPoolServiceTests` cases, no regressions).
- `dotnet ef database update` applied locally against the dev Postgres instance — confirmed only `ALTER TABLE "TalentPools" ADD "JobPostingId"`, the index, and the FK ran; no other tables touched.
- `ng build --configuration development` compiles clean.
- End-to-end via the Browser pane against local `dotnet run` + `npm start`, logged in as HR: created a pool linked to an open vacancy, confirmed the vacancy subtitle appears on the list and detail pages; created a second pool with no vacancy (still works); filtered the list by vacancy and confirmed only the matching pool showed, cleared the filter and both reappeared; edited the first pool - renamed it and cleared its vacancy link, confirmed both changes persisted; opened the Fast-track dialog on the second pool after linking it to an Open vacancy and confirmed the picker pre-selected that vacancy.
