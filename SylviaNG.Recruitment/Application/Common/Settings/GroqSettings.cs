namespace SylviaNG.Recruitment.Application.Common.Settings
{
    public class GroqSettings
    {
        public const string SectionName = "Groq";

        /// <summary>
        /// Never commit a real value in appsettings.json — supply via
        /// appsettings.Development.json (gitignored) or user-secrets, same convention
        /// as Keycloak:ClientSecret.
        /// </summary>
        public string? ApiKey { get; set; }

        public string BaseUrl { get; set; } = "https://api.groq.com/openai/v1";

        public string Model { get; set; } = "llama-3.1-8b-instant";

        public double Temperature { get; set; } = 0.2;

        public int TimeoutSeconds { get; set; } = 20;

        /// <summary>Bounded concurrency for scoring many candidates in one AutoShortlistRun (US-046).</summary>
        public int MaxConcurrentRequests { get; set; } = 5;
    }
}
