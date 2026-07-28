# EP-18 F3: Remaining Generators + Branding Settings UI

## Epic

EP-18: Unified Document Design System — see [EP-18-F1-document-branding-foundation.md](EP-18-F1-document-branding-foundation.md) and [EP-18-F2-admit-card-offer-letter-redesign.md](EP-18-F2-admit-card-offer-letter-redesign.md).

## Why

F2 proved the shared branding layer on 2 of 10 generators. F3 finishes real coverage across all 10 and gives HR/Admin an actual way to edit branding (F1's `CompanyBranding` was only ever DB-seeded).

## Scope (this feature)

**1. Three new shared helpers** (`Infrastructure/Documents/Shared/`), extracted because F2's two generators' patterns would otherwise have been copy-pasted a 6th–10th time:
- `RelativeFileLoader.TryLoad` — the wwwroot-relative-file-load-or-null block (logo/photo loading), replacing duplicated `TryLoadPhoto`/`TryLoadRelativeFile` copies.
- `DocumentFrame.ApplyBorder` / `ComposeWatermarked` — the branding-driven border/corner-radius + rotated-watermark-layer logic, replacing the copy in each F2 generator.
- `BrandedLetterPdfComposer.Compose` — the entire "letter" document shape (header/body-lines/signature/QR/footer). `QuestPdfOfferLetterGenerator` (F2) was refactored onto it; `QuestPdfAppointmentLetterGenerator`, `QuestPdfJoiningBookletGenerator`, `QuestPdfMedicalLetterGenerator`, `QuestPdfTargetLetterGenerator`, `QuestPdfOfficeNoteGenerator` are now thin wrappers around it.

**2. All 8 remaining generators migrated** onto `IBrandingResolverService` + the shared layer (`Task<byte[]>` now, was sync `byte[]`):
- **SeatPlan** — full roster kept as a custom room-grouped table; branded header/footer/frame + QR only (no signee for a multi-candidate document). Reference: `SEATPLAN`/`examId`.
- **Cv**, **CandidateProfile** — custom sections (education table, work experience, skills, certifications) kept as-is; branded header/footer/frame added. No QR/signature (personal exports, not official outbound letters). Reference: `CV`/`PROFILE` on `CandidateProfileId`.
- **AppointmentLetter, JoiningBooklet, MedicalLetter, TargetLetter, OfficeNote** — now call `BrandedLetterPdfComposer`. Reference sequence value = `offerLetter.OfferLetterId` where an `OfferLetter` is already loaded in scope (all but OfficeNote), else `jobApplication.JobApplicationId` (OfficeNote). `JoiningBookletService` has two call sites with different available ids (`offerLetter.OfferLetterId` on first generation, `booklet.JoiningBookletId` on a later bulk re-download) — both correct for their own context, just not the same number, a minor documented quirk.
- Async ripple: 5 letter-generator interfaces gained a `long sequenceValue` param (same as F2's `IOfferLetterPdfGeneratorService`); `ISeatPlanPdfGeneratorService`/`ICvPdfGeneratorService`/`ICandidateProfilePdfGeneratorService` needed no new params (the sequence value was already on the entity/argument passed in). 9 call sites across `ExamSeatPlanService`, `CvBankCvDownloadHandler`, `CvBankCvBulkDownloadHandler`, `CvZipBuilder`, `CandidateProfileService`, `AppointmentLetterService`, `JoiningBookletService` (×2), `MedicalLetterService`, `TargetLetterService`, `OfficeNoteService` updated with `await` — all were already `async` methods. Mocks in 8 test files updated `.Returns` → `.ReturnsAsync`.

**3. Branding settings admin API** (`Controllers/CompanyBrandingController.cs`, all actions `[Authorize(Roles = "Admin")]`):
- `ICompanyBrandingService`/`CompanyBrandingService` — separate from `IBrandingResolverService` (read-only, used by generators). `GetAsync`/`UpdateAsync` reuse `ICompanyBrandingRepository.Update` + `IUnitOfWork.SaveChangesAsync` (no new repo method needed); creates the tenant's row on first save if F1's in-memory default was never persisted.
- `UploadLogoAsync` — same `IFileStorageService.SaveAsync`/`DeleteAsync` pattern as `CandidateProfileService.UploadMediaAsync` (subfolder `company-branding`).
- `GeneratePreviewPdfAsync` + new `IBrandingPreviewPdfGeneratorService`/`BrandingPreviewPdfGenerator` — renders a sample page using all 6 F1 components with canned data (productionized version of `DocumentBrandingComponentsSmokeTests`). Takes an **unsaved** draft `CompanyBranding` built from the request body merged over the persisted logo/margins/etc, so "Preview PDF" reflects in-progress edits before Save.
- Editable field set this round (trimmed, per the "minimal" ask): company identity/contact, 3 colors, font, border style, watermark toggle+opacity, page-number toggle. Margins/corner-radius/layout-variant/QR-signature-seal-position/reference-format fields stay at their F1 defaults — a later, fuller admin-editor round.

**4. Angular `branding-settings-management` module** — mirrors `application-settings-management` exactly (reactive form, `p-floatlabel`/`p-select`/`p-inputnumber`/`p-checkbox`, load/save/error-success pattern). New pieces: native `<input type="color">` × 3 (matches the only existing color-picker precedent in this codebase, `manage-hiring-pipeline.component.html`), an immediate-upload-on-select logo widget, and a Preview PDF button using the same blob/`saveFileResponse` download convention as every other PDF download in this app. Route `/branding-settings`, nav entry under "System Administration" (admin-only), both new.

## Verification

- `dotnet build` clean; `dotnet test` — 710/710 passing (no regressions from the interface/call-site ripple).
- `ng build --configuration development` clean, no template/type errors.
- Live: downloaded a real branded Seat Plan PDF (exam 1), a real branded CV (candidate 4), and generated a real branded Appointment Letter (offer letter 4) via their actual HR-facing endpoints — all render correctly (header/footer/border/watermark/reference number, QR+signature on the letter, no QR/signature on the CV/seat plan as designed).
- Live: `GET`/`PUT`/`POST logo`/`POST preview/pdf` on `/recruitment/company-branding` all exercised via curl — update persists, preview correctly reflects unsaved draft values (different colors/border/font/watermark than the saved state) rather than the saved ones.
- Live (Playwright, headless Chromium): logged in as Admin, loaded `/branding-settings` — form pre-populates from the saved settings, "Branding Settings" appears correctly under System Administration in the sidebar, changing Primary Color and clicking Preview PDF triggers a real file download, zero browser console errors.
