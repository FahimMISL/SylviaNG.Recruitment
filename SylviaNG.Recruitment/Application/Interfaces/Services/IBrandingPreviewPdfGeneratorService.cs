using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>
    /// EP-18 F3: renders one sample page using all 6 F1 shared components with canned data, so
    /// the branding-settings admin screen's "Preview PDF" button can show what a real document
    /// would look like - including for a not-yet-saved draft, since it takes CompanyBranding
    /// directly rather than resolving it internally.
    /// </summary>
    public interface IBrandingPreviewPdfGeneratorService
    {
        Task<byte[]> Generate(CompanyBranding branding);
    }
}
