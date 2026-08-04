namespace SylviaNG.Recruitment.Application.Common.Exceptions
{
    /// <summary>
    /// Raised when the SSLCommerz gateway cannot be reached or returns an unexpected/error
    /// response, as opposed to a legitimate declined/cancelled payment. Lets callers distinguish
    /// "gateway offline" (503) from a normal failed-payment outcome.
    /// </summary>
    public class SslCommerzUnavailableException : Exception
    {
        public SslCommerzUnavailableException(string message, Exception? inner = null) : base(message, inner)
        {
        }
    }
}
