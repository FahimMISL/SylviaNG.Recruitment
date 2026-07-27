namespace SylviaNG.Recruitment.Application.Interfaces.Externals
{
    public record SslCommerzSessionRequest(
        string TransactionId,
        decimal Amount,
        string Currency,
        string CustomerName,
        string? CustomerEmail,
        string? CustomerPhone,
        string ProductName);

    public record SslCommerzSessionResult(bool Success, string? GatewayPageUrl, string? SessionKey, string? FailureReason);

    public record SslCommerzValidationResult(
        bool IsValid,
        string? Status,
        decimal? Amount,
        string? Currency,
        string? TransactionId);

    /// <summary>
    /// Thin HTTP client over SSLCommerz's Session API (payment initiation) and Validation API
    /// (server-side confirmation). All gateway traffic stays server-side so the store
    /// credentials never reach the browser.
    /// </summary>
    public interface ISslCommerzPaymentGateway
    {
        /// <summary>
        /// Opens a checkout session for one payment attempt. Throws SslCommerzUnavailableException
        /// when the gateway cannot be reached; a normal "your request was invalid" response comes
        /// back as Success=false rather than an exception.
        /// </summary>
        Task<SslCommerzSessionResult> InitiateSessionAsync(SslCommerzSessionRequest request);

        /// <summary>
        /// Re-fetches a transaction's authoritative status from SSLCommerz using the val_id
        /// SSLCommerz itself issued at IPN time. This is the ONLY trustworthy source of "did this
        /// payment actually succeed" — the raw IPN POST body must never be trusted on its own.
        /// </summary>
        Task<SslCommerzValidationResult> ValidateTransactionAsync(string validationId);
    }
}
