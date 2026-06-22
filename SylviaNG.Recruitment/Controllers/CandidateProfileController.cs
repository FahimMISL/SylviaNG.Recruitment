using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Features.Candidates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.Infrastructure.Services;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Controllers;

[Authorize(Roles = "Candidate")]
[ApiController]
[Route("recruitment/candidate-profile")]
public class CandidateProfileController : ControllerBase
{
    private readonly ICandidateRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly EmailSettings _emailSettings;
    private readonly IEligibilityCheckService _eligibilityCheck;
    private readonly IEmailOtpService _emailOtpService;

    public CandidateProfileController(ICandidateRepository repository, IUnitOfWork unitOfWork,
        IEmailService emailService, IOptions<EmailSettings> emailSettings,
        IEligibilityCheckService eligibilityCheck, IEmailOtpService emailOtpService)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _emailSettings = emailSettings.Value;
        _eligibilityCheck = eligibilityCheck;
        _emailOtpService = emailOtpService;
    }

    [HttpGet("my-profile")]
    public async Task<IActionResult> GetMyProfile()
    {
        var keycloakUserId = GetKeycloakUserId();
        if (string.IsNullOrEmpty(keycloakUserId))
            return Unauthorized();

        var candidate = await _repository.GetByKeycloakUserIdAsync(keycloakUserId);
        if (candidate == null)
            return Ok(new { hasError = true, decentMessage = "No candidate profile found.", errorDetails = (string?)null, content = (object?)null });

        return Ok(candidate.ToResponse());
    }

    [HttpPut("my-profile")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] CandidateUpdateRequest request)
    {
        var keycloakUserId = GetKeycloakUserId();
        if (string.IsNullOrEmpty(keycloakUserId))
            return Unauthorized();

        var candidate = await _repository.GetByKeycloakUserIdAsync(keycloakUserId);
        if (candidate == null)
            return NotFound();

        candidate.ApplyUpdate(request);
        _repository.Update(candidate);
        await _unitOfWork.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("my-skills")]
    public async Task<IActionResult> GetMySkills()
    {
        var candidate = await GetMyCandidate();
        if (candidate == null) return Unauthorized();

        var skills = await _unitOfWork.Context.CandidateSkills
            .Where(s => s.CandidateId == candidate.CandidateId && s.IsActive)
            .Select(s => new { s.CandidateSkillId, s.CandidateId, s.SkillName, s.ProficiencyLevel })
            .ToListAsync();

        return Ok(skills);
    }

    [HttpPost("my-skills")]
    public async Task<IActionResult> AddMySkill([FromBody] AddSkillRequest request)
    {
        var candidate = await GetMyCandidate();
        if (candidate == null) return Unauthorized();

        var skill = new CandidateSkill
        {
            CandidateId = candidate.CandidateId,
            SkillName = request.SkillName,
            ProficiencyLevel = request.ProficiencyLevel,
            IsActive = true
        };

        await _unitOfWork.Context.CandidateSkills.AddAsync(skill);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { skill.CandidateSkillId, skill.CandidateId, skill.SkillName, skill.ProficiencyLevel });
    }

    [HttpDelete("my-skills/{skillId}")]
    public async Task<IActionResult> RemoveMySkill(long skillId)
    {
        var candidate = await GetMyCandidate();
        if (candidate == null) return Unauthorized();

        var skill = await _unitOfWork.Context.CandidateSkills
            .FirstOrDefaultAsync(s => s.CandidateSkillId == skillId && s.CandidateId == candidate.CandidateId);

        if (skill == null) return NotFound();

        _unitOfWork.Context.CandidateSkills.Remove(skill);
        await _unitOfWork.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("open-jobs")]
    public async Task<IActionResult> GetOpenJobs([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
    {
        if (pageSize > 50) pageSize = 50;
        if (page < 1) page = 1;

        var query = _unitOfWork.Context.Set<JobPosting>()
            .Where(j => j.IsActive && j.Status == JobStatusEnum.Open);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(j => j.Title.Contains(search) || (j.Description != null && j.Description.Contains(search)));

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(j => j.PostingDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(j => new
            {
                j.JobPostingId,
                j.Title,
                j.Description,
                j.Location,
                j.Requirements,
                j.NumberOfPositions,
                j.EmploymentType,
                j.Status,
                j.MinSalary,
                j.MaxSalary,
                j.PostingDate,
                j.ClosingDate,
                j.DepartmentId,
                j.SiteId,
                j.MinAge,
                j.MaxAge,
                j.MinExperienceYears,
                j.MinEducationLevel,
                j.RequiredDistrict
            })
            .ToListAsync();

        return Ok(new { items, totalCount = total, page, pageSize });
    }

    [HttpGet("my-applications")]
    public async Task<IActionResult> GetMyApplications()
    {
        var candidate = await GetMyCandidate();
        if (candidate == null) return Unauthorized();

        var applications = await _unitOfWork.Context.Set<JobApplication>()
            .Where(a => a.CandidateId == candidate.CandidateId && a.IsActive)
            .Join(
                _unitOfWork.Context.Set<JobPosting>(),
                a => a.JobPostingId,
                j => j.JobPostingId,
                (a, j) => new
                {
                    a.JobApplicationId,
                    a.JobPostingId,
                    JobPostingTitle = j.Title,
                    a.ApplicationStatus,
                    a.AppliedDate,
                    a.CoverLetter,
                    a.IsActive
                })
            .OrderByDescending(a => a.AppliedDate)
            .ToListAsync();

        return Ok(applications);
    }

    [HttpPost("apply/{jobPostingId}")]
    public async Task<IActionResult> ApplyForJob(long jobPostingId, [FromBody] CandidateApplyRequest request)
    {
        var keycloakUserId = GetKeycloakUserId();
        if (string.IsNullOrEmpty(keycloakUserId)) return Unauthorized();

        var candidate = await _unitOfWork.Context.Set<Candidate>()
            .Include(c => c.Educations.Where(e => e.IsActive))
            .FirstOrDefaultAsync(c => c.KeycloakUserId == keycloakUserId);
        if (candidate == null) return Unauthorized();

        var job = await _unitOfWork.Context.Set<JobPosting>()
            .FirstOrDefaultAsync(j => j.JobPostingId == jobPostingId && j.IsActive && j.Status == JobStatusEnum.Open);
        if (job == null) return NotFound("Job posting not found or not open.");

        var alreadyApplied = await _unitOfWork.Context.Set<JobApplication>()
            .AnyAsync(a => a.JobPostingId == jobPostingId && a.CandidateId == candidate.CandidateId && a.IsActive);
        if (alreadyApplied) return BadRequest("You have already applied for this job.");

        var (isEligible, reasons) = _eligibilityCheck.CheckEligibility(candidate, job);
        if (!isEligible)
            return BadRequest(new { message = "You do not meet the eligibility requirements.", reasons });

        var fee = await _unitOfWork.Context.Set<ApplicationFee>()
            .FirstOrDefaultAsync(f => f.JobPostingId == jobPostingId && f.IsActive);
        if (fee != null)
        {
            var hasPaid = await _unitOfWork.Context.Set<PaymentTransaction>()
                .AnyAsync(t => t.CandidateId == candidate.CandidateId
                    && t.JobPostingId == jobPostingId
                    && t.PaymentStatus == PaymentStatusEnum.Completed
                    && t.IsActive);
            if (!hasPaid)
                return BadRequest(new { message = "Application fee payment is required before applying.", feeAmount = fee.Amount, feeCurrency = fee.Currency });
        }

        var profileSnapshot = System.Text.Json.JsonSerializer.Serialize(new
        {
            candidate.FullName,
            candidate.Email,
            candidate.Phone,
            candidate.DateOfBirth,
            candidate.NationalIdNumber,
            candidate.PresentAddress,
            candidate.PermanentAddress,
            candidate.IsEmailVerified,
            candidate.SignatureUrl,
            Educations = candidate.Educations?.Select(e => new { e.Degree, e.Institution, e.PassingYear, e.Result }).ToList()
        });

        var application = new JobApplication
        {
            JobPostingId = jobPostingId,
            CandidateId = candidate.CandidateId,
            CandidateName = candidate.FullName ?? "",
            CandidateEmail = candidate.Email,
            CandidatePhone = candidate.Phone,
            CoverLetter = request.CoverLetter,
            ProfileSnapshotJson = profileSnapshot,
            ApplicationStatus = ApplicationStatusEnum.Applied,
            AppliedDate = DateTime.UtcNow,
            IsActive = true
        };

        await _unitOfWork.Context.Set<JobApplication>().AddAsync(application);

        var candidateName = candidate.FullName ?? "Candidate";
        var candidateNotification = new UserNotification
        {
            KeycloakUserId = candidate.KeycloakUserId ?? "",
            Title = "Application Submitted",
            Message = $"Your application for \"{job.Title}\" has been submitted successfully.",
            NotificationType = UserNotificationTypeEnum.Success,
            ActionUrl = "/my-applications",
            IsActive = true
        };
        await _unitOfWork.Context.Set<UserNotification>().AddAsync(candidateNotification);

        var hrUsers = await _unitOfWork.Context.Set<Domain.Entities.User>()
            .Where(u => u.IsActive && u.KeycloakUserId != (candidate.KeycloakUserId ?? ""))
            .Select(u => u.KeycloakUserId)
            .ToListAsync();

        foreach (var hrKeycloakId in hrUsers)
        {
            var hrNotification = new UserNotification
            {
                KeycloakUserId = hrKeycloakId,
                Title = "New Application Received",
                Message = $"{candidateName} applied for \"{job.Title}\" position.",
                NotificationType = UserNotificationTypeEnum.Info,
                ActionUrl = $"/job-postings/{jobPostingId}/pipeline",
                IsActive = true
            };
            await _unitOfWork.Context.Set<UserNotification>().AddAsync(hrNotification);
        }

        await _unitOfWork.SaveChangesAsync();

        _ = Task.Run(async () =>
        {
            try
            {
                await _emailService.SendAsync(
                    candidate.Email ?? "", candidateName,
                    $"Application Submitted — {job.Title}",
                    EmailTemplates.ApplicationSubmittedCandidate(candidateName, job.Title));

                if (!string.IsNullOrEmpty(_emailSettings.HrNotificationEmail))
                {
                    await _emailService.SendAsync(
                        _emailSettings.HrNotificationEmail, "HR Team",
                        $"New Application — {job.Title}",
                        EmailTemplates.ApplicationReceivedHR(candidateName, candidate.Email ?? "", job.Title));
                }
            }
            catch { }
        });

        return Ok(new { application.JobApplicationId });
    }

    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp()
    {
        var candidate = await GetMyCandidate();
        if (candidate == null) return Unauthorized();
        if (string.IsNullOrEmpty(candidate.Email)) return BadRequest("No email address on profile.");

        var (success, message) = await _emailOtpService.SendOtpAsync(candidate.Email);
        return Ok(new { success, message });
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
    {
        var candidate = await GetMyCandidate();
        if (candidate == null) return Unauthorized();
        if (string.IsNullOrEmpty(candidate.Email)) return BadRequest("No email address on profile.");

        var (success, message) = await _emailOtpService.VerifyOtpAsync(candidate.Email, request.OtpCode);
        if (success)
        {
            candidate.IsEmailVerified = true;
            _repository.Update(candidate);
            await _unitOfWork.SaveChangesAsync();
        }
        return Ok(new { success, message });
    }

    [HttpPost("upload-signature")]
    public async Task<IActionResult> UploadSignature(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided.");
        if (file.Length > 1 * 1024 * 1024)
            return BadRequest("Signature file must be under 1MB.");

        var allowedTypes = new[] { "image/jpeg", "image/png" };
        if (!allowedTypes.Contains(file.ContentType))
            return BadRequest("Only JPEG and PNG files are allowed.");

        using var headerStream = new MemoryStream();
        await file.OpenReadStream().CopyToAsync(headerStream);
        var bytes = headerStream.ToArray();
        var isJpeg = bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF;
        var isPng = bytes.Length >= 8 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47;
        if (!isJpeg && !isPng)
            return BadRequest("File content does not match a valid JPEG or PNG image.");

        var candidate = await GetMyCandidate();
        if (candidate == null) return Unauthorized();

        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "signatures");
        Directory.CreateDirectory(uploadsDir);

        var extension = isPng ? ".png" : ".jpg";
        var fileName = $"{candidate.CandidateId}_{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(uploadsDir, fileName);

        await System.IO.File.WriteAllBytesAsync(filePath, bytes);

        candidate.SignatureUrl = $"/uploads/signatures/{fileName}";
        _repository.Update(candidate);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { signatureUrl = candidate.SignatureUrl });
    }

    private async Task<Candidate?> GetMyCandidate()
    {
        var keycloakUserId = GetKeycloakUserId();
        if (string.IsNullOrEmpty(keycloakUserId)) return null;
        return await _repository.GetByKeycloakUserIdAsync(keycloakUserId);
    }

    private string? GetKeycloakUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
    }
}

public class AddSkillRequest
{
    public string SkillName { get; set; } = string.Empty;
    public string? ProficiencyLevel { get; set; }
}

public class CandidateApplyRequest
{
    public string? CoverLetter { get; set; }
}

public class VerifyOtpRequest
{
    public string OtpCode { get; set; } = string.Empty;
}
