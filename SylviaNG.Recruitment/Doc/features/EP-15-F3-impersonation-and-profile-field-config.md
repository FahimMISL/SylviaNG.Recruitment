# EP-15 F3: Impersonation + Profile Field Config

## Epic

EP-15: Access Control & User Management. F3 is the last of the three EP-15 features.

## User stories

- **US-115 — Admin Impersonation** (Should Have, M): SuperAdmin impersonates another user with a banner, 30-minute expiry, and full audit log.
- **US-116 — Configurable Profile Fields** (Should Have, M): mandatory/optional/hidden candidate profile fields, global or scoped, with a hard minimum (Name/Email/CV) that can never be hidden.

## Scope decisions (deviate from the literal brief — flagged for review, confirmed with user)

1. **Impersonation targets are Admin/HR UserAccounts only — not candidates.** Asked the user directly given the brief's `TargetUserAccountId/CandidateId` wording implied either; confirmed: staff-to-staff impersonation only for now. Candidate identity resolution (email fallback, guest applications, `CurrentCandidateService`) is a materially different, more fragile subsystem that would need its own dedicated pass to impersonate safely.
2. **Impersonation reuses the existing "Local" JWT scheme rather than a live-principal-swap middleware.** `StartAsync` issues a short-lived (30 min) token, signed with the same `Jwt:Local:SigningKey` the hardcoded-auth scheme already uses, carrying the *target's* own identity claims (their Keycloak subject id, so every existing `sub`/`NameIdentifier`-keyed lookup in the app — `PermissionService`, `JobPostingService`'s `CreatedBy` resolver, etc. — resolves to the target automatically) plus two custom claims (`imp_sid`, `imp_by`). The frontend swaps its active token to this one for the session. This avoids needing the target's real Keycloak credentials at all, and reuses infrastructure already proven by `AuthLoginSmokeTests` instead of building a new claims-swap middleware from scratch.
3. **Live revocation, not just token expiry.** `ImpersonationMiddleware` re-checks the session against the database on every request bearing an `imp_sid` claim (not ended, not expired) — so calling `/end` takes effect immediately even though the JWT itself is still cryptographically valid for the rest of its 30 minutes. Verified live (see below): reusing the exact same token after `/end` gets rejected.
4. **"Per job type" reinterpreted as "per job posting."** No `JobType` master-data entity exists anywhere in this schema (checked). `ProfileFieldConfig.JobPostingId` (nullable, null = global default) is the concrete, real equivalent — overriding a specific posting's apply form, which is what the story's underlying need actually is.
5. **`Name`/`Email`/`CV` are structurally excluded from `CandidateProfileFieldEnum`**, not merely defaulted to Mandatory — they were never added as enum members, so there's no code path that could ever mark them Hidden.
6. **No SuperAdmin fallback account exists** in `AuthService`'s hardcoded `FallbackUsers` list (only Admin/HR/Candidate). The positive "SuperAdmin can start impersonation" path is covered by live curl verification below rather than an automated smoke test, since adding a 4th fallback user was out of scope here; the negative path (non-SuperAdmin gets 403) is covered by an automated test.
7. **No frontend edit for `ProfileFieldConfig` rows** — list, create, delete only. Editing an existing row's visibility is delete-then-recreate. Kept scope tight; can add a proper edit form later if it turns out to matter in practice.

## Key files

### Backend (`SylviaNG.Recruitment-master`)
- `Domain/Entities/ImpersonationSession.cs`, `ImpersonationLog.cs`, `ProfileFieldConfig.cs` (new)
- `Domain/Enums/Enum.cs` — `CandidateProfileFieldEnum`, `ProfileFieldVisibilityEnum`
- `Infrastructure/Configurations/ImpersonationSessionConfiguration.cs`, `ImpersonationLogConfiguration.cs`, `ProfileFieldConfigConfiguration.cs` (new)
- `Application/Common/Authorization/ImpersonationClaimTypes.cs` (new) — shared `imp_sid`/`imp_by` claim name constants
- `Application/Services/ImpersonationService.cs` + `IImpersonationService.cs` (new) — token issuance, `EndCurrentAsync`
- `Middlewares/ImpersonationMiddleware.cs` (new) — live session re-validation + audit log write, wired into `Program.cs` right after `UseAuthentication`
- `Controllers/ImpersonationController.cs` (new) — `POST start` (SuperAdmin), `POST end`
- `Application/Services/ProfileFieldConfigService.cs` + `IProfileFieldConfigService.cs` (new) — CRUD + `GetEffectiveConfigAsync` (global/posting merge)
- `Controllers/ProfileFieldConfigController.cs` (new) — CRUD Admin-only, `GET effective` `[AllowAnonymous]` (drives the public/guest apply form, same reasoning as `CareerPortalController`)
- `Application/Interfaces/Repositories/IImpersonationSessionRepository.cs`/`Infrastructure/Repositories/ImpersonationSessionRepository.cs`, `IProfileFieldConfigRepository.cs`/`ProfileFieldConfigRepository.cs` (new)
- Migration `20260731084237_AddImpersonationAndProfileFieldConfig`
- `SylviaNG.Recruitment.Tests/Smoke/ImpersonationScopeSmokeTests.cs` (new), `SylviaNG.Recruitment.Tests/Services/ProfileFieldConfigServiceTests.cs` (new)

### Frontend (`sylviang.adminui.recruitment-main`)
- `src/app/@core/services/auth/auth.service.ts` — `startImpersonation`/`restoreOriginalSession`/`isImpersonating`/`getImpersonationInfo`, stashes the real session under separate storage keys so End restores it exactly
- `src/app/@core/services/recruitment/impersonation/impersonation.service.ts`, `recruitment/profile-field-config/profile-field-config.service.ts` (new)
- `src/app/shell/components/impersonation-banner/**` (new) — countdown banner wired into `shell.component.html`, auto-restores the original session client-side if the timer runs out
- `src/app/pages/access-control-management/user-account-list/**` — "Impersonate" button (SuperAdmin-only, Admin/HR targets only)
- `src/app/pages/profile-field-config-management/**` (new module) — list/create/delete, lazy route, nav entry under System Administration

## Verification

- Backend: `dotnet test` — 765/765 passing (6 new: 2 impersonation-403 smoke cases, 4 ProfileFieldConfig service tests).
- Backend live end-to-end (curl, real Keycloak + Postgres): invited a SuperAdmin and a target HR user via F1's flow; SuperAdmin's token started impersonation → real signed JWT returned carrying the target's identity/role; used that token directly against an HR-gated endpoint (`GET /hiring-pipeline`) → 200; confirmed an `ImpersonationLog` row was written for that exact request; called `/end` using the impersonation token itself → 200; **reused the identical token again → 403 "session has ended or expired"**, proving live DB revalidation works, not just JWT expiry. Confirmed `ProfileFieldConfig`: created a global "PhoneNumber = Hidden" row and a posting-specific "PhoneNumber = Mandatory" override for posting #1; `GET /effective?jobPostingId=1` correctly returned Mandatory (override wins), `GET /effective?jobPostingId=2` correctly returned Hidden (falls back to global); confirmed the `/effective` endpoint works with no Authorization header at all (anonymous).
- Frontend: `ng build` clean, `profile-field-config-management` bundles as its own lazy chunk.
- **Not verified**: browser rendering/interaction (banner countdown, impersonate button visibility, profile-field-config form) — no browser-automation tool available this session, same caveat as F1/F2.

## Known follow-ups (not blocking, flagged for later)

- If candidate impersonation is ever wanted, it needs its own design pass — the current token-issuance approach could extend to it (issue a token carrying `CurrentCandidateService`'s expected identity shape), but candidate identity resolution has more edge cases (guest applications, email-only matches) than staff identity does.
- `ProfileFieldConfig` has no frontend edit — only create/delete. Fine for now given how rarely this config is expected to change; revisit if that assumption breaks.
- The impersonation banner's client-side countdown auto-restores the original session locally when it hits zero, but doesn't proactively call `/end` first — if the tab is closed instead of hitting zero naturally, the session simply expires server-side at its 30-minute mark on its own (no orphaned-session risk, just not an explicit `EndedAt` stamp in that case).
