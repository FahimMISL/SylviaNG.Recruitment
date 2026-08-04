using FluentAssertions;
using Moq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.Infrastructure.Documents;
using SylviaNG.Recruitment.Infrastructure.Documents.Shared;

namespace SylviaNG.Recruitment.Tests.Smoke;

/// <summary>
/// EP-18 F1: proof-of-life for the shared QuestPDF component layer - proves all six components
/// actually compose into a real, non-empty PDF without prematurely wiring them into the real
/// Admit Card/Offer Letter generators (that's F2's job, per Doc/features/EP-18-F1-...md).
/// </summary>
public class DocumentBrandingComponentsSmokeTests
{
    static DocumentBrandingComponentsSmokeTests()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    private static CompanyBranding BuildTestBranding() => new()
    {
        CompanyBrandingId = 1,
        TenantId = "default_tenant",
        CompanyName = "Millennium Information Solution Ltd.",
        AddressLine = "Administrative Building-01, Level-18, Mirpur-2, Dhaka-1216",
        Phone = "09601 789 789",
        Email = "info@mislbd.com",
        Website = "https://mislbd.com",
        PrimaryColor = "#7A2E2E",
        HeaderLayout = HeaderLayoutEnum.LogoLeftTextRight,
        FooterLayout = FooterLayoutEnum.ThreeColumn,
        HeaderDividerStyle = DocumentDividerStyleEnum.SolidLine,
        FooterDividerStyle = DocumentDividerStyleEnum.SolidLine,
        QrCodePosition = DocumentElementPositionEnum.TopRight,
        SignaturePosition = DocumentElementPositionEnum.BottomLeft,
        SealPosition = DocumentElementPositionEnum.BottomRight,
        DocumentReferenceFormat = "{ORG}/{DOCTYPE}/{YEAR}/{SEQ}",
        ShowPageNumbers = true
    };

    [Fact]
    public void AllSixSharedComponents_ComposedIntoOneDocument_ShouldRenderNonEmptyPdf()
    {
        var branding = BuildTestBranding();
        var referenceNumber = ReferenceNumberComponent.BuildReferenceNumber(branding, "ADMIT", 2026, 42);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);

                page.Header().Component(new DocumentHeaderComponent(branding, "Test Document", referenceNumber, DateTime.UtcNow));

                page.Content().Column(column =>
                {
                    column.Item().Component(new InfoSectionComponent(branding, "Details", new List<(string, string)>
                    {
                        ("Candidate", "Test Candidate"),
                        ("Reference", referenceNumber)
                    }));

                    column.Item().Component(new QrCodeComponent(branding, referenceNumber));
                    column.Item().Component(new SignatureBlockComponent(branding, "Jane Doe", "Head of HR"));
                });

                page.Footer().Component(new DocumentFooterComponent(branding));
            });
        });

        var pdfBytes = document.GeneratePdf();

        pdfBytes.Should().NotBeNull();
        pdfBytes.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetActiveBrandingAsync_WhenNoRowSeededForTenant_ShouldReturnNonNullFallback()
    {
        var repositoryMock = new Mock<ICompanyBrandingRepository>();
        repositoryMock
            .Setup(r => r.GetByTenantIdAsync(It.IsAny<string>()))
            .ReturnsAsync((CompanyBranding?)null);

        var resolver = new BrandingResolverService(repositoryMock.Object);

        var branding = await resolver.GetActiveBrandingAsync();

        branding.Should().NotBeNull();
        branding.DocumentReferenceFormat.Should().NotBeNullOrWhiteSpace();
    }
}
