namespace SylviaNG.Recruitment.Application.Common.Settings
{
    /// <summary>
    /// Signing key for short-lived download tokens on private files served through
    /// FilesController (candidate documents, at present) - see PrivateFileAccessTokenService.
    /// </summary>
    public class PrivateFileAccessSettings
    {
        public const string SectionName = "PrivateFileAccess";

        public string SigningKey { get; set; } = string.Empty;
        public int ExpiryMinutes { get; set; } = 15;
    }
}
