using System.Net;

namespace SylviaNG.Recruitment.Application.Common.Helpers
{
    /// <summary>
    /// Builds the recruitment/files/download proxy URL for a raw stored object key/relative path
    /// (as returned by IFileStorageService.SaveAsync / IApplicationCvStorageService.SaveAsync).
    /// Used at every response-DTO boundary that used to hand back the raw wwwroot-relative path
    /// directly - the frontend's `${Base_URL}${path}` concatenation (environment.ts) works
    /// unmodified against this value since it's just a relative path/query string, same as before.
    /// </summary>
    public static class FileUrlBuilder
    {
        /// <summary>Returns null/empty unchanged (so optional fields like LogoFilePath/
        /// ProfilePhotoPath stay null when nothing was uploaded, instead of pointing at a broken
        /// download link).</summary>
        public static string? BuildDownloadUrl(string? relativeFilePathOrKey)
        {
            if (string.IsNullOrWhiteSpace(relativeFilePathOrKey))
                return relativeFilePathOrKey;

            return $"recruitment/files/download?key={WebUtility.UrlEncode(relativeFilePathOrKey)}";
        }
    }
}
