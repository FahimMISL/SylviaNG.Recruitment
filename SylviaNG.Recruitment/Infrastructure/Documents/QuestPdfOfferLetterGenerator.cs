using Microsoft.AspNetCore.Hosting;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Infrastructure.Documents.Shared;

namespace SylviaNG.Recruitment.Infrastructure.Documents
{
    /// <summary>
    /// EP-18 F2: branded offer letter, the first implementation of the "letter" document shape
    /// (see BrandedLetterPdfComposer, extracted in F3 once 5 more generators needed the same
    /// shape). The rendered body stays plain text with line breaks, same as before EP-10 US-081.
    /// </summary>
    public class QuestPdfOfferLetterGenerator : IOfferLetterPdfGeneratorService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IBrandingResolverService _brandingResolverService;

        public QuestPdfOfferLetterGenerator(IWebHostEnvironment environment, IBrandingResolverService brandingResolverService)
        {
            _environment = environment;
            _brandingResolverService = brandingResolverService;
        }

        public async Task<byte[]> Generate(string documentTitle, string candidateName, string renderedBody, long sequenceValue)
        {
            var branding = await _brandingResolverService.GetActiveBrandingAsync();
            var logoBytes = RelativeFileLoader.TryLoad(_environment, branding.LogoFilePath);
            var issueDate = DateTime.UtcNow;
            var referenceNumber = ReferenceNumberComponent.BuildReferenceNumber(branding, "OFFER", issueDate.Year, sequenceValue);

            return BrandedLetterPdfComposer.Compose(
                branding, documentTitle, candidateName, renderedBody, referenceNumber, issueDate, logoBytes,
                signeeName: "HR Manager", signeeTitle: "Authorized Signatory");
        }
    }
}
