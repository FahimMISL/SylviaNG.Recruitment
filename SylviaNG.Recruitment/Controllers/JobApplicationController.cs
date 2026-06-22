using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR,Candidate")]
    [ApiController]
    [Route("recruitment/job-application")]
    public class JobApplicationController : ControllerBase
    {
        private readonly IJobApplicationService _jobApplicationService;
        private readonly IJobApplicationRepository _jobApplicationRepo;
        private readonly ICandidateRepository _candidateRepo;
        private readonly IUnitOfWork _unitOfWork;

        public JobApplicationController(
            IJobApplicationService jobApplicationService,
            IJobApplicationRepository jobApplicationRepo,
            ICandidateRepository candidateRepo,
            IUnitOfWork unitOfWork)
        {
            _jobApplicationService = jobApplicationService;
            _jobApplicationRepo = jobApplicationRepo;
            _candidateRepo = candidateRepo;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("{jobApplicationId}")]
        public async Task<ActionResult<JobApplicationResponse>> GetById(long jobApplicationId)
        {
            if (!await IsAuthorizedForApplication(jobApplicationId))
                return Forbid();

            var result = await _jobApplicationService.GetByIdAsync(jobApplicationId);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet("job-posting/{jobPostingId}/paged")]
        public async Task<ActionResult<PagedResult<JobApplicationResponse>>> GetPagedByJobPosting(long jobPostingId, [FromQuery] PagedRequest request)
        {
            var result = await _jobApplicationService.GetPaginatedByJobPostingAsync(jobPostingId, request);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<JobApplicationResponse>>> GetAllPaged([FromQuery] PagedRequest request)
        {
            var result = await _jobApplicationService.GetAllPaginatedAsync(request);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] JobApplicationCreateRequest request)
        {
            var id = await _jobApplicationService.CreateAsync(request);
            return Ok(id);
        }

        [HttpPut("{jobApplicationId}")]
        public async Task<ActionResult> Update(long jobApplicationId, [FromBody] JobApplicationUpdateRequest request)
        {
            if (!await IsAuthorizedForApplication(jobApplicationId))
                return Forbid();

            await _jobApplicationService.UpdateAsync(jobApplicationId, request);
            return Ok();
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPost("shortlist")]
        public async Task<ActionResult> ShortlistByCandidate([FromBody] ShortlistRequest request)
        {
            var applications = await _jobApplicationRepo.FindAsync(
                a => a.CandidateId == request.CandidateId
                     && a.JobPostingId == request.JobPostingId
                     && a.IsActive);
            var application = applications.FirstOrDefault();

            if (application == null)
                return NotFound(new { message = "Candidate has not applied for this job. Only applied candidates can be shortlisted." });

            await _jobApplicationService.UpdateAsync(application.JobApplicationId,
                new JobApplicationUpdateRequest { ApplicationStatus = Domain.Enums.ApplicationStatusEnum.Shortlisted });

            return Ok(new { message = "Candidate shortlisted successfully.", jobApplicationId = application.JobApplicationId });
        }

        [HttpDelete("{jobApplicationId}")]
        public async Task<ActionResult> Delete(long jobApplicationId)
        {
            if (!await IsAuthorizedForApplication(jobApplicationId))
                return Forbid();

            await _jobApplicationService.DeleteAsync(jobApplicationId);
            return Ok();
        }

        private async Task<bool> IsAuthorizedForApplication(long jobApplicationId)
        {
            var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            if (userRoles.Contains("Admin") || userRoles.Contains("HR"))
                return true;

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value
                ?? User.FindFirst("email")?.Value;
            if (string.IsNullOrEmpty(userEmail))
                return false;

            var application = await _jobApplicationRepo.GetByIdAsync(jobApplicationId);
            if (application?.CandidateId == null)
                return false;

            var candidate = await _candidateRepo.GetByIdAsync(application.CandidateId.Value);
            return candidate != null &&
                   string.Equals(candidate.Email, userEmail, StringComparison.OrdinalIgnoreCase);
        }
    }
}
