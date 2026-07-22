using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SylviaNG.Recruitment.Infrastructure.Services
{
    /// <summary>
    /// Thin HTTP client over Groq's OpenAI-compatible Chat Completions API (US-046, the "Ai"
    /// shortlist-scoring provider). One retry on HTTP 429 before giving up.
    /// </summary>
    public class GroqClient : IGroqClient
    {
        private readonly HttpClient _httpClient;
        private readonly GroqSettings _settings;
        private readonly ILogger<GroqClient> _logger;

        public GroqClient(HttpClient httpClient, IOptions<GroqSettings> settings, ILogger<GroqClient> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;
        }

        private string ChatCompletionsEndpoint => $"{_settings.BaseUrl.TrimEnd('/')}/chat/completions";

        public async Task<string> GetJsonCompletionAsync(string systemPrompt, string userPrompt, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(_settings.ApiKey))
                throw new GroqUnavailableException("Groq API key is not configured.");

            var payload = new
            {
                model = _settings.Model,
                messages = new object[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt }
                },
                temperature = _settings.Temperature,
                response_format = new { type = "json_object" }
            };

            var response = await SendWithRetryAsync(payload, ct);
            var body = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Groq chat completion failed ({Status}): {Body}", (int)response.StatusCode, body);
                throw new GroqUnavailableException($"Groq chat completions returned {(int)response.StatusCode}.");
            }

            using var json = JsonDocument.Parse(body);
            return json.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString()
                ?? throw new GroqUnavailableException("Groq response had no message content.");
        }

        private async Task<HttpResponseMessage> SendWithRetryAsync(object payload, CancellationToken ct)
        {
            var response = await SendAsync(payload, ct);
            if (response.StatusCode != HttpStatusCode.TooManyRequests)
                return response;

            var delay = response.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(2);
            _logger.LogWarning("Groq rate limit hit, retrying once after {Delay}.", delay);
            await Task.Delay(delay, ct);

            return await SendAsync(payload, ct);
        }

        private async Task<HttpResponseMessage> SendAsync(object payload, CancellationToken ct)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, ChatCompletionsEndpoint)
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);

            try
            {
                return await _httpClient.SendAsync(request, ct);
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                throw new GroqUnavailableException("Groq server is unreachable.", ex);
            }
        }
    }
}
