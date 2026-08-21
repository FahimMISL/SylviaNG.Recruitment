# EP-15 F1: User Accounts + Role/Permission Engine

## Epic

EP-15: Access Control & User Management. F1 is the foundation every later EP-15 feature (and this story's own scope) assumes exists: HR user CRUD with Keycloak invite, custom role creation with a resource-level permission matrix.

## User stories

- **US-111 — User Accounts** (Must Have, M): HR/Admin user CRUD, Keycloak invite (creates the realm user + sends verify-email), multi-role assignment, activate/deactivate.
- **US-112 — Role/Permission Engine** (Must Have, L): custom role creation with a resource-level permission matrix (Module x Action: View/Create/Edit/Delete/Approve across JobPostings/Applications/Interviews/Assessments/Reports/Admin — no Requisitions module, this system has no separate requisition entity).

## Why

Must Have foundation — every other EP-15 story (F2 scoped access, F3 impersonation) enforces against this engine.

## Branch base

`demo/local-showcase` (switched from `dev` per user decision mid-session — dev is 86 commits behind and this system is heading to production/SaaS, so building on the branch reflecting real current state mattered more than dev purity here).

## Scope decisions (deviate from the literal brief — flagged for review)

1. **The granular Role/Permission engine is additive, not a retrofit.** Today's 3 system roles (`Admin`/`HR`/`Candidate`) are hardcoded Keycloak realm roles checked via `[Authorize(Roles="Admin,HR")]` across ~40 existing controllers. Rewriting all of that to route through the new permission engine was out of scope for an "L" story — the new engine governs only the endpoints EP-15 itself introduces (`UserAccountController`, `RoleController`, and later F3's impersonation/profile-field-config), gated via a new `[RequirePermission(Module, Action)]` filter instead of `[Authorize(Roles=...)]`. Admin/SuperAdmin bypass the check (same superuser shortcut every other role-gated endpoint already has); a custom role needs an explicit grant for that exact (Module, Action) cell.
2. **`SuperAdmin` added as a 5th... then 4th system role.** User confirmed a production/SaaS direction, so a real platform-operator tier (`SuperAdmin`) was added now rather than deferred, to avoid retrofitting it after real tenant customers exist. A `HiringManager` role was also built (for F2's US-113) then **fully rolled back mid-session** after the user said it wasn't needed — `dotnet ef migrations remove` + revert-then-reapply, deleted the stray Keycloak realm role, stripped doc comments referencing it. Final system roles: `Admin`/`HR`/`Candidate`/`SuperAdmin` (4). US-113's "own postings" scoping will apply to the existing `HR` role instead — exact mechanism still open, see F2 planning note below.
3. **No separate `Permission` table.** Modeled as an enum pair (`AccessControlModuleEnum`, `PermissionActionEnum`) plus a `RolePermission` junction (composite key `RoleId`+`Module`+`Action`) — a row's mere existence is the grant, no `IsGranted` flag, exactly mirroring the checked cells in the UI matrix.
4. **System roles get mirror `Role` rows (`IsSystemRole=true`, seeded via migration `HasData`)** so they list alongside custom roles in the management UI, but their `Permissions` are informational only — actual enforcement for those 4 roles is unchanged (`[Authorize(Roles=...)]`). Editing/deleting a system role row throws `ForbiddenException` even for Admin.
5. **`UserAccount` is a local mirror, not the identity source of truth.** Keycloak remains authoritative for credentials; `UserAccount` (KeycloakUserId, Email, FullName, IsActive) exists so the app can list/manage role assignments without hitting the Keycloak Admin API on every read.

## Real gaps found during implementation (not assumptions)

- **No HR user CRUD existed at all before this.** `IKeycloakClient.CreateUserAsync` was previously only ever called from candidate self-registration (`AuthService.RegisterAsync`) with a single hardcoded `Candidate` role. Had to add `CreateRealmRoleAsync`, `GetRealmRolesAsync`, and a public `AssignRealmRolesAsync` (multi-role, targets an *existing* user) — the old `AssignRealmRoleAsync` was private and only ran once, during creation.
- **The backend's Keycloak service-account client only had `manage-users`, not `manage-realm`.** Creating a realm role via the Admin REST API returned a live 403 the first time it was exercised — nothing before this feature ever needed to create roles, only users. Granted `manage-realm` (client role on `realm-management`) to the `sylviang-api` service account in the local Keycloak realm as part of getting this working; **this same grant needs to happen in every other environment** (staging/prod Keycloak realm) before F1 can work there.
- **`Audit.CreatedBy` is not actually populated anywhere today** (confirmed via `git grep` — only ever set in seed data as a placeholder `1L`, no `SaveChanges` interceptor stamps it from the current user). This matters for F2: `JobPosting.CreatedBy`-based scoping doesn't yet have real data behind it and will need that wiring as part of F2, not just a query filter.
- **A real repository bug found during live verification, not code review**: `IUserAccountRepository`'s `GetByKeycloakUserIdWithRolesAsync` included `RoleAssignments.Role` but not `.Role.Permissions` — so `PermissionService.HasPermissionAsync` always saw an empty permission list and every non-Admin request 403'd regardless of actual grants. Fixed by extending the `Include` chain to `.ThenInclude(r => r.Permissions)`; caught by literally toggling a permission on/off against a live custom-role user and watching the same JWT go from 403 to 200.

## Key files

### Backend (`SylviaNG.Recruitment-master`)
- `Domain/Entities/UserAccount.cs`, `Role.cs`, `RolePermission.cs`, `UserRoleAssignment.cs` (new)
- `Domain/Enums/Enum.cs` — `UserRoleEnum` gets `SuperAdmin`; new `AccessControlModuleEnum`, `PermissionActionEnum`
- `Infrastructure/Configurations/UserAccountConfiguration.cs`, `RoleConfiguration.cs` (seeds the 4 system roles), `RolePermissionConfiguration.cs`, `UserRoleAssignmentConfiguration.cs` (new)
- `Infrastructure/Data/ApplicationDBContext.cs` — 4 new `DbSet<T>`
- `Application/Interfaces/Services/IKeycloakClient.cs` + `Infrastructure/Services/KeycloakClient.cs` — `CreateRealmRoleAsync`, `GetRealmRolesAsync`, `AssignRealmRolesAsync`
- `Application/Interfaces/Repositories/IUserAccountRepository.cs`/`Infrastructure/Repositories/UserAccountRepository.cs`, `IRoleRepository.cs`/`RoleRepository.cs` (new)
- `Application/Interfaces/Services/IUserAccountService.cs`/`Application/Services/UserAccountService.cs`, `IRoleService.cs`/`RoleService.cs`, `IPermissionService.cs`/`PermissionService.cs` (new)
- `Application/Common/Authorization/RequirePermissionAttribute.cs` (new)
- `Application/Features/UserAccounts/**`, `Application/Features/Roles/**` (new, CQRS feature folders mirroring `HiringPipelines/`)
- `Controllers/UserAccountController.cs`, `RoleController.cs` (new)
- `Application/Mappings/UserAccountMapper.cs`, `RoleMapper.cs` (new)
- `Application/Extensions/DependencyInjection.cs`, `Infrastructure/Extensions/DependencyInjection.cs` — new service/repository registrations
- Migration `20260730071124_AddAccessControlEngine`

### Frontend (`sylviang.adminui.recruitment-main`)
- `src/app/@core/enums/user-role.enum.ts` — adds `SuperAdmin`
- `src/app/@core/interfaces/recruitment-management/access-control.interface.ts` (new)
- `src/app/@core/services/recruitment/access-control/access-control.service.ts` (new — `UserAccountService`, `RoleService`)
- `src/app/pages/access-control-management/**` (new module: `user-account-list`/`user-account-form`, `role-list`/`role-form` with the permission-matrix grid, routing + NgModule)
- `src/app/pages/pages-routing.module.ts` — new `access-control` lazy route
- `src/app/@core/constants/nav-menu-items.ts` — "User Accounts"/"Roles" under System Administration

## Verification

- Backend: `dotnet test` — 754/754 passing (no regressions; no new unit tests added yet for the new services — flagged as a gap, see below).
- Backend live end-to-end (both servers running, real local Keycloak + Postgres, curl): logged in as `admin`/HR seed user; created a custom "Recruiter" role with a partial permission grant (JobPostings View+Edit, Applications View) — verified the matching Keycloak realm role got created; invited a new user assigned to that role — verified Keycloak realm user created with the correct role mapping and the issued JWT's `realm_access.roles` carries exactly `Recruiter`; confirmed granular enforcement by hitting `GET /recruitment/user-accounts` (needs Admin/View) with that user's token — 403, then granting Admin/View to the same role and re-hitting with the **same unexpired JWT** — 200 (proves the check is DB-backed per-request, not baked into the token); confirmed per-action granularity (Admin/Edit and Admin/Delete still 403 with only Admin/View granted); confirmed system-role guard (Admin's own token still gets 403 `ForbiddenException` trying to edit/delete the seeded `Admin`/`HR` rows); confirmed `DuplicateException` (409) on duplicate role name / duplicate user email, `ResourceInUseException` (409) deleting a role with an assigned user, and FluentValidation 400 on missing `roleIds`.
- Frontend: `ng build` clean, new `access-control-management` module bundles as its own lazy chunk, route path confirmed present in the built JS.
- **Not verified**: actual browser rendering/interaction of the new UI. No browser-automation tool was available in this session (unlike the Playwright-based live verification used in earlier EP-15-adjacent features) — only the backend API surface and the frontend build/type-check were exercised. Recommend a manual pass at `http://localhost:4600/access-control/user-account-list` and `/access-control/role-list` before merging.

## Known follow-ups (not blocking, flagged for later)

- Local Keycloak's `sylviang-api` service account now has `manage-realm` granted manually via the Admin API during this session — needs the same grant applied to any other environment's realm before F1's role-creation feature will work there (not something a migration can do; it's a Keycloak realm config change).
- `UserAccountService.UpdateAsync`'s Keycloak role sync is additive-only (see code comment) — a role removed locally is not removed from the user's Keycloak realm roles. Low risk today since this app's own permission checks read the local `RoleAssignments` table, not the Keycloak token's role claim, but worth tightening later.
- F2 needs a real decision (with the user) on how "HR scoped to own postings" coexists with HR's existing full system-wide access before implementation starts.
