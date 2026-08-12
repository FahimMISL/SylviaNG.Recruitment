using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Common.Helpers
{
    /// <summary>
    /// Appends the token/expires pair FilesController requires for private file keys (see its
    /// PrivateKeySegment check) onto a FileUrlBuilder-built download URL. Shared by every place
    /// that hands a candidate document's URL to a caller - CandidateDocumentService (candidate's
    /// own list/upload/update) and CandidateProfileService (HR's candidate-detail view) both
    /// build the same DownloadUrl shape and need the same token minted on it.
    /// </summary>
    public static class PrivateFileUrlSecurer
    {
        public static string? Secure(string? downloadUrl, string? rawKey, IPrivateFileAccessTokenService tokenService, int expiryMinutes)
        {
            if (string.IsNullOrEmpty(downloadUrl) || string.IsNullOrEmpty(rawKey))
                return downloadUrl;

            var expiresAt = DateTimeOffset.UtcNow.AddMinutes(expiryMinutes);
            var token = tokenService.GenerateToken(rawKey, expiresAt);
            return $"{downloadUrl}&token={Uri.EscapeDataString(token)}&expires={expiresAt.ToUnixTimeSeconds()}";
        }
    }
}
