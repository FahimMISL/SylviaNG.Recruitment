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
    /// EP-18 F3: branded seat plan, grouped by room (US-056). Full roster document - no
    /// signature block (no single signee for a room-by-room list), but keeps a QR code for
    /// verification, same as the other branded documents.
    /// </summary>
    public class QuestPdfSeatPlanGenerator : ISeatPlanPdfGeneratorService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IBrandingResolverService _brandingResolverService;

        public QuestPdfSeatPlanGenerator(IWebHostEnvironment environment, IBrandingResolverService brandingResolverService)
        {
            _environment = environment;
            _brandingResolverService = brandingResolverService;
        }

        public async Task<byte[]> Generate(Exam exam, List<ExamEnrollment> enrollments)
        {
            var branding = await _brandingResolverService.GetActiveBrandingAsync();
            var logoBytes = RelativeFileLoader.TryLoad(_environment, branding.LogoFilePath);
            var referenceNumber = ReferenceNumberComponent.BuildReferenceNumber(branding, "SEATPLAN", exam.ScheduledStartAt.Year, exam.ExamId);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.MarginTop(branding.MarginTop);
                    page.MarginBottom(branding.MarginBottom);
                    page.MarginLeft(branding.MarginLeft);
                    page.MarginRight(branding.MarginRight);
                    page.DefaultTextStyle(x => x
                        .FontSize(10)
                        .FontFamily(string.IsNullOrWhiteSpace(branding.FontFamily) ? "Helvetica" : branding.FontFamily)
                        .FontColor(DocumentFrame.ResolveColor(branding.SecondaryColor, Colors.Grey.Darken3)));

                    page.Header().Component(new DocumentHeaderComponent(branding, exam.Title, referenceNumber, exam.ScheduledStartAt, logoBytes));
                    page.Content().PaddingTop(10).Element(content => DocumentFrame.ComposeWatermarked(content, branding,
                        inner => ComposeFramedContent(inner, branding, referenceNumber, exam, enrollments)));
                    page.Footer().Element(footer => footer.Component(new DocumentFooterComponent(branding)));
                });
            });

            return document.GeneratePdf();
        }

        private static void ComposeFramedContent(IContainer container, CompanyBranding branding, string referenceNumber, Exam exam, List<ExamEnrollment> enrollments)
        {
            var primaryColor = DocumentFrame.ResolveColor(branding.PrimaryColor, Colors.Brown.Darken2);
            var accentColor = DocumentFrame.ResolveColor(branding.AccentColor, Colors.Brown.Lighten4);

            DocumentFrame.ApplyBorder(container, branding).Padding(10).Column(column =>
            {
                column.Spacing(4);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Background(primaryColor).Padding(5).Text("EXAM SEAT PLAN").FontSize(11).Bold().FontColor(Colors.White);
                    row.ConstantItem(60).Component(new QrCodeComponent(branding, referenceNumber));
                });

                column.Item().PaddingBottom(6).Text(text =>
                {
                    text.Span("Scheduled: ").SemiBold();
                    text.Span($"{exam.ScheduledStartAt:dddd, dd MMM yyyy HH:mm} ({exam.DurationMinutes} min)");
                });

                if (exam.ExamVenue != null)
                {
                    column.Item().PaddingBottom(6).Text(text =>
                    {
                        text.Span("Venue: ").SemiBold();
                        text.Span($"{exam.ExamVenue.VenueName} - {exam.ExamVenue.Location}");
                    });
                }

                var groups = enrollments
                    .GroupBy(e => e.ExamRoom?.RoomName ?? "Unassigned")
                    .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase);

                foreach (var group in groups)
                {
                    column.Item().Element(section => ComposeRoomSection(section, primaryColor, accentColor, group.Key, group
                        .OrderBy(e => e.SeatNumber, StringComparer.OrdinalIgnoreCase)
                        .ToList()));
                }
            });
        }

        private static void ComposeRoomSection(IContainer container, Color primaryColor, Color accentColor, string roomName, List<ExamEnrollment> enrollments)
        {
            container.Column(column =>
            {
                column.Item().Background(primaryColor).Padding(4)
                    .Text(roomName).FontSize(10).Bold().FontColor(Colors.White);

                column.Item().PaddingTop(6).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(4);
                        columns.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(cell => HeaderCell(cell, accentColor)).Text("Seat Number");
                        header.Cell().Element(cell => HeaderCell(cell, accentColor)).Text("Candidate Name");
                        header.Cell().Element(cell => HeaderCell(cell, accentColor)).Text("Application ID");
                    });

                    foreach (var enrollment in enrollments)
                    {
                        table.Cell().Element(BodyCell).Text(enrollment.SeatNumber ?? "-");
                        table.Cell().Element(BodyCell).Text(enrollment.JobApplication?.CandidateName ?? "-");
                        table.Cell().Element(BodyCell).Text(enrollment.JobApplicationId.ToString());
                    }
                });
            });
        }

        private static IContainer HeaderCell(IContainer container, Color accentColor) =>
            container.Background(accentColor).DefaultTextStyle(x => x.SemiBold().FontSize(9)).Padding(3);

        private static IContainer BodyCell(IContainer container) =>
            container.PaddingVertical(3).DefaultTextStyle(x => x.FontSize(9));
    }
}
