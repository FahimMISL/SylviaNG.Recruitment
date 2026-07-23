namespace SylviaNG.Recruitment.Application.Common.Settings
{
    /// <summary>
    /// Candidate-portal (frontend) base URL, referenced when a backend-generated message needs
    /// to point candidates somewhere in the SPA - e.g. the admit-card-distribution SMS's
    /// download link (US-057 AC2). Same IOptions-bound-settings shape as SslCommerzSettings'
    /// FrontendReturnBaseUrl.
    /// </summary>
    public class PortalSettings
    {
        public const string SectionName = "Portal";

        public string FrontendBaseUrl { get; set; } = "http://localhost:4600";
    }
}
