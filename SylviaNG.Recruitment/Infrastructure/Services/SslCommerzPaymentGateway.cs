using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Interfaces.Externals;
using System.Text.Json;

namespace SylviaNG.Recruitment.Infrastructure.Services
{
    /// <summary>
    /// Thin HTTP client over SSLCommerz's Session API (gwprocess/v4/api.php) and Validation API
    /// (validator/api/validationserverAPI.php). Mirrors KeycloakClient's manual try/catch ->
    /// custom-exception pattern; no retry/circuit-breaker (none exists elsewhere in this repo).
    /// </summary>
    public class SslCommerzPaymentGateway : ISslCommerzPaymentGateway
    {
        private readonly HttpClient _httpClient;
        private readonly SslCommerzSettings _settings;
        private readonly ILogger<SslCommerzPaymentGateway> _logger;

        public SslCommerzPaymentGateway(HttpClient httpClient, IOptions<SslCommerzSettings> settings, ILogger<SslCommerzPaymentGateway> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;
        }

        private string SessionEndpoint => $"{_settings.ApiBaseUrl.TrimEnd('/')}/gwprocess/v4/api.php";
        private string ValidationEndpoint => $"{_settings.ApiBaseUrl.TrimEnd('/')}/validator/api/validationserverAPI.php";

        public async Task<SslCommerzSessionResult> InitiateSessionAsync(SslCommerzSessionRequest request)
        {
            var backendBase = _settings.BackendBaseUrl.TrimEnd('/');
            var form = new Dictionary<string, string>
            {
                ["store_id"] = _settings.StoreId,
                ["store_passwd"] = _settings.StorePassword,
                ["total_amount"] = request.Amount.ToString("F2"),
                ["currency"] = request.Currency,
                ["tran_id"] = request.TransactionId,
                ["success_url"] = $"{backendBase}/recruitment/payment/callback/success",
                ["fail_url"] = $"{backendBase}/recruitment/payment/callback/fail",
                ["cancel_url"] = $"{backendBase}/recruitment/payment/callback/cancel",
                ["ipn_url"] = $"{backendBase}/recruitment/payment/ipn",
                ["cus_name"] = request.CustomerName,
                ["cus_email"] = string.IsNullOrWhiteSpace(request.CustomerEmail) ? "no-reply@sylviang.local" : request.CustomerEmail,
                ["cus_add1"] = "N/A",
                ["cus_phone"] = string.IsNullOrWhiteSpace(request.CustomerPhone) ? "N/A" : request.CustomerPhone,
                ["shipping_method"] = "NO",
                ["product_name"] = request.ProductName,
                ["product_category"] = "Service",
                ["product_profile"] = "general"
            };

            string body;
            try
            {
                var response = await _httpClient.PostAsync(SessionEndpoint, new FormUrlEncodedContent(form));
                body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("SSLCommerz session-init returned {Status}: {Body}", (int)response.StatusCode, body);
                    throw new SslCommerzUnavailableException($"SSLCommerz session-init returned {(int)response.StatusCode}.");
                }
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                throw new SslCommerzUnavailableException("SSLCommerz gateway is unreachable.", ex);
            }

            using var json = JsonDocument.Parse(body);
            var root = json.RootElement;
            var status = root.TryGetProperty("status", out var statusEl) ? statusEl.GetString() : null;

            if (!string.Equals(status, "SUCCESS", StringComparison.OrdinalIgnoreCase))
            {
                var failedReason = root.TryGetProperty("failedreason", out var reasonEl) ? reasonEl.GetString() : status;
                _logger.LogWarning("SSLCommerz session-init rejected the request: {Reason}", failedReason);
                return new SslCommerzSessionResult(false, null, null, failedReason);
            }

            var gatewayPageUrl = root.TryGetProperty("GatewayPageURL", out var urlEl) ? urlEl.GetString() : null;
            var sessionKey = root.TryGetProperty("sessionkey", out var keyEl) ? keyEl.GetString() : null;

            return new SslCommerzSessionResult(true, gatewayPageUrl, sessionKey, null);
        }

        public async Task<SslCommerzValidationResult> ValidateTransactionAsync(string validationId)
        {
            var url = $"{ValidationEndpoint}?val_id={Uri.EscapeDataString(validationId)}" +
                      $"&store_id={Uri.EscapeDataString(_settings.StoreId)}" +
                      $"&store_passwd={Uri.EscapeDataString(_settings.StorePassword)}" +
                      "&format=json";

            string body;
            try
            {
                var response = await _httpClient.GetAsync(url);
                body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("SSLCommerz validation API returned {Status}: {Body}", (int)response.StatusCode, body);
                    throw new SslCommerzUnavailableException($"SSLCommerz validation API returned {(int)response.StatusCode}.");
                }
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                throw new SslCommerzUnavailableException("SSLCommerz gateway is unreachable.", ex);
            }

            using var json = JsonDocument.Parse(body);
            var root = json.RootElement;
            var status = root.TryGetProperty("status", out var statusEl) ? statusEl.GetString() : null;
            var isValid = status is "VALID" or "VALIDATED";

            var amount = root.TryGetProperty("amount", out var amountEl) && decimal.TryParse(amountEl.GetString(), out var parsedAmount)
                ? parsedAmount
                : (decimal?)null;
            var currency = root.TryGetProperty("currency", out var currencyEl) ? currencyEl.GetString() : null;
            var tranId = root.TryGetProperty("tran_id", out var tranIdEl) ? tranIdEl.GetString() : null;

            return new SslCommerzValidationResult(isValid, status, amount, currency, tranId);
        }
    }
}
