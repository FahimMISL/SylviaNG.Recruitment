namespace SylviaNG.Recruitment.Application.Common.Settings
{
    public class SslCommerzSettings
    {
        public const string SectionName = "SslCommerz";

        public string StoreId { get; set; } = string.Empty;

        /// <summary>
        /// Never commit a real value in appsettings.json — supply via appsettings.Development.json
        /// (gitignored) or user-secrets, same convention as Keycloak:ClientSecret.
        /// </summary>
        public string StorePassword { get; set; } = string.Empty;

        /// <summary>SSLCommerz API host, e.g. https://sandbox.sslcommerz.com for testing.</summary>
        public string ApiBaseUrl { get; set; } = "https://sandbox.sslcommerz.com";

        public bool IsSandbox { get; set; } = true;

        /// <summary>
        /// Base URL of THIS backend API, used to build the success/fail/cancel/ipn URLs handed to
        /// SSLCommerz at session-init time (SSLCommerz calls these directly, so they must be
        /// publicly reachable, not the frontend's).
        /// </summary>
        public string BackendBaseUrl { get; set; } = "http://localhost:5208";

        /// <summary>
        /// Base URL of the Angular frontend. Used only inside PaymentController's callback
        /// actions to build the final browser-redirect target (/careers/payment-result) after
        /// SSLCommerz posts back to the backend's success/fail/cancel URLs above.
        /// </summary>
        public string FrontendReturnBaseUrl { get; set; } = "http://localhost:4600";
    }
}
