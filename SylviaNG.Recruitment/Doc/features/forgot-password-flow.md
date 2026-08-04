# Forgot Password Flow

## What

Closes a real login-lockout gap: Account Settings' "change password" requires an active session and the current password, so a user who forgot their password had no way back in. Adds `POST /recruitment/auth/forgot-password` (username → emails a 6-digit OTP to the account's own address) and `POST /recruitment/auth/reset-password` (challengeId + OTP + new password → sets the password directly via Keycloak's Admin API, no old password needed). Both `AllowAnonymous`. Applies to all 3 roles (Admin/HR/Candidate), not just candidates. Frontend adds a "Forgot password?" link on the login page → a 2-step `/login/forgot-password` page (request code → enter code + new password).

## Why

User-reported gap while reviewing the login flow: a locked-out user had zero recovery path. Branched off `demo/local-showcase`, not `dev` (same recurring gap — `dev` predates Keycloak/EP-15 auth work).

## Design decisions

- **New `PasswordResetOtp` entity/table**, not a reuse of `CandidateLoginOtp` or `EmailChangeVerification` — different lifecycle/purpose than either (login-gate OTP vs. email-ownership OTP vs. this), but copies their exact shape: hash/attempt/expiry/lockout, `HMACSHA256` with the same `OtpSettings.Pepper`, `CryptographicOperations.FixedTimeEquals` comparison. Keyed by `KeycloakUserId` (like `EmailChangeVerification`) since the requester isn't authenticated and applies to all 3 roles, not `Username` alone.
- **Enumeration-safe by construction.** `ForgotPasswordAsync` always returns 200 with a `ChallengeId`/`ExpiresAtUtc`, whether or not the username resolves to a real Keycloak user. A `NotFoundException`/`KeycloakUnavailableException` from the Keycloak lookup is swallowed — no OTP row gets persisted and no email goes out, but the caller can't tell the difference from the response shape. An unresolved challenge simply never verifies (falls into the existing generic `OtpVerificationException` path on reset).
- **Reused `OtpVerificationException`** (one generic exception/message for every failure case — missing/expired/wrong/locked) rather than a new exception type, same rationale as the existing OTP flows: the response can't be used to distinguish "wrong code" from "unknown username" from "expired".
- **`KeycloakClient.ResetPasswordAsync`** (admin REST `PUT .../reset-password`, already existed for `AccountSettingsService.ChangePasswordAsync`) is the actual password-set call — deliberately the same one authenticated change-password uses, just reached via OTP proof-of-ownership instead of a valid session + current password.
- **No new NotificationTemplate/EventTemplateMapping seeded in code** — matches the existing pattern (`AccountCreatedOtp`, `EmailChangeRequested` etc. are all admin-configured via the Notification Template Management UI post-deploy, not code-seeded). `RecruitmentEventEnum.PasswordResetRequested` added; dispatch is `Skipped` (not an error) until an admin creates the mapping.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Domain/Entities/PasswordResetOtp.cs` — new.
- `Infrastructure/Configurations/PasswordResetOtpConfiguration.cs` — new.
- `Application/Interfaces/Repositories/IPasswordResetOtpRepository.cs` + `Infrastructure/Repositories/PasswordResetOtpRepository.cs` — new.
- `Domain/Enums/Enum.cs` — `+PasswordResetRequested` on `RecruitmentEventEnum`.
- `Application/Features/Auth/Models/ForgotPasswordRequest.cs`, `ForgotPasswordResponse.cs`, `ResetPasswordRequest.cs` — new.
- `Application/Interfaces/Services/IAuthService.cs` + `Application/Services/AuthService.cs` — `+ForgotPasswordAsync`, `+ResetPasswordAsync`; new `IPasswordResetOtpRepository` dependency.
- `Application/Features/Auth/Commands/ForgotPassword/**`, `Commands/ResetPassword/**` — new (Command/Handler/Validator, MediatR pattern matching `Login`/`VerifyOtp`).
- `Controllers/AuthController.cs` — `POST forgot-password`, `POST reset-password`, both `AllowAnonymous`.
- `Infrastructure/Data/ApplicationDBContext.cs` — `+PasswordResetOtps` DbSet.
- `Infrastructure/Extensions/DependencyInjection.cs` — registered `IPasswordResetOtpRepository`.
- `Migrations/20260804081103_AddPasswordResetOtp.cs` — new migration (`PasswordResetOtps` table).
- `SylviaNG.Recruitment.Tests/Services/AuthServiceTests.cs` — updated `AuthService` constructor call for the new repository dependency.

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/interfaces/auth/forgot-password-request.interface.ts`, `forgot-password-response.interface.ts`, `reset-password-request.interface.ts` — new.
- `@core/services/auth/auth.service.ts` — `+forgotPassword()`, `+resetPassword()`.
- `auth/forgot-password/forgot-password.component.{ts,html,scss}` — new, 2-step (request → OTP + new password), reuses `p-inputotp` from `otp-verify.component` and the login card layout/styling.
- `auth/login/login.component.html`, `.scss` — `+"Forgot password?"` link.
- `auth/auth-routing.module.ts`, `auth.module.ts` — `+forgot-password` route/declaration.

## Verification

- `dotnet build` clean, `npx tsc --noEmit` clean.
- `dotnet ef database update` applied cleanly against local Postgres (`PasswordResetOtps` table).
- Full E2E via `curl` against the running local stack (HR user `abir`):
  - `forgot-password` with a real username and with a nonexistent one → identical 200 response shape (`challengeId`/`expiresAtUtc`) both times — confirmed no enumeration signal.
  - `reset-password` with a wrong code → 401, generic "Incorrect or expired code."
  - Real OTP code recovered from the stored HMAC hash via the known local dev pepper (legitimate — this session's own local test data, same technique used in the EP-09 OTP verification) → `reset-password` succeeded, then confirmed the password was actually changed in Keycloak by logging in with the new password (real token pair returned).
  - Restored `abir`'s original password via the same flow afterward so the local dev HR login credential wasn't left broken.
- Frontend: navigated the running dev app, clicked "Forgot password?" from `/login`, confirmed it routes to `/login/forgot-password` and renders the request-code step correctly.
- `dotnet test` on this branch still fails to *compile* on ~7 pre-existing unrelated test files (`CandidateRecommendationServiceTests`, `ExamScoreImportServiceTests`, `InterviewServiceTests`, etc., all constructor/API drift) — confirmed via `git status` none of those files were touched by this change; pre-existing gap on `demo`, not introduced here.
