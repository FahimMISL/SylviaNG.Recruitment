using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.JobPostings.Commands.JobApplicationSubmit;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Controllers
{
    [ApiController]
    [Route("recruitment/job-application")]
    public class JobApplicationController : ControllerBase
    {
        private readonly IJobApplicationService _jobApplicationService;
        private readonly IMediator _mediator;

        public JobApplicationController(IJobApplicationService jobApplicationService, IMediator mediator)
        {
            _jobApplicationService = jobApplicationService;
            _mediator = mediator;
        }

        /// <summary>
        /// Get a job application by ID.
        /// </summary>
        [HttpGet("{jobApplicationId}")]
        public async Task<ActionResult<JobApplicationResponse>> GetById(long jobApplicationId)
        {
            var result = await _jobApplicationService.GetByIdAsync(jobApplicationId);
            return Ok(result);
        }

        /// <summary>
        /// Get paginated job applications for a specific job posting.
        /// </summary>
        [HttpGet("job-posting/{jobPostingId}/paged")]
        public async Task<ActionResult<PagedResult<JobApplicationResponse>>> GetPagedByJobPosting(long jobPostingId, [FromQuery] PagedRequest request)
        {
            var result = await _jobApplicationService.GetPaginatedByJobPostingAsync(jobPostingId, request);
            return Ok(result);
        }

        /// <summary>
        /// Create a new job application.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] JobApplicationCreateRequest request)
        {
            var id = await _jobApplicationService.CreateAsync(request);
            return Ok(id);
        }

        /// <summary>
        /// Update an existing job application.
        /// </summary>
        [HttpPut("{jobApplicationId}")]
        public async Task<ActionResult> Update(long jobApplicationId, [FromBody] JobApplicationUpdateRequest request)
        {
            await _jobApplicationService.UpdateAsync(jobApplicationId, request);
            return Ok();
        }

        /// <summary>
        /// Delete a job application.
        /// </summary>
        [HttpDelete("{jobApplicationId}")]
        public async Task<ActionResult> Delete(long jobApplicationId)
        {
            await _jobApplicationService.DeleteAsync(jobApplicationId);
            return Ok();
        }

        /// <summary>
        /// ATS dashboard: paginated, filterable view of every application across all job postings (US-035),
        /// plus candidate-attribute filters (education/experience/skills/location/age) scoped to one
        /// vacancy (US-050).
        /// </summary>
        [HttpGet("dashboard/paged")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult<PagedResult<JobApplicationDashboardResponse>>> GetDashboardPaged(
            [FromQuery] PagedRequest request,
            [FromQuery] JobApplicationAttributeFilterRequest filter)
        {
            var result = await _jobApplicationService.GetDashboardPagedAsync(request, filter);
            return Ok(result);
        }

        /// <summary>
        /// Full application detail: candidate snapshot, CV link, and status-history audit trail (US-035 AC4).
        /// </summary>
        [HttpGet("{jobApplicationId}/detail")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult<JobApplicationDetailResponse>> GetDetail(long jobApplicationId)
        {
            var result = await _jobApplicationService.GetDetailAsync(jobApplicationId);
            return Ok(result);
        }

        /// <summary>
        /// IDs of every application matching the dashboard filters, unpaginated - backs "select all
        /// N matching applications" bulk selection across pages (US-047 AC5).
        /// </summary>
        [HttpGet("dashboard/matching-ids")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult<List<long>>> GetDashboardMatchingIds([FromQuery] JobApplicationAttributeFilterRequest filter)
        {
            var result = await _jobApplicationService.GetDashboardMatchingIdsAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// Configurable reject/withdraw reasons for the given target status (US-036 AC3).
        /// </summary>
        [HttpGet("status-reasons")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult<List<ApplicationStatusReasonResponse>>> GetStatusReasons([FromQuery] ApplicationStatusEnum status)
        {
            var result = await _jobApplicationService.GetStatusReasonsAsync(status);
            return Ok(result);
        }

        /// <summary>
        /// Move a single application to a new status (US-036).
        /// </summary>
        [HttpPatch("{jobApplicationId}/status")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult> UpdateStatus(long jobApplicationId, [FromBody] JobApplicationStatusUpdateRequest request)
        {
            await _jobApplicationService.UpdateStatusAsync(jobApplicationId, request);
            return Ok();
        }

        /// <summary>
        /// Move a batch of applications to a new status at once, e.g. 50 at a time (US-035 AC5).
        /// </summary>
        [HttpPatch("bulk-status")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult<JobApplicationBulkStatusUpdateResponse>> BulkUpdateStatus([FromBody] JobApplicationBulkStatusUpdateRequest request)
        {
            var result = await _jobApplicationService.BulkUpdateStatusAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// HR submits an application on behalf of a candidate (agency/direct outreach), to any open
        /// vacancy regardless of its audience restriction. Recorded with Source=Admin (US-034).
        /// </summary>
        [HttpPost("apply-on-behalf")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult<JobApplicationResponse>> ApplyOnBehalf([FromForm] JobApplicationSubmitRequest request)
        {
            var result = await _mediator.Send(new JobApplicationSubmitCommand(request, ApplicationSourceEnum.Admin));
            return Ok(result);
        }

        /// <summary>
        /// The current candidate's own submitted applications, with status, interview dates,
        /// and a computed CanWithdraw flag per row (US-040 AC1/AC2/AC3).
        /// </summary>
        [HttpGet("my-applications")]
        [Authorize(Roles = "Candidate")]
        public async Task<ActionResult<List<MyApplicationResponse>>> GetMyApplications()
        {
            var result = await _jobApplicationService.GetMyApplicationsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Candidate withdraws their own active application (US-040 AC4). The confirmation
        /// prompt is a frontend concern; this call performs the withdrawal directly.
        /// </summary>
        [HttpPatch("my-applications/{jobApplicationId}/withdraw")]
        [Authorize(Roles = "Candidate")]
        public async Task<ActionResult> WithdrawMyApplication(long jobApplicationId)
        {
            await _jobApplicationService.WithdrawMyApplicationAsync(jobApplicationId);
            return Ok();
        }
    }
}
