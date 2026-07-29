namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>Thin wrapper over Groq's OpenAI-compatible Chat Completions API (US-046).</summary>
    public interface IGroqClient
    {
        /// <summary>
        /// Sends a chat completion request with JSON-object response format and returns the raw
        /// JSON string from choices[0].message.content. Parsing into a specific shape is the
        /// caller's job - this stays a thin, reusable HTTP wrapper.
        /// </summary>
        Task<string> GetJsonCompletionAsync(string systemPrompt, string userPrompt, CancellationToken ct = default);
    }
}
