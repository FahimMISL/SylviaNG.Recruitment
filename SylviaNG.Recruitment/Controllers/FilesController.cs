using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Controllers
{
    /// <summary>
    /// Shared streaming endpoint for files persisted via IFileStorageService/IApplicationCvStorageService
    /// - required once FileStorage:Provider is "Minio" (object storage isn't reachable via the
    /// static-file middleware/wwwroot the way local-disk files were). Response DTOs across the
    /// file-bearing features put this endpoint's URL (built by FileUrlBuilder) into their path/URL
    /// fields instead of a raw relative path, so the frontend's existing `${Base_URL}${path}`
    /// concatenation keeps working unmodified.
    /// [AllowAnonymous] overrides the global AuthorizeFilter (Program.cs) - matches the old
    /// app.UseStaticFiles() behavior it replaces, which was also unauthenticated. Required anyway:
    /// plain &lt;img src&gt;/&lt;a href&gt; browser-native requests (profile photos, document links)
    /// don't carry the Angular HttpClient interceptor's auth header, so an authorized endpoint
    /// would 401 on every such load.
    /// </summary>
    [ApiController]
    [Route("recruitment/files")]
    [AllowAnonymous]
    public class FilesController : ControllerBase
    {
        private static readonly FileExtensionContentTypeProvider ContentTypeProvider = new();

        // Keys containing this segment are private (candidate-uploaded documents, not public
        // assets like logos/job-posting attachments) - see CandidateDocumentService, which mints
        // the token/expires pair on every authenticated "list my documents" call. Actual stored
        // keys look like "uploads/job-postings/candidate-documents/{profileId}/{guid}.ext" (the
        // configured FileStorage root prefixed onto the subFolder CandidateDocumentService.
        // UploadAsync passes), so this must be a Contains check, not StartsWith.
        private const string PrivateKeySegment = "/candidate-documents/";

        // Security review 2026-08-16: OfferLetter/AppointmentLetter/MedicalLetter/TargetLetter/
        // JoiningBooklet/OfficeNote all save under a "documents/..." subFolder (see each service's
        // PdfStorageSubFolder constant) and, before this fix, were served through this endpoint
        // exactly like a public job-posting attachment - fully unauthenticated. The stored key
        // itself is an unguessable GUID (LocalFileStorageService.SaveAsync), and every API path
        // that reveals one is now company-scoped (see the ICompanyScoped fixes on those entities
        // the same day), so this is defense in depth rather than the only gate - but a leaked
        // link (browser history, referrer, a shared screenshot) must not be enough on its own to
        // read someone else's document. Authenticated (not anonymous) is the bar here, same as
        // the candidate document/offer-letter portal pages that link to these already require a
        // login to reach.
        private const string DocumentsKeySegment = "documents/";

        private readonly IFileStorageService _fileStorageService;
        private readonly IApplicationCvStorageService _applicationCvStorageService;
        private readonly IPrivateFileAccessTokenService _privateFileAccessTokenService;

        public FilesController(
            IFileStorageService fileStorageService,
            IApplicationCvStorageService applicationCvStorageService,
            IPrivateFileAccessTokenService privateFileAccessTokenService)
        {
            _fileStorageService = fileStorageService;
            _applicationCvStorageService = applicationCvStorageService;
            _privateFileAccessTokenService = privateFileAccessTokenService;
        }

        /// <summary>
        /// key is the raw stored object key/relative path (e.g. "job-postings/123/{guid}.pdf" or
        /// "applications/456/{guid}.pdf"). Tries IFileStorageService first (job postings,
        /// candidate docs/photos, generated letters, branding logo - the majority of callers),
        /// falls back to IApplicationCvStorageService (CV uploads) on FileNotFoundException, since
        /// both providers can address different subfolder namespaces and there's no separate
        /// "which service" flag persisted anywhere.
        ///
        /// This endpoint is [AllowAnonymous] out of necessity - plain &lt;img src&gt;/&lt;a href&gt;
        /// browser requests carry no Authorization header - which is fine for public assets but
        /// not for private ones. Keys under PrivateKeyPrefix additionally require a valid,
        /// unexpired token/expires pair (see IPrivateFileAccessTokenService) so a leaked/guessed
        /// key alone isn't enough to read someone else's document.
        /// </summary>
        [HttpGet("download")]
        public async Task<IActionResult> Download([FromQuery] string key, [FromQuery] string? token, [FromQuery] long? expires)
        {
            if (string.IsNullOrWhiteSpace(key))
                return BadRequest("key is required.");

            if (key.Contains(PrivateKeySegment, StringComparison.OrdinalIgnoreCase))
            {
                var expiresAt = expires.HasValue
                    ? DateTimeOffset.FromUnixTimeSeconds(expires.Value)
                    : DateTimeOffset.MinValue;

                if (!_privateFileAccessTokenService.IsValid(key, token, expiresAt))
                    return StatusCode(StatusCodes.Status403Forbidden);
            }
            else if (key.Contains(DocumentsKeySegment, StringComparison.OrdinalIgnoreCase))
            {
                if (User.Identity?.IsAuthenticated != true)
                    return Unauthorized();
            }

            Stream stream;
            try
            {
                stream = await _fileStorageService.OpenReadAsync(key);
            }
            catch (FileNotFoundException)
            {
                try
                {
                    stream = await _applicationCvStorageService.OpenReadAsync(key);
                }
                catch (FileNotFoundException)
                {
                    return NotFound();
                }
            }

            var extension = Path.GetExtension(key);
            var contentType = ContentTypeProvider.TryGetContentType($"file{extension}", out var resolved)
                ? resolved
                : "application/octet-stream";

            // No filename arg - that overload sets Content-Disposition: attachment, which makes
            // browsers refuse to render the response inline (breaks <img src> for profile photos/
            // logos). The old app.UseStaticFiles() it replaces never set that header either.
            return File(stream, contentType);
        }
    }
}
