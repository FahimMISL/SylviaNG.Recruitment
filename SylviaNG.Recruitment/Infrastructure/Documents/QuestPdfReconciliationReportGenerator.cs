using Microsoft.AspNetCore.Hosting;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SylviaNG.Recruitment.Application.Features.PaymentReports.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Documents.Shared;

namespace SylviaNG.Recruitment.Infrastructure.Documents
{
    /// <summary>EP-17/US-128: branded one-page summary of the paid/failed/waived/net totals for a
    /// reconciliation period - the detailed row-by-row breakdown lives in the Excel export and the
    /// on-screen transaction list, not duplicated here.</summary>
    public class QuestPdfReconciliationReportGenerator : IPaymentReportPdfGeneratorService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IBrandingResolverService _brandingResolverService;

        public QuestPdfReconciliationReportGenerator(IWebHostEnvironment environment, IBrandingResolverService brandingResolverService)
        {
            _environment = environment;
            _brandingResolverService = brandingResolverService;
        }

        public async Task<byte[]> GenerateReconciliationPdfAsync(ReconciliationRequest request, ReconciliationResponse summary)
        {
            var branding = await _brandingResolverService.GetActiveBrandingAsync();
            var logoBytes = RelativeFileLoader.TryLoad(_environment, branding.LogoFilePath);
            var referenceNumber = ReferenceNumberComponent.BuildReferenceNumber(branding, "RECON", summary.DateTo.Year, summary.DateTo.Ticks % 100000);

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

                    page.Header().Component(new DocumentHeaderComponent(branding, "Payment Reconciliation Report", referenceNumber, DateTime.UtcNow, logoBytes));
                    page.Content().PaddingTop(10).Element(content => DocumentFrame.ComposeWatermarked(content, branding,
                        inner => ComposeContent(inner, branding, request, summary)));
                    page.Footer().Element(footer => footer.Component(new DocumentFooterComponent(branding)));
                });
            });

            return document.GeneratePdf();
        }

        private static void ComposeContent(IContainer container, CompanyBranding branding, ReconciliationRequest request, ReconciliationResponse summary)
        {
            var primaryColor = DocumentFrame.ResolveColor(branding.PrimaryColor, Colors.Brown.Darken2);
            var accentColor = DocumentFrame.ResolveColor(branding.AccentColor, Colors.Brown.Lighten4);

            DocumentFrame.ApplyBorder(container, branding).Padding(10).Column(column =>
            {
                column.Spacing(4);

                column.Item().Background(primaryColor).Padding(5).Text("RECONCILIATION SUMMARY").FontSize(11).Bold().FontColor(Colors.White);

                column.Item().PaddingBottom(6).Text(text =>
                {
                    text.Span("Period: ").SemiBold();
                    text.Span($"{summary.DateFrom:dd MMM yyyy} - {summary.DateTo:dd MMM yyyy}");
                });

                if (request.JobPostingId.HasValue || request.DepartmentId.HasValue)
                {
                    column.Item().PaddingBottom(6).Text(text =>
                    {
                        text.Span("Scope: ").SemiBold();
                        var parts = new List<string>();
                        if (request.JobPostingId.HasValue) parts.Add($"Vacancy #{request.JobPostingId}");
                        if (request.DepartmentId.HasValue) parts.Add($"Department #{request.DepartmentId}");
                        text.Span(string.Join(", ", parts));
                    });
                }

                column.Item().PaddingTop(6).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(3);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(cell => HeaderCell(cell, accentColor)).Text("Metric");
                        header.Cell().Element(cell => HeaderCell(cell, accentColor)).Text("Count");
                        header.Cell().Element(cell => HeaderCell(cell, accentColor)).Text("Amount");
                    });

                    var rows = new (string Metric, string Count, string Amount)[]
                    {
                        ("Paid", summary.PaidCount.ToString(), summary.PaidAmount.ToString("N2")),
                        ("Failed", summary.FailedCount.ToString(), "-"),
                        ("Waived", summary.WaivedCount.ToString(), "-"),
                        ("Net", "-", summary.NetAmount.ToString("N2")),
                    };

                    foreach (var row in rows)
                    {
                        table.Cell().Element(BodyCell).Text(row.Metric).SemiBold();
                        table.Cell().Element(BodyCell).Text(row.Count);
                        table.Cell().Element(BodyCell).Text(row.Amount);
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
