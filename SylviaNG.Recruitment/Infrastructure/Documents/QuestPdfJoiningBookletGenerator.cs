using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Infrastructure.Documents
{
    /// <summary>
    /// Single-page joining booklet (EP-10 US-084). Same QuestPDF Document.Create / Compose*
    /// shape as QuestPdfAppointmentLetterGenerator - renderedBody is plain text with line breaks,
    /// each line becomes its own paragraph.
    /// </summary>
    public class QuestPdfJoiningBookletGenerator : IJoiningBookletPdfGeneratorService
    {
        public byte[] Generate(string documentTitle, string candidateName, string renderedBody)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(11).FontColor(Colors.Grey.Darken3));

                    page.Header().Element(header => ComposeHeader(header, documentTitle, candidateName));
                    page.Content().PaddingTop(15).Element(content => ComposeContent(content, renderedBody));

                    page.Footer().AlignCenter().Text("This is a system-generated document.")
                        .FontSize(8).FontColor(Colors.Grey.Medium);
                });
            });

            return document.GeneratePdf();
        }

        private static void ComposeHeader(IContainer container, string documentTitle, string candidateName)
        {
            container.Column(column =>
            {
                column.Item().Text(documentTitle).FontSize(18).Bold().FontColor(Colors.Blue.Darken2);
                column.Item().Text($"Prepared for: {candidateName}").FontSize(10).Italic().FontColor(Colors.Grey.Medium);
            });
        }

        private static void ComposeContent(IContainer container, string renderedBody)
        {
            container.Column(column =>
            {
                column.Spacing(6);

                var lines = renderedBody.Replace("\r\n", "\n").Split('\n');
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        column.Item().Height(6);
                    }
                    else
                    {
                        column.Item().Text(line);
                    }
                }
            });
        }
    }
}
