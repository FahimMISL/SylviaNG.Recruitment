using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Documents
{
    /// <summary>
    /// Single-page admit card for one exam enrollment (US-055/US-056). Same QuestPDF
    /// Document.Create / Compose* shape as QuestPdfCvGenerator.
    /// </summary>
    public class QuestPdfAdmitCardGenerator : IAdmitCardPdfGeneratorService
    {
        public byte[] Generate(ExamEnrollment enrollment, Exam exam, JobApplication jobApplication)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A5.Landscape());
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Darken3));

                    page.Header().Element(header => ComposeHeader(header, exam));
                    page.Content().PaddingTop(15).Element(content => ComposeContent(content, enrollment, exam, jobApplication));

                    page.Footer().AlignCenter().Text("Please bring a valid photo ID along with this admit card.")
                        .FontSize(8).FontColor(Colors.Grey.Medium);
                });
            });

            return document.GeneratePdf();
        }

        private static void ComposeHeader(IContainer container, Exam exam)
        {
            container.Column(column =>
            {
                column.Item().Text("Exam Admit Card").FontSize(18).Bold().FontColor(Colors.Blue.Darken2);
                column.Item().Text(exam.Title).FontSize(12).Italic().FontColor(Colors.Grey.Medium);
            });
        }

        private static void ComposeContent(IContainer container, ExamEnrollment enrollment, Exam exam, JobApplication jobApplication)
        {
            container.Column(column =>
            {
                column.Spacing(6);

                var venueText = exam.ExamVenue != null
                    ? $"{exam.ExamVenue.VenueName} - {exam.ExamVenue.Location}"
                    : "Online";
                var roomText = enrollment.ExamRoom?.RoomName;
                var seatText = enrollment.SeatNumber ?? "To be assigned";

                column.Item().Text(text =>
                {
                    text.Span("Candidate Name: ").SemiBold();
                    text.Span(jobApplication.CandidateName);
                });

                column.Item().Text(text =>
                {
                    text.Span("Application ID: ").SemiBold();
                    text.Span(jobApplication.JobApplicationId.ToString());
                });

                column.Item().Text(text =>
                {
                    text.Span("Photo: ").SemiBold();
                    text.Span("N/A");
                });

                column.Item().PaddingTop(8).Text(text =>
                {
                    text.Span("Exam Date/Time: ").SemiBold();
                    text.Span($"{exam.ScheduledStartAt:dddd, dd MMM yyyy HH:mm}");
                });

                column.Item().Text(text =>
                {
                    text.Span("Duration: ").SemiBold();
                    text.Span($"{exam.DurationMinutes} minutes");
                });

                column.Item().Text(text =>
                {
                    text.Span("Venue: ").SemiBold();
                    text.Span(venueText);
                });

                if (!string.IsNullOrWhiteSpace(roomText))
                {
                    column.Item().Text(text =>
                    {
                        text.Span("Room: ").SemiBold();
                        text.Span(roomText);
                    });
                }

                column.Item().Text(text =>
                {
                    text.Span("Seat Number: ").SemiBold();
                    text.Span(seatText);
                });

                column.Item().PaddingTop(8).Text(text =>
                {
                    text.Span("Pass Marks: ").SemiBold();
                    text.Span($"{exam.PassMarks} / {exam.TotalMarks}");
                });
            });
        }
    }
}
