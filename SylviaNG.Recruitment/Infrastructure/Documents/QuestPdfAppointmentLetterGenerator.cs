using Microsoft.AspNetCore.Hosting;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Infrastructure.Documents.Shared;

namespace SylviaNG.Recruitment.Infrastructure.Documents
{
    /// <summary>
    /// EP-18 F3: branded appointment letter (EP-10 US-083), rolled onto BrandedLetterPdfComposer -
    /// same "letter" shape as QuestPdfOfferLetterGenerator.
    /// </summary>
    public class QuestPdfAppointmentLetterGenerator : IAppointmentLetterPdfGeneratorService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IBrandingResolverService _brandingResolverService;

        public QuestPdfAppointmentLetterGenerator(IWebHostEnvironment environment, IBrandingResolverService brandingResolverService)
        {
            _environment = environment;
            _brandingResolverService = brandingResolverService;
        }

        public async Task<byte[]> Generate(string documentTitle, string candidateName, string finalBody, long sequenceValue)
        {
            var branding = await _brandingResolverService.GetActiveBrandingAsync();
            var logoBytes = RelativeFileLoader.TryLoad(_environment, branding.LogoFilePath);
            var issueDate = DateTime.UtcNow;
            var referenceNumber = ReferenceNumberComponent.BuildReferenceNumber(branding, "APPOINTMENT", issueDate.Year, sequenceValue);

            return BrandedLetterPdfComposer.Compose(
                branding, documentTitle, candidateName, finalBody, referenceNumber, issueDate, logoBytes,
                signeeName: "Head of HR", signeeTitle: "Authorized Signatory");
        }
    }
}
