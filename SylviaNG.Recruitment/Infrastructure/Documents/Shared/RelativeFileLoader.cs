using Microsoft.AspNetCore.Hosting;

namespace SylviaNG.Recruitment.Infrastructure.Documents.Shared
{
    /// <summary>
    /// EP-18 F3: loads a wwwroot-relative file path (candidate photo, branding logo, etc.) as
    /// bytes, or null if missing/unreadable. Lifts the identical try/catch-then-null block that
    /// was duplicated across QuestPdfAdmitCardGenerator/QuestPdfOfferLetterGenerator/
    /// QuestPdfCvGenerator/QuestPdfCandidateProfileGenerator (originally QuestPdfCvGenerator's
    /// TryLoadPhoto convention).
    /// </summary>
    internal static class RelativeFileLoader
    {
        public static byte[]? TryLoad(IWebHostEnvironment environment, string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return null;

            try
            {
                var physicalPath = Path.Combine(environment.ContentRootPath, "wwwroot", relativePath.TrimStart('/'));
                return File.Exists(physicalPath) ? File.ReadAllBytes(physicalPath) : null;
            }
            catch (IOException)
            {
                return null;
            }
        }
    }
}
