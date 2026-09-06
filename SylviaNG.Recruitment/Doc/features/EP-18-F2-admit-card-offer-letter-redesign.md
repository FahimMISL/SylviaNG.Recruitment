# EP-18 F2: Admit Card + Offer Letter Redesign

## Epic

EP-18: Unified Document Design System — see [EP-18-F1-document-branding-foundation.md](EP-18-F1-document-branding-foundation.md) for the shared component layer this feature consumes.

## Why

F1 built `CompanyBranding`, `IBrandingResolverService`, and six shared QuestPDF `IComponent`s but nothing consumed them yet. F2 is the reference implementation, proving the pattern works across both document shapes MISL generates today: a compact card (admit card) and a full-page letter (offer letter), before F3 rolls the same layer onto the remaining 8 generators.

## Scope (this feature)

1. **`IAdmitCardPdfGeneratorService.Generate` / `IOfferLetterPdfGeneratorService.Generate`** — both changed to `Task<byte[]>` since branding resolution (`IBrandingResolverService.GetActiveBrandingAsync`) is async. `IOfferLetterPdfGeneratorService.Generate` also gained a `long sequenceValue` parameter (see Decisions locked).
   - 5 call sites updated to `await`: `ExamNotificationService.SendEmailAsync`, `ExamTakingAdmitCardDownloadHandler.Handle`, `ExamSeatPlanService.GenerateAdmitCardPdfAsync`, `ExamSeatPlanService.GenerateAdmitCardZipAsync`, `OfferLetterService.GenerateAsync`.
2. **`QuestPdfAdmitCardGenerator`** rewritten onto the shared layer:
   - `DocumentHeaderComponent` / `DocumentFooterComponent` replace the hand-rolled header/footer.
   - Candidate + exam details moved into two `InfoSectionComponent`s ("Candidate Details", "Exam Details" — including the existing static invigilator-contact stub line, unchanged from US-057).
   - `QrCodeComponent` (payload = the document's reference number) and `SignatureBlockComponent` ("Examination Controller" / "Authorized Signatory", no stored image → blank sign-line, same stub convention) added.
   - Page margins/font/text color, a branding-driven border/corner-radius frame around the content, and an optional rotated company-name watermark now all come from `CompanyBranding` instead of being hardcoded.
   - Photo box (candidate photo, `TryLoadPhoto`-style loading) kept as-is — no shared photo component exists yet.
3. **`QuestPdfOfferLetterGenerator`** rewritten the same way (header/footer/signature/QR/border/watermark/branding styling); the rendered template body itself is untouched free-form text (no structured fields to put into an `InfoSectionComponent`).
4. Reference numbers: `ReferenceNumberComponent.BuildReferenceNumber(branding, "ADMIT", exam.ScheduledStartAt.Year, enrollment.ExamEnrollmentId)` for admit cards, `BuildReferenceNumber(branding, "OFFER", DateTime.UtcNow.Year, sequenceValue)` for offer letters.

## Decisions locked

- **Offer letter sequence value = `JobApplicationId`**, not `OfferLetterId`. The `OfferLetter` entity doesn't have a DB id yet at the point `Generate` is called (it's saved *after* PDF generation in `OfferLetterService.GenerateAsync`), and reordering that save wasn't worth it for this feature. This matches `ReferenceNumberComponent`'s already-documented "calling entity's own id, not gap-free" limitation from F1.
- **Border style `Double`** is approximated as a thicker (2pt) single border — QuestPDF has no built-in double-line border primitive, and a true nested-double-border container was judged not worth the complexity for this feature.
- **Watermark opacity** maps `CompanyBranding.WatermarkOpacity` (0-100) onto `Color.WithAlpha(byte)` (0-255) via simple percentage scaling.
- No new shared components were added in this feature — everything reuses F1's six components as-is.

## Out of scope (later features)

- Rolling the shared layer onto the remaining 8 generators (F3: SeatPlan, Cv, CandidateProfile, AppointmentLetter, JoiningBooklet, MedicalLetter, TargetLetter, OfficeNote).
- Angular branding-settings admin screen (F3).

## Key files

- `Application/Interfaces/Services/IAdmitCardPdfGeneratorService.cs`, `IOfferLetterPdfGeneratorService.cs` (signature changes)
- `Infrastructure/Documents/QuestPdfAdmitCardGenerator.cs`, `QuestPdfOfferLetterGenerator.cs` (rewritten)
- `Application/Services/ExamNotificationService.cs`, `ExamSeatPlanService.cs`, `OfferLetterService.cs`, `Application/Features/ExamTaking/Queries/ExamTakingAdmitCardDownload/ExamTakingAdmitCardDownloadHandler.cs` (await call sites)
- `SylviaNG.Recruitment.Tests/Services/{OfferLetterServiceTests,ExamNotificationServiceTests,ExamTakingAdmitCardDownloadHandlerTests,ExamSeatPlanServiceTests}.cs` (mocks updated to `ReturnsAsync`)
