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
    /// EP-18 F3: sample document for the branding-settings admin screen's "Preview PDF" button -
    /// same six-component composition as the original EP-18 F1 smoke test
    /// (DocumentBrandingComponentsSmokeTests), productionized behind a real service/endpoint, with
    /// canned data standing in for a real candidate/application.
    /// </summary>
    public class BrandingPreviewPdfGenerator : IBrandingPreviewPdfGeneratorService
    {
        private readonly IWebHostEnvironment _environment;

        public BrandingPreviewPdfGenerator(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public Task<byte[]> Generate(CompanyBranding branding)
        {
            var logoBytes = RelativeFileLoader.TryLoad(_environment, branding.LogoFilePath);
            var issueDate = DateTime.UtcNow;
            var referenceNumber = ReferenceNumberComponent.BuildReferenceNumber(branding, "SAMPLE", issueDate.Year, 1);

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

                    page.Header().Component(new DocumentHeaderComponent(branding, "Sample Document", referenceNumber, issueDate, logoBytes));
                    page.Content().PaddingTop(15).Element(content => DocumentFrame.ComposeWatermarked(content, branding,
                        inner => ComposeFramedContent(inner, branding, referenceNumber)));
                    page.Footer().Element(footer => footer.Component(new DocumentFooterComponent(branding)));
                });
            });

            return Task.FromResult(document.GeneratePdf());
        }

        private static void ComposeFramedContent(IContainer container, CompanyBranding branding, string referenceNumber)
        {
            DocumentFrame.ApplyBorder(container, branding).Padding(10).Column(column =>
            {
                column.Spacing(10);

                column.Item().Component(new InfoSectionComponent(branding, "Sample Details", new List<(string, string)>
                {
                    ("Candidate Name", "Jane Doe"),
                    ("Application ID", "1001"),
                    ("Reference Number", referenceNumber)
                }));

                column.Item().Row(row =>
                {
                    row.RelativeItem().Component(new SignatureBlockComponent(branding, "HR Manager", "Authorized Signatory"));
                    row.ConstantItem(70).Component(new QrCodeComponent(branding, referenceNumber));
                });
            });
        }
    }
}
