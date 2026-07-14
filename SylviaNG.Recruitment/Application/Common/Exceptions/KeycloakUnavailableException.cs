namespace SylviaNG.Recruitment.Application.Common.Exceptions
{
    /// <summary>
    /// Raised when the Keycloak server cannot be reached (network failure / server down),
    /// as opposed to Keycloak rejecting the request. Lets callers distinguish "auth server
    /// offline" (fallback / 503) from "bad credentials" (401).
    /// </summary>
    public class KeycloakUnavailableException : Exception
    {
        public KeycloakUnavailableException(string message, Exception? inner = null) : base(message, inner)
        {
        }
    }
}
