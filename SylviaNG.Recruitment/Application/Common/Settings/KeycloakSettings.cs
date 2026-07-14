namespace SylviaNG.Recruitment.Application.Common.Settings
{
    public class KeycloakSettings
    {
        public const string SectionName = "Keycloak";

        /// <summary>Realm authority, e.g. http://host:8082/realms/sylviang — also the token issuer.</summary>
        public string Authority { get; set; } = string.Empty;

        public string ClientId { get; set; } = string.Empty;

        /// <summary>
        /// Confidential-client secret. Never commit a real value in appsettings.json —
        /// supply via appsettings.Development.json (gitignored) or user-secrets, same
        /// convention as Jwt:Local:SigningKey.
        /// </summary>
        public string? ClientSecret { get; set; }

        public bool RequireHttpsMetadata { get; set; }

        /// <summary>
        /// When true (production default), self-registered users get Keycloak's
        /// VERIFY_EMAIL required action and cannot log in until verified (US-001 AC2).
        /// Set false in local dev where no SMTP server is configured.
        /// </summary>
        public bool RequireEmailVerification { get; set; } = true;

        /// <summary>Realm name, derived from the trailing segment of Authority.</summary>
        public string Realm => Authority.TrimEnd('/').Split('/').Last();

        /// <summary>Keycloak base URL (Authority minus the /realms/{realm} suffix), for Admin REST API calls.</summary>
        public string BaseUrl
        {
            get
            {
                var trimmed = Authority.TrimEnd('/');
                var idx = trimmed.LastIndexOf("/realms/", StringComparison.OrdinalIgnoreCase);
                return idx > 0 ? trimmed[..idx] : trimmed;
            }
        }
    }
}
