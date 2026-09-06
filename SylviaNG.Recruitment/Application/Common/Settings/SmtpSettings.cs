namespace SylviaNG.Recruitment.Application.Common.Settings
{
    public class SmtpSettings
    {
        public const string SectionName = "Smtp";

        public bool IsEnabled { get; set; } = false;

        public string? Host { get; set; }

        public int Port { get; set; } = 587;

        public bool UseStartTls { get; set; } = true;

        public string? Username { get; set; }

        /// <summary>
        /// Never commit a real value in appsettings.json — supply via
        /// appsettings.Development.json (gitignored) or user-secrets, same convention
        /// as Keycloak:ClientSecret.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Per-operation SMTP timeout. MailKit's own default is 120 s, which meant a blocked or
        /// throttled port 587 hung a send for two minutes - multiplied by EmailRetrySender's 3
        /// attempts and by every recipient in an HR fan-out.
        /// </summary>
        public int TimeoutSeconds { get; set; } = 15;

        public string FromEmail { get; set; } = "no-reply@sylviang.local";

        public string FromName { get; set; } = "SylviaNG Recruitment";
    }
}
