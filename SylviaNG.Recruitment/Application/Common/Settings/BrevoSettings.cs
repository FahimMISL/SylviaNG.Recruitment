namespace SylviaNG.Recruitment.Application.Common.Settings
{
    public class BrevoSettings
    {
        public const string SectionName = "Brevo";

        /// <summary>
        /// Never commit a real value in appsettings.json — supply via
        /// appsettings.Development.json (gitignored) or user-secrets, same convention
        /// as Keycloak:ClientSecret.
        /// </summary>
        public string? ApiKey { get; set; }

        /// <summary>Must be an email verified as a Brevo sender (single email-click confirmation, no domain/DNS needed) - sending from an unverified address fails.</summary>
        public string FromEmail { get; set; } = string.Empty;

        public string FromName { get; set; } = "SylviaNG Recruitment";
    }
}
