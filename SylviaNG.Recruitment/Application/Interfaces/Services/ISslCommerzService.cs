namespace SylviaNG.Recruitment.Application.Interfaces.Services;

public interface ISslCommerzService
{
    Task<(bool Success, string? GatewayUrl, string? SessionKey)> InitiatePaymentAsync(
        string transactionId, decimal amount, string currency,
        string customerName, string customerEmail, string customerPhone,
        string productName);

    Task<(bool Valid, string Status)> ValidatePaymentAsync(string validationId);

    bool VerifyIpnHash(Dictionary<string, string> ipnData);
}
