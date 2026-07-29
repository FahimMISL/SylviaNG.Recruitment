# EP-17: Payment Transaction Reporting + Fee Waiver Rules (US-125/127/128)

## What

Finance/HR can now see and reconcile SSLCommerz payment activity, and Admin can configure rules
that waive the application fee for matching candidates.

- **F1 (US-125/128)** - `GET /recruitment/payment-report/transactions` (filterable, paginated
  transaction list with a running total), `GET /recruitment/payment-report/reconciliation`
  (paid/failed/waived/net summary for a period+scope), `GET
  /recruitment/payment-report/reconciliation/export?format=xlsx|pdf`.
- **F2 (US-127)** - `WaiverRuleController` (Admin-only CRUD), plus two new admin-managed lookups
  (`SpecialCategoryController`, `ReferralSourceController`) reused via the generic Master Data Admin
  engine. A matching rule skips the SSLCommerz payment step entirely at submission time.

## Why

EP-17's core payment flow (SSLCommerz checkout) shipped without any reporting surface or a way to
exempt candidates from the fee. These three stories close that gap: Finance needs visibility into
what was actually collected, and HR needs to waive the fee for internal staff, special categories,
or referred candidates without a code change per case.

## Design notes

- **`Payment` stays untouched.** No `PaymentStatusEnum.Waived` was added - a `Payment` row is
  strictly "one SSLCommerz gateway attempt" (unique `TransactionId`, gateway session fields). The
  reconciliation report's "Waived" bucket reads `JobApplication.WaiverRuleId IS NOT NULL` directly;
  no extra join since the report already needs `JobApplication` for candidate/vacancy columns.
- **Candidate Type (Internal) keys off `CandidateProfile.IsInternal`**, not
  `ApplicationSourceEnum.Internal` - the latter is the application channel (career portal vs
  internal job board vs HR-on-behalf), a different concept from whether the candidate *is* an
  internal employee.
- **Rule matching = explicit `Priority`** (int, lower evaluates first), not specificity-counting -
  avoids an ambiguous tie when two rules have the same number of non-null criteria but target
  different fields. First ACTIVE rule (ordered `Priority ASC, WaiverRuleId ASC`) whose every
  non-null criterion matches wins; a rule with every criterion null matches any candidate.
  `WaiverRuleService.TryMatchAsync`.
- **`SubmitAsync` reordering.** `CandidateProfileId` resolution now happens *before* the
  `requiresPayment` gate (it used to happen after), because the waiver check needs
  `CandidateProfile.IsInternal` up front. Waiver check only runs for `source != Admin` - Admin
  apply-on-behalf already bypasses payment unconditionally.
- **Reconciliation avoids double-counting retries.** `IPaymentRepository.GetLatestPaymentsInScopeAsync`
  filters Payment+JobApplication+JobPosting server-side, then groups by `JobApplicationId` and takes
  the latest attempt (`OrderByDescending(PaymentId)`) client-side - same "latest attempt wins" rule
  as the existing `GetLatestByJobApplicationIdAsync`, just batched. Grouping is done in C# rather
  than relying on provider `GroupBy+First()` translation, since the dataset is already period-bounded
  by the query's date filter.
- **Department/Org scoping is by raw ID only.** `JobPosting.DepartmentId`/`SiteId` are plain FK
  longs with no local Department/Site/Org table in this codebase (owned by an external Core HR
  system) - the reconciliation report filters by ID, it can't group/display by name. The frontend
  only exposes a Vacancy picker (which the backend still accepts alongside raw department/site IDs
  via query params) since there's no friendly department/site lookup to build a dropdown from.
- **`PaymentReportController` is a separate controller** from the existing `[AllowAnonymous]`
  `PaymentController` - keeps the SSLCommerz-callback surface's anonymous posture untouched; the new
  reporting endpoints are `[Authorize(Roles="Admin,HR")]`.
- **Export is synchronous** (ClosedXML for xlsx, QuestPDF + the existing shared branding components
  for pdf) - reconciliation reports are date-range-bounded, not large enough to need the async
  `ExportRequestController` queue used for bulk CV exports. The PDF is a one-page branded summary
  (paid/failed/waived/net); the row-by-row detail lives in the on-screen transaction list, not
  duplicated into the PDF.
- **FK delete behavior:** `JobApplication.WaiverRuleId` → `SetNull` (a superseded rule shouldn't
  block cleanup; `WaivedAt` + the audit-trail note preserve history independent of the live FK).
  `JobApplication.SpecialCategoryId`/`ReferralSourceId` → `Restrict` (used in rule matching at
  submit time, not just display - deleting a lookup value in use should fail loudly).
- **Audit trail reuses the existing `ApplicationStatusHistory` mechanism** - same table
  `PaymentService.HandleIpnAsync` already writes to for a real payment confirmation, with
  `ChangedByUserName = "system:fee-waiver"` and a Note citing the matched rule's name.
- **`SpecialCategory`/`ReferralSource` are net-new, simple name-only lookups** - admin-managed for
  free via the existing generic Master Data engine (`master-data.config.ts` entries + 2 backend
  controllers following the `DegreeController` pattern), since the generic engine's field types
  (`'text' | 'number'`) are enough for a plain lookup. `WaiverRule` itself needs 2 relation dropdowns
  + an enum dropdown, which the generic engine can't express, so it gets its own bespoke admin page
  (`waiver-rule-management`).
- **Candidates optionally declare Special Category/Referral Source at apply time** on both the
  external career-portal and internal-job-board apply forms - both dropdowns are populated from the
  same admin-managed lookups and hidden entirely if no options are configured yet.

## Verification

- Backend: `dotnet build` clean, `dotnet ef database update` applied migration
  `AddWaiverRulesSpecialCategoryReferralSource` against local Postgres, full suite green
  (726/726, 16 new tests covering `WaiverRuleService`, `PaymentReportService`, and the
  `SubmitAsync` waiver-skip-payment path).
- Frontend: `ng build` clean.
- Live (local dev, Admin login): created a Special Category, a Referral Source, and a Waiver Rule
  (Candidate Type = Internal, Priority 1) end to end through their admin pages. Payment Transaction
  list renders real seeded SSLCommerz transaction data with working filters and a running total.
  Reconciliation report against seeded data returned Paid=2/500.00, Failed=0, Waived=0, Net=500.00 -
  correctly deduping two applications that each had an extra `Initiated` retry row alongside their
  `Success` row. Both Excel and PDF export endpoints returned 200 with a real branded PDF (via the
  shared `DocumentHeaderComponent`/`DocumentFooterComponent`) and a real `.xlsx` workbook.
- The full candidate-submission waiver path (apply as a logged-in Internal candidate, confirm the
  application lands `Applied` with no SSLCommerz redirect) was verified via the
  `SubmitAsync_WithMatchingWaiverRule_ShouldBypassPaymentAndStampWaiverFields` unit test rather than
  a live browser run - the candidate login flow requires an OTP step that isn't scriptable through
  the browser tool in this environment (known project constraint, see candidate-login OTP gotcha in
  prior EP-13 verification notes). The apply-form dropdowns use the identical
  `MasterDataService.getAll(...)` call already confirmed live via the Waiver Rule form.

## Files touched

**Backend**
- `Domain/Entities/WaiverRule.cs`, `SpecialCategory.cs`, `ReferralSource.cs` - new.
- `Domain/Entities/JobApplication.cs` - `SpecialCategoryId`, `ReferralSourceId`, `WaiverRuleId`,
  `WaivedAt` + navigation properties.
- `Domain/Enums/Enum.cs` - `WaiverCandidateTypeEnum`.
- `Infrastructure/Configurations/WaiverRuleConfiguration.cs`, `SpecialCategoryConfiguration.cs`,
  `ReferralSourceConfiguration.cs` - new. `JobApplicationConfiguration.cs`,
  `PaymentConfiguration.cs` - new FKs/indexes.
- `Infrastructure/Data/ApplicationDBContext.cs` - 3 new `DbSet`s.
- `Migrations/20260727184937_AddWaiverRulesSpecialCategoryReferralSource.*` - new.
- `Application/Features/WaiverRules/*`, `Application/Features/SpecialCategories/*`,
  `Application/Features/ReferralSources/*` - new CQRS slices (mirrors `Application/Features/Degrees/*`).
- `Application/Features/PaymentReports/Models/*` - new.
- `Application/Interfaces/Repositories/IWaiverRuleRepository.cs`, `ISpecialCategoryRepository.cs`,
  `IReferralSourceRepository.cs` - new. `IPaymentRepository.cs`, `IJobApplicationRepository.cs` -
  new query methods for F1.
- `Application/Interfaces/Services/IWaiverRuleService.cs`, `ISpecialCategoryService.cs`,
  `IReferralSourceService.cs`, `IPaymentReportService.cs`, `IPaymentReportPdfGeneratorService.cs` - new.
- `Application/Services/WaiverRuleService.cs`, `SpecialCategoryService.cs`,
  `ReferralSourceService.cs`, `PaymentReportService.cs` - new.
- `Application/Services/JobApplicationService.cs` - waiver check wired into `SubmitAsync`.
- `Application/Mappings/WaiverRuleMapper.cs`, `SpecialCategoryMapper.cs`, `ReferralSourceMapper.cs` - new.
- `Application/Features/JobPostings/Models/JobApplicationSubmitRequest.cs` - 2 new optional fields.
- `Infrastructure/Repositories/WaiverRuleRepository.cs`, `SpecialCategoryRepository.cs`,
  `ReferralSourceRepository.cs` - new. `PaymentRepository.cs`, `JobApplicationRepository.cs` -
  new query methods.
- `Infrastructure/Documents/QuestPdfReconciliationReportGenerator.cs` - new.
- `Controllers/WaiverRuleController.cs`, `SpecialCategoryController.cs`,
  `ReferralSourceController.cs`, `PaymentReportController.cs` - new.
- `Application/Extensions/DependencyInjection.cs`, `Infrastructure/Extensions/DependencyInjection.cs`
  - new registrations.
- `SylviaNG.Recruitment.Tests/Services/WaiverRuleServiceTests.cs`,
  `PaymentReportServiceTests.cs` - new. `JobApplicationServiceTests.cs` - new mock + 1 new test.

**Frontend**
- `src/app/@core/interfaces/recruitment-management/waiver-rule.interface.ts`,
  `payment-report.interface.ts` - new. `career-portal.interface.ts` - 2 new optional fields.
- `src/app/@core/services/recruitment/waiver-rule/waiver-rule.service.ts`,
  `payment-report/payment-report.service.ts` - new. `career-portal.service.ts`,
  `internal-job-board.service.ts` - append the 2 new fields to the multipart submit.
- `src/app/pages/master-data-management/master-data.config.ts` - `specialCategory`,
  `referralSource` entries.
- `src/app/pages/waiver-rule-management/*` - new module (list + form).
- `src/app/pages/payment-management/*` - new module (transaction list + reconciliation report).
- `src/app/pages/pages-routing.module.ts` - 2 new lazy routes.
- `src/app/@core/constants/nav-menu-items.ts` - 5 new sidebar entries.
- `src/app/career-portal/apply-form/apply-form.component.ts/.html`,
  `src/app/pages/internal-job-board/internal-apply-form/internal-apply-form.component.ts/.html` -
  optional Special Category/Referral Source dropdowns.
