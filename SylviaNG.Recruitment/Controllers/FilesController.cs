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

        private readonly IFileStorageService _fileStorageService;
        private readonly IApplicationCvStorageService _applicationCvStorageService;

        public FilesController(IFileStorageService fileStorageService, IApplicationCvStorageService applicationCvStorageService)
        {
            _fileStorageService = fileStorageService;
            _applicationCvStorageService = applicationCvStorageService;
        }

        /// <summary>
        /// key is the raw stored object key/relative path (e.g. "job-postings/123/{guid}.pdf" or
        /// "applications/456/{guid}.pdf"). Tries IFileStorageService first (job postings,
        /// candidate docs/photos, generated letters, branding logo - the majority of callers),
        /// falls back to IApplicationCvStorageService (CV uploads) on FileNotFoundException, since
        /// both providers can address different subfolder namespaces and there's no separate
        /// "which service" flag persisted anywhere.
        /// </summary>
        [HttpGet("download")]
        public async Task<IActionResult> Download([FromQuery] string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return BadRequest("key is required.");

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
