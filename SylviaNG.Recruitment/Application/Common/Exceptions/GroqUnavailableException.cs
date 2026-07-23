namespace SylviaNG.Recruitment.Application.Common.Exceptions
{
    /// <summary>
    /// Raised when Groq cannot be reached or rejects a request at the connection/config level
    /// (network failure, timeout, missing API key, exhausted retry on 429) - as opposed to a
    /// single candidate's scoring call failing, which is caught and recorded per-result instead
    /// of thrown (see AiShortlistScoringService).
    /// </summary>
    public class GroqUnavailableException : Exception
    {
        public GroqUnavailableException(string message, Exception? inner = null) : base(message, inner)
        {
        }
    }
}
