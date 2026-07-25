# EP-17 — Application Fee Payment (US-120, US-124, US-126) + US-007 AC4 gap closure

## What

Candidates applying to a vacancy with an application fee configured are redirected to SSLCommerz
(Bangladeshi payment gateway) to pay it, and the application only becomes a real `Applied` record
once payment is confirmed via SSLCommerz's server-to-server IPN callback.

Three user stories ship as one bundle (one branch, one PR per repo, one combined doc — they're a
single atomic flow, none of the three is independently useful):
- **US-120** — Configure SSLCommerz Payment Integration
- **US-124** — Candidate Pays Application Fee
- **US-126** — Verify Payment Status via IPN Callback

**US-123 (Configure Application Fee for a Vacancy) was already fully shipped under EP-02** —
`JobPosting.ApplicationFeeAmount`/`ApplicationFeeCurrency`, admin vacancy-config UI, career-portal
display — no work was needed there. This bundle only closes the "nothing actually collects the fee"
gap.

## Why

A vacancy could already carry a configured fee, but a candidate applying to it landed straight on
`Applied` regardless — the fee was purely informational. This wires up real payment collection so
HR can require an application fee on designated vacancies.

## US-007 AC4 gap closure

While in `JobApplicationService.SubmitAsync` for the fee-gate work, closed a long-standing spec
gap: US-007 AC4 ("A minimum completeness threshold (configurable by Admin) must be met before an
application can be submitted") was written but never implemented — no settings entity, no admin
config, no validator rule. Bundled into this same branch/PR rather than a separate feature since
it touches the exact same `SubmitAsync` gating code path as the payment work.

- **New `ApplicationSetting` singleton entity** (`: Audit`, one seeded row, `ApplicationSettingId
  = 1`) holds `MinimumProfileCompletenessPercentage` (0-100, 0 = gate disabled). Seeded at 0 so
  existing candidates/applications aren't retroactively blocked until an Admin opts in.
- **`GET/PUT recruitment/application-settings`** (`ApplicationSettingController`) — GET is
  readable by any authenticated role (candidate UI can surface "X% required"), PUT is
  `[Authorize(Roles="Admin")]`.
- **Gate lives in `JobApplicationService.EnsureMinimumProfileCompletenessAsync`**, called from
  `SubmitAsync` for every source except `Admin` (same on-behalf bypass precedent as payment).
  Resolves the `CandidateProfile` by email (no FK — same join precedent as
  `GetAttributeFilteredApplicationsAsync`/`GetMyApplicationsAsync`), reuses
  `CandidateProfileMapper.CalculateCompleteness` (made `public` for this), and throws
  `FluentValidation.ValidationException` when under threshold.
- **Guest applicants (no CandidateProfile) are let through unchanged.** Career-portal/internal-job-
  board apply never requires an account — there's nothing to measure completeness against, so a
  configured threshold only affects candidates who registered and have a profile.

## Design notes

- **New `AwaitingPayment` status, not a separate flag.** `JobApplicationService.SubmitAsync` now
  computes `requiresPayment = source != Admin && jobPosting.ApplicationFeeAmount is > 0` and sets
  `ApplicationStatus` to `AwaitingPayment` (not `Applied`) when true, before saving. This keeps
  unpaid applications out of the normal `Applied` pipeline (HR dashboard, shortlist filters, etc.)
  without adding a parallel status field. `LegalStatusTransitions[AwaitingPayment] = [Applied,
  Rejected, Withdrawn]` — `Applied` is reached only by `PaymentService.HandleIpnAsync` on a
  confirmed payment (bypassing the HR-user-scoped `EnsureLegalStatusTransition` path, since IPN has
  no authenticated user context); `Rejected` lets HR close out stale/abandoned unpaid applications
  (requires a reason, same as any other rejection); `Withdrawn` lets a candidate with an account
  cancel one they never intend to pay.
- **Admin (`apply-on-behalf`) bypasses payment entirely.** HR manually applying on a candidate's
  behalf shouldn't have to complete an SSLCommerz checkout mid-admin-flow — deliberately scoped out.
- **Single combined submit response.** Rather than a second round-trip, `JobApplicationResponse`
  gained `PaymentRequired`/`PaymentRedirectUrl` — `SubmitAsync` calls `IPaymentService.InitiateAsync`
  inline right after saving the application, wrapped in try/catch (`SslCommerzUnavailableException`)
  so a gateway outage never rolls back the already-saved application; the frontend just checks
  `paymentRequired && paymentRedirectUrl` on the existing apply response and redirects
  (`window.location.href` — the first external redirect in this codebase).
- **IPN is the only source of truth; the browser-return callbacks are not.** `PaymentController`
  exposes both: `POST payment/callback/success|fail|cancel` are SSLCommerz's *browser*-return
  targets — non-authoritative, they just look up the `JobApplicationId` by `tran_id` and 302-redirect
  to the frontend's `payment-result` page. `POST payment/ipn` is SSLCommerz's *server-to-server*
  webhook and is the only path that ever marks a payment `Success` — it always re-fetches the
  transaction from SSLCommerz's own Validation API using the `val_id` and cross-checks
  amount/currency/tran_id against the stored `Payment` row before trusting it, per SSLCommerz's own
  anti-tampering guidance (never trust the raw IPN POST body's status field alone). The frontend
  `payment-result` page polls `GET payment/status/{jobApplicationId}` rather than trusting its own
  query-string hint, since the IPN can land slightly after the browser redirect.
- **One-to-many `Payment` per `JobApplication`.** A candidate can retry after a declined/cancelled
  sandbox attempt; one-to-one would force overwriting failed-attempt history. `PaymentRepository`
  exposes `GetLatestByJobApplicationIdAsync`/`GetByTransactionIdAsync`/`HasSuccessfulPaymentAsync`.
- **All `PaymentController` actions are `[AllowAnonymous]`**, mirroring `CareerPortalController`'s
  posture — career-portal applicants have no guaranteed JWT, and SSLCommerz's own callbacks
  obviously carry none either. The ID-based retry/status endpoints carry no ownership check, matching
  the app's existing risk posture (`JobApplicationController.GetById` already has none today); the
  IPN handler is the only money-moving path and is properly protected via the Validation API
  cross-check.
- **Gateway client mirrors `KeycloakClient` exactly** — typed `HttpClient`
  (`AddHttpClient<ISslCommerzPaymentGateway, SslCommerzPaymentGateway>`, 10s timeout), manual
  try/catch → `SslCommerzUnavailableException` (no Polly, none exists elsewhere in this repo),
  settings class (`SslCommerzSettings`) with the real secret left blank in committed
  `appsettings.json` and filled in gitignored `appsettings.Development.json`.
- **Known accepted gap:** an anonymous career-portal applicant (no account) who abandons payment has
  no self-service way out of `AwaitingPayment` — `WithdrawMyApplicationAsync` is
  `[Authorize(Roles="Candidate")]`. This mirrors an existing gap (anonymous applicants can't
  self-withdraw `Applied` today either), not a new regression; HR can `Reject` it instead.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `Domain/Enums/Enum.cs` — new `PaymentStatusEnum`; `ApplicationStatusEnum.AwaitingPayment`.
- `Domain/Entities/Payment.cs` — new entity (`: Audit`), one-to-many with `JobApplication`.
- `Domain/Entities/JobApplication.cs` — new `Payments` navigation collection.
- `Infrastructure/Configurations/PaymentConfiguration.cs` — new EF config.
- `Application/Interfaces/Repositories/IPaymentRepository.cs` / `Infrastructure/Repositories/PaymentRepository.cs` — new.
- `Application/Common/Settings/SslCommerzSettings.cs` — new (`StoreId`, `StorePassword`, `ApiBaseUrl`, `IsSandbox`, `BackendBaseUrl`, `FrontendReturnBaseUrl`).
- `Application/Interfaces/Externals/ISslCommerzPaymentGateway.cs` / `Infrastructure/Services/SslCommerzPaymentGateway.cs` — new gateway client (Session API + Validation API).
- `Application/Interfaces/Services/IPaymentService.cs` / `Application/Services/PaymentService.cs` — new (`InitiateAsync`, `HandleIpnAsync`, `GetStatusAsync`, `GetJobApplicationIdByTransactionIdAsync`).
- `Application/Features/Payments/Models/PaymentModels.cs` — new (`PaymentInitiateResponse`, `PaymentStatusResponse`).
- `Controllers/PaymentController.cs` — new (`initiate`, `status`, `ipn`, `callback/success|fail|cancel`).
- `Application/Common/Exceptions/SslCommerzUnavailableException.cs` — new; `Middlewares/GlobalExceptionHandlerMiddleware.cs` — new catch block (503).
- `Application/Services/JobApplicationService.cs` — `SubmitAsync` payment gating; `LegalStatusTransitions[AwaitingPayment]`.
- `Application/Features/JobPostings/Models/JobApplicationResponse.cs` — new `PaymentRequired`/`PaymentRedirectUrl`.
- `Application/Mappings/JobPostingMapper.cs` — `ToResponse(JobApplication)` maps `PaymentRequired` from entity status.
- `Infrastructure/Extensions/DependencyInjection.cs` — new registrations (`IPaymentRepository`, `SslCommerzSettings`, typed `ISslCommerzPaymentGateway` client).
- `Infrastructure/Data/ApplicationDBContext.cs` — new `DbSet<Payment>`.
- `appsettings.json` — empty-secret `SslCommerz` placeholder section.
- `appsettings.Development.json` (gitignored) — real sandbox `SslCommerz` credentials.
- New migration `AddPayment`.
- Tests: `SylviaNG.Recruitment.Tests/Services/PaymentServiceTests.cs`, `SylviaNG.Recruitment.Tests/Controllers/PaymentControllerTests.cs` (new), `SylviaNG.Recruitment.Tests/Services/JobApplicationServiceTests.cs` (new `SubmitAsync` payment-gating cases).

**US-007 AC4 gap closure (same branch):**
- `Domain/Entities/ApplicationSetting.cs` — new singleton entity (`: Audit`).
- `Infrastructure/Configurations/ApplicationSettingConfiguration.cs` — new EF config + seed row (`Id=1`, threshold `0`).
- `Application/Interfaces/Repositories/IApplicationSettingRepository.cs` / `Infrastructure/Repositories/ApplicationSettingRepository.cs` — new.
- `Application/Features/ApplicationSettings/Models/ApplicationSettingModels.cs` — new (`ApplicationSettingResponse`, `ApplicationSettingUpdateRequest`).
- `Application/Interfaces/Services/IApplicationSettingService.cs` / `Application/Services/ApplicationSettingService.cs` — new (`GetAsync`, `UpdateAsync`, `GetMinimumProfileCompletenessPercentageAsync`).
- `Controllers/ApplicationSettingController.cs` — new (`GET`/`PUT recruitment/application-settings`).
- `Application/Mappings/CandidateProfileMapper.cs` — `CalculateCompleteness` made `public`.
- `Application/Services/JobApplicationService.cs` — new `EnsureMinimumProfileCompletenessAsync` gate called from `SubmitAsync`.
- `Infrastructure/Data/ApplicationDBContext.cs` — new `DbSet<ApplicationSetting>`.
- `Infrastructure/Extensions/DependencyInjection.cs` / `Application/Extensions/DependencyInjection.cs` — new registrations.
- New migration `AddApplicationSetting`.
- Tests: `SylviaNG.Recruitment.Tests/Services/JobApplicationServiceTests.cs` (4 new `SubmitAsync` completeness-gating cases: below threshold throws, meets threshold succeeds, no-profile bypasses, Admin source bypasses).
- **Scope note:** backend logic only — no admin-UI screen was requested for editing the threshold (candidate closed this as "missing logic", not a new UI feature); the API (`PUT recruitment/application-settings`) is ready for a future settings screen if one gets prioritized.

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/interfaces/recruitment-management/payment.interface.ts` — new.
- `@core/services/recruitment/payment/payment.service.ts` — new.
- `career-portal/payment-result/*` / `pages/internal-job-board/internal-payment-result/*` — new twin components (result-page polling, mirrors the existing apply-form/internal-apply-form twin convention).
- `@core/interfaces/recruitment-management/career-portal.interface.ts` — `IJobApplicationSubmitResponse` gains `paymentRequired`/`paymentRedirectUrl`.
- `career-portal/apply-form/*` / `pages/internal-job-board/internal-apply-form/*` — redirect-to-payment on success, retry-payment banner on gateway outage.
- `career-portal/career-portal-routing.module.ts` / `pages/internal-job-board/internal-job-board-routing.module.ts` — new `payment-result` route.
- `career-portal/job-detail/job-detail.component.html` / `pages/internal-job-board/internal-job-detail/internal-job-detail.component.html` — extended fee copy.

## Verification

- `dotnet test` — 235/235 passing (`PaymentServiceTests`, `PaymentControllerTests`, 4 `JobApplicationServiceTests` payment-gating cases, plus 4 new AC4 completeness-gating cases).
- `dotnet ef database update` applied `AddApplicationSetting` cleanly against local Postgres; seed row (`Id=1`, threshold `0`) inserted.
- `dotnet run` boots clean with the new DI registrations; `GET /recruitment/application-settings` correctly 401s unauthenticated (global auth filter), `GET /recruitment/career-portal/job-postings` still 200s unaffected.
- `npx tsc --noEmit` (frontend) — compiles clean.
- End-to-end against local Docker Postgres + `dotnet run` (5208) + `ng serve` (4600), driven with Playwright, against the **real SSLCommerz sandbox** (not mocked):
  - Configured a fee (500 BDT) on a live job posting via the admin API.
  - Anonymous career-portal apply → response carried `paymentRequired: true` + a genuine SSLCommerz `GatewayPageURL`; browser redirected there; checkout page showed "PAY 500 BDT" (screenshot-verified).
  - Completed a real sandbox test-card payment (OTP simulation page's "Success" button) → SSLCommerz's browser-return callback correctly redirected to `/careers/payment-result`, which showed a "Confirming your payment..." polling state (screenshot-verified) since the real IPN — being genuinely server-to-server — cannot reach a local dev machine.
  - Captured the real `tran_id`/`val_id` SSLCommerz issued and posted them to the local `payment/ipn` endpoint (simulating the otherwise-unreachable webhook delivery) → `PaymentService.HandleIpnAsync` called the **real** SSLCommerz Validation API, cross-checked amount/currency/tran_id, and correctly flipped `Payment.PaymentStatus → Success` and `JobApplication.ApplicationStatus: AwaitingPayment → Applied`, with a `system:sslcommerz-ipn`-attributed status-history row.
  - Redelivered the same IPN a second time → confirmed idempotent (`PaymentStatus` stayed `Success`, exactly one status-history row, no duplicate).
  - Applied to a no-fee-configured posting → confirmed unchanged behavior (`Applied` immediately, `paymentRequired: false`, no redirect).
  - HR rejected a stale `AwaitingPayment` application with a reason → confirmed the new legal transition works via the existing ATS status-update endpoint.
- **Known local-dev limitation:** the real IPN webhook cannot reach `localhost` from SSLCommerz's servers (a public callback URL, e.g. via ngrok, would be needed for genuine inbound delivery) — worked around above by manually replaying the real, captured `val_id` to prove the validation logic itself is correct against the live gateway. Not a code gap, a local-network reachability constraint that resolves itself once deployed with a public backend URL.
