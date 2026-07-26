namespace SylviaNG.Recruitment.Application.Common.Settings
{
    /// <summary>EP-09 Feature 2: candidate login OTP gate, layered on top of - not replacing -
    /// Keycloak's RequireEmailVerification link flow. Off by default until fully configured.</summary>
    public class OtpSettings
    {
        public const string SectionName = "CandidateLoginOtp";

        public bool Enabled { get; set; } = false;
        public int ExpiryMinutes { get; set; } = 10;
        public int MaxAttempts { get; set; } = 5;

        /// <summary>
        /// Secret key mixed into the OTP hash. Do NOT commit a real value here - set it locally
        /// via appsettings.Development.json (gitignored) or 'dotnet user-secrets set
        /// "CandidateLoginOtp:Pepper" "<value>"', same convention as Jwt:Local:SigningKey.
        /// </summary>
        public string? Pepper { get; set; }
    }
}
