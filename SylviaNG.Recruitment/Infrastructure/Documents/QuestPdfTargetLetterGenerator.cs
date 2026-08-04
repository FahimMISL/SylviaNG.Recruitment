using Microsoft.AspNetCore.Hosting;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Infrastructure.Documents.Shared;

namespace SylviaNG.Recruitment.Infrastructure.Documents
{
    /// <summary>
    /// EP-18 F3: branded target letter (EP-10 US-086), rolled onto BrandedLetterPdfComposer -
    /// same "letter" shape as QuestPdfOfferLetterGenerator.
    /// </summary>
    public class QuestPdfTargetLetterGenerator : ITargetLetterPdfGeneratorService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IBrandingResolverService _brandingResolverService;

        public QuestPdfTargetLetterGenerator(IWebHostEnvironment environment, IBrandingResolverService brandingResolverService)
        {
            _environment = environment;
            _brandingResolverService = brandingResolverService;
        }

        public async Task<byte[]> Generate(string documentTitle, string candidateName, string finalBody, long sequenceValue)
        {
            var branding = await _brandingResolverService.GetActiveBrandingAsync();
            var logoBytes = RelativeFileLoader.TryLoad(_environment, branding.LogoFilePath);
            var issueDate = DateTime.UtcNow;
            var referenceNumber = ReferenceNumberComponent.BuildReferenceNumber(branding, "TARGET", issueDate.Year, sequenceValue);

            return BrandedLetterPdfComposer.Compose(
                branding, documentTitle, candidateName, finalBody, referenceNumber, issueDate, logoBytes,
                signeeName: "HR Manager", signeeTitle: "Authorized Signatory");
        }
    }
}
