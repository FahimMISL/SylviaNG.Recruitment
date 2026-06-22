using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Controllers;

[Authorize]
[ApiController]
[Route("recruitment/profile-photo")]
public class ProfilePhotoController : ControllerBase
{
    private readonly IUserProfilePhotoService _service;
    private readonly ILogger<ProfilePhotoController> _logger;

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/webp"
    };

    private static readonly Dictionary<string, byte[][]> MagicBytes = new()
    {
        { "image/jpeg", new[] { new byte[] { 0xFF, 0xD8, 0xFF } } },
        { "image/png", new[] { new byte[] { 0x89, 0x50, 0x4E, 0x47 } } },
        { "image/webp", new[] { new byte[] { 0x52, 0x49, 0x46, 0x46 } } },
    };

    public ProfilePhotoController(IUserProfilePhotoService service, ILogger<ProfilePhotoController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return Ok(new { hasError = true, decentMessage = "No file provided." });

        if (file.Length > 2 * 1024 * 1024)
            return Ok(new { hasError = true, decentMessage = "Photo must be under 2MB." });

        if (!AllowedContentTypes.Contains(file.ContentType))
            return Ok(new { hasError = true, decentMessage = "Only JPEG, PNG, and WebP images are allowed." });

        var keycloakUserId = GetKeycloakUserId();
        if (string.IsNullOrEmpty(keycloakUserId))
            return Ok(new { hasError = true, decentMessage = "User identity not found." });

        byte[] fileBytes;
        using (var ms = new MemoryStream())
        {
            await file.CopyToAsync(ms);
            fileBytes = ms.ToArray();
        }

        if (!ValidateMagicBytes(fileBytes, file.ContentType))
            return Ok(new { hasError = true, decentMessage = "File content does not match its type." });

        var sanitizedName = SanitizeFileName(file.FileName);

        var id = await _service.UploadAsync(keycloakUserId, sanitizedName, file.ContentType, fileBytes);

        return Ok(new { userProfilePhotoId = id, fileName = sanitizedName });
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyPhoto()
    {
        var keycloakUserId = GetKeycloakUserId();
        if (string.IsNullOrEmpty(keycloakUserId))
            return NotFound();

        var photo = await _service.GetByKeycloakUserIdAsync(keycloakUserId);
        if (photo == null)
            return Ok((object?)null);

        return Ok(photo);
    }

    [HttpGet("view/{keycloakUserId}")]
    public async Task<IActionResult> ViewPhoto(string keycloakUserId)
    {
        var repo = HttpContext.RequestServices.GetRequiredService<IUserProfilePhotoRepository>();
        var entity = await repo.GetByKeycloakUserIdAsync(keycloakUserId);

        if (entity == null || entity.PhotoData.Length == 0)
            return NotFound();

        return File(entity.PhotoData, entity.ContentType, entity.FileName);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete()
    {
        var keycloakUserId = GetKeycloakUserId();
        if (string.IsNullOrEmpty(keycloakUserId))
            return Ok(new { hasError = true, decentMessage = "User identity not found." });

        await _service.DeleteAsync(keycloakUserId);
        return Ok(new { hasError = false, decentMessage = "Profile photo deleted." });
    }

    private string? GetKeycloakUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
    }

    private static bool ValidateMagicBytes(byte[] fileBytes, string contentType)
    {
        if (!MagicBytes.TryGetValue(contentType, out var signatures))
            return true;

        foreach (var sig in signatures)
        {
            if (fileBytes.Length >= sig.Length && fileBytes.AsSpan(0, sig.Length).SequenceEqual(sig))
                return true;
        }
        return false;
    }

    private static string SanitizeFileName(string fileName)
    {
        var name = Path.GetFileNameWithoutExtension(fileName);
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        var sanitized = Regex.Replace(name, @"[^a-zA-Z0-9_\-\.]", "_");
        if (sanitized.Length > 100) sanitized = sanitized[..100];
        if (string.IsNullOrWhiteSpace(sanitized)) sanitized = "photo";
        return sanitized + ext;
    }
}
