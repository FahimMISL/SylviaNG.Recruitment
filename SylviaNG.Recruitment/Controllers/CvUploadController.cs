using System.Security.Claims;
using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Features.CandidateDocuments.Commands.CandidateDocumentCreate;
using SylviaNG.Recruitment.Application.Features.CandidateDocuments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Controllers;

[Authorize(Roles = "Admin,HR,Candidate")]
[ApiController]
[Route("recruitment/cv")]
public class CvUploadController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICvParsingService _cvParser;
    private readonly ICandidateRepository _candidateRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CvUploadController> _logger;

    private static readonly Dictionary<string, byte[][]> AllowedMagicBytes = new()
    {
        { ".pdf", new[] { new byte[] { 0x25, 0x50, 0x44, 0x46 } } },
        { ".doc", new[] { new byte[] { 0xD0, 0xCF, 0x11, 0xE0 } } },
        { ".docx", new[] { new byte[] { 0x50, 0x4B, 0x03, 0x04 } } },
    };

    private static readonly Dictionary<string, string> SafeContentTypes = new()
    {
        { ".pdf", "application/pdf" },
        { ".doc", "application/msword" },
        { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
        { ".txt", "text/plain" },
    };

    public CvUploadController(
        IMediator mediator,
        ICvParsingService cvParser,
        ICandidateRepository candidateRepo,
        IUnitOfWork unitOfWork,
        ILogger<CvUploadController> logger)
    {
        _mediator = mediator;
        _cvParser = cvParser;
        _candidateRepo = candidateRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpPost("upload/{candidateId}")]
    public async Task<IActionResult> UploadAndParse(long candidateId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided.");

        if (!await IsAuthorizedForCandidate(candidateId))
            return Forbid();

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!SafeContentTypes.ContainsKey(ext))
            return BadRequest("Only PDF, DOC, DOCX, and TXT files are allowed.");

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest("File size must be under 5MB.");

        var candidate = await _candidateRepo.GetByIdAsync(candidateId);
        if (candidate == null)
            return NotFound("Candidate not found.");

        byte[] fileBytes;
        using (var ms = new MemoryStream())
        {
            await file.CopyToAsync(ms);
            fileBytes = ms.ToArray();
        }

        if (ext != ".txt" && !ValidateMagicBytes(fileBytes, ext))
            return Ok(new { hasError = true, decentMessage = "File content does not match its extension. Upload rejected." });

        var sanitizedName = SanitizeFileName(file.FileName);
        var safeContentType = SafeContentTypes[ext];

        var docRequest = new CandidateDocumentCreateRequest
        {
            CandidateId = candidateId,
            DocumentType = CandidateDocumentTypeEnum.CV,
            FileName = sanitizedName,
            FileUrl = $"db://{candidateId}/{sanitizedName}",
            ContentType = safeContentType,
            FileSizeBytes = file.Length,
            FileData = fileBytes,
            IsDefault = true
        };
        var docId = await _mediator.Send(new CandidateDocumentCreateCommand(docRequest));

        CvParsedData? parsed = null;
        try
        {
            using var parseStream = new MemoryStream(fileBytes);
            parsed = await _cvParser.ParseCvAsync(parseStream, sanitizedName);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "CV parsing failed for candidate {CandidateId}, document {DocumentId}", candidateId, docId);
        }

        if (parsed != null)
        {
            var cvResult = new CvParsingResult
            {
                CandidateId = candidateId,
                CandidateDocumentId = docId,
                ParsedDataJson = JsonSerializer.Serialize(parsed),
                ConfidenceScore = parsed.ConfidenceScore,
                IsReviewedByCandidate = false,
                ParsedAt = DateTime.UtcNow,
                IsActive = true
            };

            _unitOfWork.Context.CvParsingResults.Add(cvResult);
            await _unitOfWork.SaveChangesAsync();
        }

        return Ok(new
        {
            documentId = docId,
            parsedData = parsed,
            fileName = sanitizedName
        });
    }

    [HttpGet("download/{candidateDocumentId}")]
    public async Task<IActionResult> Download(long candidateDocumentId)
    {
        var doc = await _unitOfWork.Context.CandidateDocuments
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.CandidateDocumentId == candidateDocumentId);

        if (doc == null || doc.FileData == null)
            return NotFound();

        if (!await IsAuthorizedForCandidate(doc.CandidateId))
            return Forbid();

        var ext = Path.GetExtension(doc.FileName).ToLowerInvariant();
        var safeContentType = SafeContentTypes.GetValueOrDefault(ext, "application/octet-stream");
        var safeName = SanitizeFileName(doc.FileName);

        return File(doc.FileData, safeContentType, safeName);
    }

    [HttpPost("apply-parsed/{candidateId}")]
    public async Task<IActionResult> ApplyParsedData(long candidateId, [FromBody] ApplyParsedDataRequest request)
    {
        if (!await IsAuthorizedForCandidate(candidateId))
            return Forbid();

        var candidate = await _candidateRepo.GetByIdAsync(candidateId);
        if (candidate == null)
            return NotFound("Candidate not found.");

        if (request.FullName != null) candidate.FullName = request.FullName;
        if (request.Email != null) candidate.Email = request.Email;
        if (request.Phone != null) candidate.Phone = request.Phone;
        if (request.PresentAddress != null) candidate.PresentAddress = request.PresentAddress;
        if (request.LinkedInUrl != null) candidate.LinkedInUrl = request.LinkedInUrl;
        if (request.GitHubUrl != null) candidate.GitHubUrl = request.GitHubUrl;
        if (request.PortfolioUrl != null) candidate.PortfolioUrl = request.PortfolioUrl;
        if (request.CurrentDesignation != null) candidate.CurrentDesignation = request.CurrentDesignation;
        if (request.CurrentOrganization != null) candidate.CurrentOrganization = request.CurrentOrganization;
        if (request.TotalExperienceYears.HasValue) candidate.TotalExperienceYears = request.TotalExperienceYears;

        _candidateRepo.Update(candidate);
        await _unitOfWork.SaveChangesAsync();

        return Ok("Profile updated from CV data.");
    }

    private async Task<bool> IsAuthorizedForCandidate(long candidateId)
    {
        var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        if (userRoles.Contains("Admin") || userRoles.Contains("HR"))
            return true;

        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value
            ?? User.FindFirst("email")?.Value;
        if (string.IsNullOrEmpty(userEmail))
            return false;

        var candidate = await _candidateRepo.GetByIdAsync(candidateId);
        return candidate != null &&
               string.Equals(candidate.Email, userEmail, StringComparison.OrdinalIgnoreCase);
    }

    private static bool ValidateMagicBytes(byte[] fileBytes, string extension)
    {
        if (!AllowedMagicBytes.TryGetValue(extension, out var signatures))
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

        var sanitized = System.Text.RegularExpressions.Regex.Replace(name, @"[^a-zA-Z0-9_\-\.]", "_");
        if (sanitized.Length > 100) sanitized = sanitized[..100];
        if (string.IsNullOrWhiteSpace(sanitized)) sanitized = "document";

        return sanitized + ext;
    }
}

public class ApplyParsedDataRequest
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? PresentAddress { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? GitHubUrl { get; set; }
    public string? PortfolioUrl { get; set; }
    public string? CurrentDesignation { get; set; }
    public string? CurrentOrganization { get; set; }
    public int? TotalExperienceYears { get; set; }
}
