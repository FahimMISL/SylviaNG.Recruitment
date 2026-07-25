# EP-09 Feature 2 — Notification Dispatch, Bulk Notify, Admit-Card Retry, OTP-Gated Login (US-075, US-076, US-077 + OTP scope addition)

## What

Wires Feature 1's template/mapping engine up to real senders: a new `NotificationDispatchService` resolves `EventTemplateMapping` for both `Candidate` and `AdminHr` recipients, renders via `PlaceholderSubstitutionService`, sends via `ISmtpEmailService`, and records every attempt in a new `NotificationLog` table. Hooked into `JobApplicationService` at submit / status-change / withdraw (US-075); a new `bulk-notify` endpoint + ATS dashboard button re-dispatches a chosen event across a batch of applications (US-076); admit-card email now retries up to 3 times on transient SMTP failure (US-077). Also adds a numeric OTP gate on top of Keycloak's existing link-based email-verification flow for Candidate logins — a scope addition the user asked to fold into this feature rather than defer again — using a two-phase challenge (Keycloak credential check succeeds but the real token is withheld until the OTP is verified).

## Why

Feature 1 deliberately shipped with nothing sending — this feature is the consumer side that makes the whole EP-09 template/mapping system actually useful. The OTP-gate addition reuses the `AccountCreatedOtp` event Feature 1 already anticipated in both enums but left unwired.

Branched off `demo/local-showcase` (not `dev` — same recurring gap: backend `dev` predates the Feature-1 entities, frontend `dev` is at US-050).

## Design decisions

- **New `NotificationLog` table built now, not deferred to Feature 3** (US-078/079, notification log + in-app bell) — avoids that feature retrofitting a log table. One row per (event, recipient) dispatch attempt; `DeliveryStatus` (not `Status`, to avoid shadowing `Audit.Status`) reuses the existing `NotificationStatusEnum` (`Pending/Sent/Failed/Skipped`).
- **`NotificationDispatchService.DispatchAsync` never throws** — mirrors `ExamNotificationService`'s existing pattern exactly. A missing mapping is `Skipped` (expected until Admin configures one), a render/send failure is `Failed`, but the caller's own action (submit/status-change/bulk-notify) always succeeds regardless.
- **`persistImmediately` flag** on `DispatchAsync` — `true` for the standalone bulk-notify path (issues its own `SaveChangesAsync`), `false` when called inline mid-transaction from `JobApplicationService` so the status change and the `NotificationLog` row commit atomically in the caller's existing single save.
- **No event bus.** `entity.AddDomainEvent(new ApplicationStatusChangedEvent{...})` in `JobApplicationService` has been dead code since it was written (`ApplicationDBContext` does `modelBuilder.Ignore<DomainEvent>()`, nothing ever publishes it) — confirmed via full-repo grep, left alone rather than resurrected. Dispatch is wired as direct synchronous service calls instead, matching this codebase's actual convention everywhere else.
- **`ApplicationSetting.HrNotificationEmail`** (new nullable field on the existing single-row settings entity) is where the AdminHr leg's address comes from — no HR contact mailbox existed anywhere in the system before this. Unset = AdminHr leg is `Skipped`, not failed.
- **US-077 retry is a bounded in-process retry** (`EmailRetrySender`, 3 attempts, fixed 2s delay), not a durable queue — this codebase has no Hangfire/Polly/Quartz anywhere (confirmed via `.csproj`; the one dormant `BackgroundService`, `EmployeeEventConsumer`, is commented out of DI). A durable retry-queue is a bigger, separate infrastructure investment, explicitly not built here.
- **OTP is a two-phase challenge, not a token-plus-flag.** `AuthService.LoginAsync`'s Keycloak branch, on success, checks `OtpSettings.Enabled && role == Candidate`: if true, it withholds `Token`/`RefreshToken` entirely and returns `RequiresOtp=true` + a `ChallengeId`; the real Keycloak refresh token obtained at that first login attempt is cached in `IMemoryCache` keyed by `ChallengeId` (never persisted to Postgres) until `verify-otp` exchanges it via `_keycloakClient.RefreshTokenAsync`. Chosen over issuing a client-trusted "requires OTP" flag alongside a real token, because every other endpoint validates the Keycloak JWT signature only — a flag-gated-but-real token would need every protected endpoint retrofitted to also check it.
- **OTP gate applies only to real Keycloak-authenticated Candidate logins** — the 3 hardcoded offline fallback accounts (used only when Keycloak is unreachable) are untouched, since gating a dev-only safety net adds friction with no security benefit.
- **OTP code**: `RandomNumberGenerator.GetInt32` (never `Random`), stored only as an HMACSHA256 hash keyed by a new `OtpSettings.Pepper` secret (never plaintext), compared via `CryptographicOperations.FixedTimeEquals`. 10-minute expiry, 5-attempt lockout, both configurable. Off by default (`Enabled=false`) until explicitly turned on.
- **One generic `OtpVerificationException`** for every failure case (missing/expired/wrong/locked) rather than distinct exception types — the error message is deliberately the same regardless of reason, so a caller can't use the response shape to enumerate valid challenges or narrow down attempts.
- **Resend-OTP included** (regenerates code, resets expiry/attempt count, reuses the still-cached refresh token) — without it, missing the 10-minute window means restarting login from scratch.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Domain/Entities/NotificationLog.cs`, `CandidateLoginOtp.cs` — new.
- `Infrastructure/Configurations/NotificationLogConfiguration.cs`, `CandidateLoginOtpConfiguration.cs` — new.
- `Application/Interfaces/Repositories/INotificationLogRepository.cs`, `ICandidateLoginOtpRepository.cs` + `Infrastructure/Repositories/` implementations — new.
- `Application/Interfaces/Repositories/IEventTemplateMappingRepository.cs` + `Infrastructure/Repositories/EventTemplateMappingRepository.cs` — added `GetActiveMappingAsync` (the runtime resolver Feature 1 didn't need).
- `Application/Interfaces/Services/INotificationDispatchService.cs` + `Application/Services/NotificationDispatchService.cs` — new.
- `Application/Common/Notifications/EmailRetrySender.cs` — new; wired into `Application/Services/ExamNotificationService.cs`'s `SendEmailAsync`.
- `Application/Common/Settings/OtpSettings.cs` — new.
- `Application/Common/Exceptions/OtpVerificationException.cs` — new; mapped to 401 in `Middlewares/GlobalExceptionHandlerMiddleware.cs`.
- `Domain/Entities/ApplicationSetting.cs`, `Application/Features/ApplicationSettings/Models/ApplicationSettingModels.cs`, `Application/Services/ApplicationSettingService.cs`, `Application/Interfaces/Services/IApplicationSettingService.cs` — `+HrNotificationEmail`.
- `Application/Services/JobApplicationService.cs` — `INotificationDispatchService` injected; dispatch hooks in `SubmitAsync`, `ApplyStatusChangeAsync`, `WithdrawMyApplicationAsync`; new `BulkNotifyAsync`.
- `Application/Interfaces/Services/IJobApplicationService.cs`, `Application/Features/JobPostings/Models/JobApplicationBulkNotifyRequest.cs`, `JobApplicationBulkNotifyResponse.cs` — new.
- `Controllers/JobApplicationController.cs` — `POST bulk-notify`.
- `Application/Services/AuthService.cs`, `Application/Interfaces/Services/IAuthService.cs` — OTP branch in `LoginAsync`, new `VerifyOtpAsync`/`ResendOtpAsync`.
- `Application/Features/Auth/Models/LoginResponse.cs` — `+RequiresOtp`, `+ChallengeId` (additive, non-breaking).
- `Application/Features/Auth/Models/VerifyOtpRequest.cs`, `ResendOtpRequest.cs`, `Commands/VerifyOtp/**`, `Commands/ResendOtp/**` — new.
- `Controllers/AuthController.cs` — `POST verify-otp`, `POST resend-otp`.
- `Application/Extensions/DependencyInjection.cs`, `Infrastructure/Extensions/DependencyInjection.cs` — registered new services/repositories, `AddMemoryCache()`, `OtpSettings` config binding.
- `Infrastructure/Data/ApplicationDBContext.cs` — 2 new DbSets.
- `Migrations/20260724082116_AddNotificationDispatchAndOtpLogin.cs` — new migration (`NotificationLogs`, `CandidateLoginOtps` tables, `ApplicationSettings.HrNotificationEmail` column).
- `appsettings.json`, `appsettings.Development.json` (gitignored) — new `CandidateLoginOtp` section.

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/interfaces/auth/login-response.interface.ts` — `+requiresOtp`, `+challengeId`.
- `@core/interfaces/auth/verify-otp-request.interface.ts`, `resend-otp-request.interface.ts` — new.
- `@core/services/auth/auth.service.ts` — `login()` only persists a session when `!requiresOtp`; new `verifyOtp()`/`resendOtp()`.
- `auth/login/login.component.ts` — redirects to `/login/verify-otp` when `requiresOtp`.
- `auth/otp-verify/otp-verify.component.{ts,html,scss}` — new, PrimeNG `p-inputotp` (first real use in this codebase), resend with a 30s client cooldown.
- `auth/auth-routing.module.ts`, `auth.module.ts` — `+verify-otp` route, `+InputOtpModule`.
- `@core/interceptors/error-handler.interceptor.ts` — excluded `/auth/verify-otp` and `/auth/resend-otp` from the global "401 → force logout + redirect to /login" handler (see Verification — this was a real bug caught live, not a speculative fix).
- `@core/interfaces/recruitment-management/job-application.interface.ts` — `IJobApplicationBulkNotify{Request,Response,Failure}`.
- `@core/services/recruitment/job-application/job-application.service.ts` — `bulkNotify()`.
- `pages/application-tracking/ats-dashboard/ats-dashboard.component.{ts,html}` — "Notify Event" dropdown + "Notify N Selected" button in the existing bulk-action bar.

## Verification

- `dotnet build` clean (one pre-existing unrelated warning). `npx tsc --noEmit` clean. No new unit tests were added this pass — verification below is live/E2E only, matching Feature 1's second verification approach but without the unit-test layer this time; flagging as a gap if test coverage is wanted later, especially for the OTP hash/lockout logic.
- `dotnet ef database update` applied cleanly against local Postgres (`NotificationLogs`, `CandidateLoginOtps`, `ApplicationSettings.HrNotificationEmail`).
- Full E2E via `curl` against the running local stack (Keycloak `admin`/`admin123`):
  - Seeded `NotificationTemplate`s + `EventTemplateMapping`s for `ApplicationSubmitted`/`ApplicationStatusChanged` (Candidate + AdminHr) and `AccountCreatedOtp` (Candidate).
  - `PATCH .../status` on a real seeded application → both Candidate and AdminHr `NotificationLog` rows `Sent` via the live dev Gmail SMTP.
  - `POST bulk-notify` with a mix of valid/invalid ids → correct `succeededIds`/`failed` split, correct `NotificationLog` rows with `JobApplicationId` FK set.
  - An event with no configured mapping (`CandidateActionRequired`) → `Skipped`, no exception, the underlying action still succeeded.
  - OTP: registered a real Candidate via `/auth/register`, logged in → `Token`/`RefreshToken` empty, `RequiresOtp=true`, `ChallengeId` present (no usable session issued). Wrong code → 401, generic message. Reused/already-consumed code → 401. 5 wrong attempts → row `Locked=true`, even the real code then rejected. `resend-otp` → new hash, `AttemptCount` reset, still worked end-to-end. Correct code → real Keycloak-issued token pair returned. Admin login unaffected (`RequiresOtp` absent/false) throughout.
- Full E2E in a real headless-Chromium browser (Playwright via `npx`, no project browser-automation tool existed):
  - Candidate login → correctly routes to `/login/verify-otp` (not the dashboard), renders the 6-digit `p-inputotp` entry.
  - **Wrong code initially force-redirected to `/login` instead of showing the error in place** — the global `ErrorHandlerInterceptor` treats any non-`/auth/login` 401 as "session expired" and logs out + redirects. Fixed by excluding `/auth/verify-otp`/`/auth/resend-otp` from that behavior (no session exists yet at that point, so there's nothing to expire). Re-tested: wrong code now correctly stays on the OTP screen and shows "Incorrect or expired code."; a full happy-path run (real code, recovered from the DB hash via the known dev pepper — legitimate since it's this session's own local test data) completed the whole flow through to `/dashboard` with a persisted token and zero console errors.
  - Admin login confirmed unaffected — straight to `/dashboard`, no OTP screen.
  - Bulk-notify UI: selecting a row on the ATS dashboard renders the "Notify Event" dropdown and "Notify N Selected" button correctly alongside the existing bulk-status controls, zero console errors.
