namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>
    /// Short-lived signed tokens gating private files (candidate documents, at present) served
    /// through FilesController - which is [AllowAnonymous] by necessity (plain &lt;a href&gt;/
    /// &lt;img src&gt; requests carry no Authorization header). A candidate's own authenticated
    /// "list my documents" call mints the token; the download link only works while it's valid.
    /// </summary>
    public interface IPrivateFileAccessTokenService
    {
        /// <summary>Generates a token for <paramref name="key"/> valid until <paramref name="expiresAtUtc"/>.</summary>
        string GenerateToken(string key, DateTimeOffset expiresAtUtc);

        /// <summary>True if <paramref name="token"/> was minted for exactly this <paramref name="key"/>,
        /// hasn't been tampered with, and <paramref name="expiresAtUtc"/> hasn't passed.</summary>
        bool IsValid(string key, string? token, DateTimeOffset expiresAtUtc);
    }
}
