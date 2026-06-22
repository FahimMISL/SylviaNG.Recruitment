using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Infrastructure.Services;

public class SslCommerzService : ISslCommerzService
{
    private readonly SslCommerzSettings _settings;
    private readonly HttpClient _httpClient;
    private readonly ILogger<SslCommerzService> _logger;

    public SslCommerzService(IOptions<SslCommerzSettings> settings, IHttpClientFactory httpClientFactory, ILogger<SslCommerzService> logger)
    {
        _settings = settings.Value;
        _httpClient = httpClientFactory.CreateClient("SslCommerz");
        _logger = logger;
    }

    public async Task<(bool Success, string? GatewayUrl, string? SessionKey)> InitiatePaymentAsync(
        string transactionId, decimal amount, string currency,
        string customerName, string customerEmail, string customerPhone,
        string productName)
    {
        var formData = new Dictionary<string, string>
        {
            ["store_id"] = _settings.StoreId,
            ["store_passwd"] = _settings.StorePassword,
            ["total_amount"] = amount.ToString("F2"),
            ["currency"] = currency,
            ["tran_id"] = transactionId,
            ["success_url"] = _settings.SuccessUrl,
            ["fail_url"] = _settings.FailUrl,
            ["cancel_url"] = _settings.CancelUrl,
            ["ipn_url"] = _settings.IpnUrl,
            ["cus_name"] = customerName,
            ["cus_email"] = customerEmail,
            ["cus_phone"] = customerPhone,
            ["cus_add1"] = "N/A",
            ["cus_city"] = "Dhaka",
            ["cus_country"] = "Bangladesh",
            ["shipping_method"] = "NO",
            ["product_name"] = productName,
            ["product_category"] = "Application Fee",
            ["product_profile"] = "non-physical-goods",
        };

        try
        {
            var response = await _httpClient.PostAsync(
                $"{_settings.BaseUrl}/gwprocess/v4/api.php",
                new FormUrlEncodedContent(formData));

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var status = root.GetProperty("status").GetString();
            if (status == "SUCCESS")
            {
                var gatewayUrl = root.GetProperty("GatewayPageURL").GetString();
                var sessionKey = root.GetProperty("sessionkey").GetString();
                return (true, gatewayUrl, sessionKey);
            }

            var failReason = root.TryGetProperty("failedreason", out var fr) ? fr.GetString() : "Unknown error";
            _logger.LogWarning("SSLCommerz initiation failed: {Reason}", failReason);
            return (false, null, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SSLCommerz initiation error for transaction {TransactionId}", transactionId);
            return (false, null, null);
        }
    }

    public async Task<(bool Valid, string Status)> ValidatePaymentAsync(string validationId)
    {
        try
        {
            var url = $"{_settings.BaseUrl}/validator/api/validationserverAPI.php" +
                      $"?val_id={validationId}&store_id={_settings.StoreId}&store_passwd={_settings.StorePassword}&format=json";

            var response = await _httpClient.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var status = root.GetProperty("status").GetString() ?? "";
            return (status == "VALID" || status == "VALIDATED", status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SSLCommerz validation error for {ValidationId}", validationId);
            return (false, "ERROR");
        }
    }

    public bool VerifyIpnHash(Dictionary<string, string> ipnData)
    {
        if (!ipnData.TryGetValue("verify_sign", out var receivedSign))
            return false;
        if (!ipnData.TryGetValue("verify_key", out var verifyKey))
            return false;

        var keys = verifyKey.Split(',');
        var sorted = keys.OrderBy(k => k).ToList();

        var sb = new StringBuilder();
        foreach (var key in sorted)
        {
            if (ipnData.TryGetValue(key, out var value))
            {
                if (sb.Length > 0) sb.Append('&');
                sb.Append(key).Append('=').Append(value);
            }
        }
        sb.Append('&').Append("store_passwd=").Append(
            Convert.ToHexStringLower(MD5.HashData(Encoding.UTF8.GetBytes(_settings.StorePassword))));

        var computedSign = Convert.ToHexStringLower(MD5.HashData(Encoding.UTF8.GetBytes(sb.ToString())));
        return string.Equals(computedSign, receivedSign, StringComparison.OrdinalIgnoreCase);
    }
}
