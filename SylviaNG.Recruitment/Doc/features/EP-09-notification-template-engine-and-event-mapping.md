# EP-09 Feature 1 — Notification Template Engine + Event Mapping (US-073, US-074)

## What

An admin-managed notification content engine: `NotificationTemplate` per delivery `Channel` (Email/SMS/In-App/Push), each edit auto-snapshotted into `NotificationTemplateVersion` history; `{{Placeholder}}` substitution via a new `PlaceholderSubstitutionService` (render + auto-detect); and `EventTemplateMapping`, resolving `(RecruitmentEvent, Channel, RecipientType)` → a specific template, so the same event can notify both the Candidate and Admin/HR with different content. Admin CRUD screens for both live under System Administration.

## Why

First of 3 EP-09 features (7 stories, US-073–US-079). Everything downstream — auto-send on status change, bulk notify, admit-card email — reads from this template/mapping layer, so it had to land first. Built off `demo/local-showcase` (not `dev`, which is 297 files behind and missing the Master Data Admin pattern this reuses architecturally).

Also configured dev SMTP (Gmail, `appsettings.Development.json`, gitignored — never committed) since real email is how EP-09 will be verified going forward.

## Scope boundary — nothing sends yet

This feature builds the engine only. **No dispatch wiring**: `ApplicationStatusChangedEvent` (already raised by `JobApplicationService`, already documented as "EP-09's job") is still not consumed by anything. Hooking real events into `EventTemplateMapping` — status-change auto-send, bulk notify, admit-card email with download link — is Feature 2 (US-075/076/077). OTP-gated candidate login (a separate ask, auth-flow not notification-content work) is also deferred to Feature 2 planning, not built here.

## Design decisions

- **Enums, not lookup tables**, for `RecruitmentEvent`/`Channel`/`RecipientType` — appended to the codebase's single `Domain/Enums/Enum.cs`, string-persisted (`HasConversion<string>()`), matching every other fixed/code-known "type" concept in this codebase (`ApplicationStatusEnum`, `InterviewStatusEnum`, etc). No lookup-table precedent exists for a closed set like this — lookup tables here are reserved for genuinely admin-growable master data (Degree, Country).
- **`RecipientType` is a mapping-key dimension, not a template field.** User wants every application/withdrawal/progress/required-action event to notify both the Candidate and Admin/HR, and those two audiences need different content — so `EventTemplateMapping` is unique on `(RecruitmentEvent, Channel, RecipientType)`, letting the same event+channel resolve to two independent templates side by side.
- **Update never overwrites history.** `NotificationTemplateService.UpdateAsync` bumps `CurrentVersionNumber` and inserts a new `NotificationTemplateVersion` row with the new content on every save — the template row itself always mirrors the latest version for fast reads. No content-versioning precedent existed anywhere in this codebase; `ApplicationStatusHistory`'s FK+cascade shape was borrowed structurally only (that one logs status transitions, not content snapshots).
- **Channel and Code are immutable after create** (`NotificationTemplateUpdateRequest` omits both) — changing either would silently invalidate any `EventTemplateMapping` already pointing at this template.
- **Placeholder engine is new** — confirmed via full read of `ExamNotificationService`/`InterviewNotificationService` that neither has any substitution logic; both hardcode bodies via C# string interpolation. `PlaceholderSubstitutionService.Render` leaves unmatched `{{Token}}`s visible rather than blanking them (a visible unresolved token is a better failure mode than a silent gap), case-insensitive key match.
- **Business-rule validation lives in the service, thrown as `FluentValidation.ValidationException`** (channel-mismatch check: a template's `Channel` must match the mapping's `Channel`) — matches the existing codebase convention (`JobApplicationService`, `ShortlistFilterEvaluationService`, etc. all throw this manually from services, not just from generated validators) rather than introducing an unprecedented `MustAsync` validator pattern.
- **Frontend: dedicated components, not the generic Master Data Admin config engine.** `MasterDataFieldType` only supports `'text' | 'number'` — too flat for a channel dropdown, conditional Subject field, textarea body, live preview, and version history. Reused the Master Data Admin module's *architecture* (routing/RoleGuard/nav pattern) and `ExamQuestionService`'s fixed-`API_URL` service shape instead, plus `manage-exam-question`'s `p-select`/`pTextarea` markup as the closest existing richer-form precedent.

## Files changed

**Backend** (`SylviaNG.Recruitment-master/SylviaNG.Recruitment-master/SylviaNG.Recruitment/`):
- `appsettings.Development.json` — dev SMTP config (gitignored, no code change needed — `SmtpEmailService` already reads this section).
- `Domain/Enums/Enum.cs` — `RecruitmentEventEnum`, `NotificationChannelEnum`, `NotificationRecipientTypeEnum` appended.
- `Domain/Entities/NotificationTemplate.cs`, `NotificationTemplateVersion.cs`, `EventTemplateMapping.cs` — new.
- `Infrastructure/Configurations/NotificationTemplateConfiguration.cs`, `NotificationTemplateVersionConfiguration.cs`, `EventTemplateMappingConfiguration.cs` — new.
- `Infrastructure/Data/ApplicationDBContext.cs` — 3 new DbSets.
- `Migrations/20260724054931_AddNotificationTemplateEngine.cs` — new migration.
- `Application/Common/Notifications/PlaceholderSubstitutionService.cs` + `Application/Interfaces/Services/IPlaceholderSubstitutionService.cs` — new.
- `Application/Interfaces/{Repositories,Services}/I{NotificationTemplate,EventTemplateMapping}{Repository,Service}.cs` + `Infrastructure/Repositories/`, `Application/Services/` implementations — new.
- `Application/Features/NotificationTemplates/**` — CQRS: Create/Update/Delete/GetAll/GetById/GetVersions/Preview.
- `Application/Features/EventTemplateMappings/**` — CQRS: Create/Update/Delete/GetAll.
- `Application/Mappings/NotificationTemplateMapper.cs`, `EventTemplateMappingMapper.cs` — new.
- `Controllers/NotificationTemplateController.cs` (`recruitment/notification-template`), `EventTemplateMappingController.cs` (`recruitment/event-template-mapping`) — new, every endpoint `[Authorize(Roles = "Admin")]` (unlike Degree/Country, nothing here is candidate-facing).
- `Application/Extensions/DependencyInjection.cs`, `Infrastructure/Extensions/DependencyInjection.cs` — registered new services/repositories.
- `SylviaNG.Recruitment.Tests/Services/NotificationTemplateServiceTests.cs`, `EventTemplateMappingServiceTests.cs`, `PlaceholderSubstitutionServiceTests.cs` — new (19 tests total).

**Frontend** (`sylviang.adminui.recruitment-main/sylviang.adminui.recruitment-main/src/app/`):
- `@core/enums/recruitment.enum.ts` — 3 new enums mirroring the backend ones.
- `@core/interfaces/recruitment-management/notification-template.interface.ts`, `event-template-mapping.interface.ts` — new.
- `@core/services/recruitment/notification-template/notification-template.service.ts`, `event-template-mapping/event-template-mapping.service.ts` — new.
- `pages/notification-management/` — new module: `notification-template-list`/`-form`, `event-template-mapping-list`/`-form`, routing module, module.ts.
- `pages/pages-routing.module.ts` — lazy `notification-management` route.
- `@core/constants/nav-menu-items.ts` — 2 new "System Administration" subitems.

## Verification

- `dotnet build` clean; `dotnet test` — 599/605 passing, the 6 failures are the pre-existing documented baseline (3× `InternalJobBoardControllerTests` NRE gap, 3× `AuthLoginSmokeTests` needing live infra) — unrelated, confirmed via `git status` that none of the touched files overlap. All 19 new tests pass.
- `dotnet ef database update` applied cleanly against local Postgres.
- `ng build` compiles clean, `notification-management-module` chunk generated.
- Full end-to-end via `curl` against the running local stack (logged in as a local Admin user, temp-provisioned via Keycloak admin API since no seeded Admin dev credential existed): created an Email template with placeholders, confirmed `Preview` renders correctly and returns detected placeholders; updated the template and confirmed `GetVersions` returns both snapshots newest-first with content preserved; created an Event→Template mapping; confirmed channel-mismatch is rejected (400), duplicate `(Event, Channel, RecipientType)` is rejected (409), deleting a mapped template is blocked (409); deleted the mapping then the template cleanly (200/200).
- Full end-to-end in a real headless-Chromium browser (Playwright, fetched via `npx` since no project browser-automation tool existed) against the running dev servers: logged in as Admin, navigated all 4 new pages (template list/form, mapping list/form) — all render correctly, breadcrumbs correct, dropdowns populated, zero console errors; confirmed both new nav entries appear under System Administration for the Admin role.
