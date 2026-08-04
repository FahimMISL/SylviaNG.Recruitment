using Microsoft.AspNetCore.Hosting;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Documents.Shared;

namespace SylviaNG.Recruitment.Infrastructure.Documents
{
    /// <summary>
    /// EP-18 F2: branded admit card - reference implementation for the shared QuestPDF component
    /// layer built in EP-18 F1 (DocumentHeaderComponent/DocumentFooterComponent/
    /// InfoSectionComponent/QrCodeComponent/SignatureBlockComponent) plus CompanyBranding-driven
    /// page styling (margins, font, colors, border, watermark - the latter two via the DocumentFrame
    /// helper extracted in F3). Same overall content shape as before (US-055/US-056/US-057),
    /// resolved through IBrandingResolverService instead of hardcoded values.
    /// </summary>
    public class QuestPdfAdmitCardGenerator : IAdmitCardPdfGeneratorService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IBrandingResolverService _brandingResolverService;

        public QuestPdfAdmitCardGenerator(IWebHostEnvironment environment, IBrandingResolverService brandingResolverService)
        {
            _environment = environment;
            _brandingResolverService = brandingResolverService;
        }

        public async Task<byte[]> Generate(ExamEnrollment enrollment, Exam exam, JobApplication jobApplication)
        {
            var branding = await _brandingResolverService.GetActiveBrandingAsync();
            var photoBytes = RelativeFileLoader.TryLoad(_environment, jobApplication.CandidateProfile?.ProfilePhotoPath);
            var signatureBytes = RelativeFileLoader.TryLoad(_environment, jobApplication.CandidateProfile?.SignaturePath);
            var logoBytes = RelativeFileLoader.TryLoad(_environment, branding.LogoFilePath);
            var referenceNumber = ReferenceNumberComponent.BuildReferenceNumber(branding, "ADMIT", exam.ScheduledStartAt.Year, enrollment.ExamEnrollmentId);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    // Every other generated document in this codebase uses A4 (see
                    // QuestPdfSeatPlanGenerator, QuestPdfCvGenerator, etc.) - A5.Landscape() is
                    // only ~148mm tall, not enough room for this layout, which was spilling the
                    // trailing "IMPORTANT" notice onto a second page. A4.Landscape() keeps the
                    // wide two-column layout but with ~210mm of height instead.
                    page.Size(PageSizes.A4.Landscape());
                    page.MarginTop(branding.MarginTop);
                    page.MarginBottom(branding.MarginBottom);
                    page.MarginLeft(branding.MarginLeft);
                    page.MarginRight(branding.MarginRight);
                    page.DefaultTextStyle(x => x
                        .FontSize(8)
                        .FontFamily(string.IsNullOrWhiteSpace(branding.FontFamily) ? "Helvetica" : branding.FontFamily)
                        .FontColor(DocumentFrame.ResolveColor(branding.SecondaryColor, Colors.Grey.Darken3)));

                    page.Header().Component(new DocumentHeaderComponent(branding, "Exam Admit Card", referenceNumber, exam.ScheduledStartAt, logoBytes));
                    page.Content().PaddingTop(8).Element(content => DocumentFrame.ComposeWatermarked(content, branding,
                        inner => ComposeFramedContent(inner, branding, referenceNumber, enrollment, exam, jobApplication, photoBytes, signatureBytes)));
                    page.Footer().Element(footer => footer.Component(new DocumentFooterComponent(branding)));
                });
            });

            return document.GeneratePdf();
        }

        private static void ComposeFramedContent(IContainer container, CompanyBranding branding, string referenceNumber, ExamEnrollment enrollment, Exam exam, JobApplication jobApplication, byte[]? photoBytes, byte[]? signatureBytes)
        {
            var primaryColor = DocumentFrame.ResolveColor(branding.PrimaryColor, Colors.Brown.Darken2);
            var accentColor = DocumentFrame.ResolveColor(branding.AccentColor, Colors.Brown.Lighten4);

            DocumentFrame.ApplyBorder(container, branding).Padding(8).Column(column =>
            {
                column.Spacing(6);

                var venueText = exam.ExamVenue != null
                    ? $"{exam.ExamVenue.VenueName} - {exam.ExamVenue.Location}"
                    : "Online";
                var roomText = enrollment.ExamRoom?.RoomName;
                var seatText = enrollment.SeatNumber ?? "To be assigned";
                var examFields = new List<(string, string)>
                {
                    ("Exam Date/Time", $"{exam.ScheduledStartAt:dddd, dd MMM yyyy HH:mm}"),
                    ("Duration", $"{exam.DurationMinutes} minutes"),
                    ("Venue", venueText)
                };

                if (!string.IsNullOrWhiteSpace(roomText))
                    examFields.Add(("Room", roomText!));

                examFields.Add(("Seat Number", seatText));
                examFields.Add(("Pass Marks", $"{exam.PassMarks} / {exam.TotalMarks}"));
                examFields.Add(("Invigilator Contact", "Contact exam venue administration"));

                column.Item().AlignCenter().Background(primaryColor).PaddingVertical(6).PaddingHorizontal(18)
                    .Text("ADMIT CARD").FontSize(16).Bold().FontColor(Colors.White);
                column.Item().AlignCenter().Text("Examination Admit Card").FontSize(9).SemiBold();

                column.Item().PaddingTop(2).Row(row =>
                {
                    row.RelativeItem(3).Column(infoColumn =>
                    {
                        infoColumn.Spacing(3);

                        infoColumn.Item().Text("Candidate information").FontSize(9).SemiBold().FontColor(primaryColor);
                        infoColumn.Item().Component(new InfoSectionComponent(branding, string.Empty, new List<(string, string)>
                        {
                            ("Candidate Name", jobApplication.CandidateName),
                            ("Application ID", jobApplication.JobApplicationId.ToString())
                        }));

                        infoColumn.Item().PaddingTop(2).Text("Please bring this card and a valid photo ID to the venue.")
                            .FontSize(7).Italic().FontColor(Colors.Grey.Darken1);
                    });

                    row.ConstantItem(92).Column(photoColumn =>
                    {
                        if (photoBytes != null)
                        {
                            photoColumn.Item().Height(74).Width(74).Border(1).BorderColor(accentColor).Padding(2).Image(photoBytes).FitArea();
                        }
                        else
                        {
                            photoColumn.Item().Height(74).Width(74).Border(1).BorderColor(accentColor)
                                .AlignCenter().AlignMiddle().Text("No Photo").FontSize(8).FontColor(Colors.Grey.Medium);
                        }

                        photoColumn.Item().PaddingTop(2).AlignCenter().Element(signature =>
                            ComposeCandidateSignature(signature, jobApplication.CandidateName, signatureBytes));
                    });
                });

                column.Item().Row(row =>
                {
                    row.RelativeItem(3).Column(detailsColumn =>
                    {
                        detailsColumn.Item().Background(primaryColor).Padding(4).Text("EXAMINATION DETAILS")
                            .FontSize(8).Bold().FontColor(Colors.White);
                        detailsColumn.Item().Border(1).BorderColor(accentColor).Padding(5)
                            .Component(new InfoSectionComponent(branding, string.Empty, examFields));
                    });

                    row.ConstantItem(100).PaddingLeft(8).Column(sidebar =>
                    {
                        sidebar.Item().Background(primaryColor).Padding(4).Text("QR CODE")
                            .FontSize(8).Bold().FontColor(Colors.White);
                        sidebar.Item().Border(1).BorderColor(accentColor).Padding(4).AlignCenter()
                            .Component(new QrCodeComponent(branding, referenceNumber, 54));
                        sidebar.Item().AlignCenter().Text("Scan to verify this admit card").FontSize(6).FontColor(Colors.Grey.Darken1);
                    });
                });

                column.Item().Background(accentColor).Padding(5).Text(text =>
                {
                    text.Span("IMPORTANT: ").Bold().FontColor(primaryColor);
                    text.Span("Arrive at least 30 minutes early. Mobile phones and electronic devices are not permitted in the examination room.")
                        .FontSize(7);
                });
            });
        }

        private static void ComposeCandidateSignature(IContainer container, string candidateName, byte[]? signatureBytes)
        {
            container.Width(74).Column(column =>
            {
                if (signatureBytes != null)
                    column.Item().Height(18).Image(signatureBytes).FitArea();
                else
                    column.Item().Height(14).BorderBottom(1).BorderColor(Colors.Grey.Medium);

                column.Item().AlignCenter().Text(candidateName).FontSize(6).SemiBold();
                column.Item().AlignCenter().Text("Candidate signature").FontSize(5).FontColor(Colors.Grey.Darken1);
            });
        }
    }
}
