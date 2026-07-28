using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Documents.Shared
{
    /// <summary>
    /// EP-18 F3: full Document.Create/Page shape shared by every free-text "letter" document
    /// (Offer, Appointment, Joining Booklet, Medical Referral, Target, Office Note) - a
    /// placeholder-substituted body rendered as plain text with line breaks (no rich-text/HTML
    /// support in QuestPDF), framed in DocumentHeaderComponent/DocumentFooterComponent/
    /// DocumentFrame, with a signature block and QR code after the body. Lifted out of
    /// QuestPdfOfferLetterGenerator (EP-18 F2), which was the first and only implementation of
    /// this shape before it multiplied across 5 more generators in F3.
    /// </summary>
    internal static class BrandedLetterPdfComposer
    {
        public static byte[] Compose(
            CompanyBranding branding,
            string documentTitle,
            string candidateName,
            string bodyText,
            string referenceNumber,
            DateTime issueDate,
            byte[]? logoBytes,
            string signeeName,
            string signeeTitle)
        {
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
                        .FontSize(10.5f)
                        .FontFamily(string.IsNullOrWhiteSpace(branding.FontFamily) ? "Helvetica" : branding.FontFamily)
                        .FontColor(DocumentFrame.ResolveColor(branding.SecondaryColor, Colors.Grey.Darken3)));

                    page.Header().Component(new DocumentHeaderComponent(branding, documentTitle, referenceNumber, issueDate, logoBytes));
                    page.Content().PaddingTop(10).Element(content => DocumentFrame.ComposeWatermarked(content, branding,
                        inner => ComposeFramedContent(inner, branding, documentTitle, referenceNumber, candidateName, bodyText, signeeName, signeeTitle)));
                    page.Footer().Element(footer => footer.Component(new DocumentFooterComponent(branding)));
                });
            });

            return document.GeneratePdf();
        }

        private static void ComposeFramedContent(IContainer container, CompanyBranding branding, string documentTitle, string referenceNumber, string candidateName, string bodyText, string signeeName, string signeeTitle)
        {
            var primaryColor = DocumentFrame.ResolveColor(branding.PrimaryColor, Colors.Brown.Darken2);

            DocumentFrame.ApplyBorder(container, branding).Padding(18).Column(column =>
            {
                column.Spacing(8);

                column.Item().Text(documentTitle.ToUpperInvariant()).FontSize(15).Bold().FontColor(primaryColor);
                column.Item().LineHorizontal(1).LineColor(primaryColor);
                column.Item().PaddingTop(4).Text($"To: {candidateName}").FontSize(10).SemiBold();
                column.Item().Text("Dear Sir/Madam,").FontSize(10);

                var lines = bodyText.Replace("\r\n", "\n").Split('\n');
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        column.Item().Height(5);
                    }
                    else
                    {
                        column.Item().Text(line).LineHeight(1.35f);
                    }
                }

                column.Item().PaddingTop(16).Row(row =>
                {
                    row.RelativeItem().Component(new SignatureBlockComponent(branding, signeeName, signeeTitle));
                    row.ConstantItem(70).Component(new QrCodeComponent(branding, referenceNumber));
                });

                column.Item().PaddingTop(8).Text("This is a system-generated document and does not require a physical signature.")
                    .FontSize(8).Italic().FontColor(Colors.Grey.Medium);
            });
        }
    }
}
