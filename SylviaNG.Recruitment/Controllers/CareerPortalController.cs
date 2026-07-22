using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.JobPostings.Commands.JobApplicationSubmit;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Features.JobPostings.Queries.JobPostingGetAllPublicPaged;
using SylviaNG.Recruitment.Application.Features.JobPostings.Queries.JobPostingGetPublicById;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Controllers
{
    /// <summary>
    /// Public career portal: anonymous browsing of externally-visible job postings, but applying
    /// requires a logged-in Candidate account - no guest apply. Browse actions are individually
    /// [AllowAnonymous] to opt out of the global RequireAuthenticatedUser() MVC filter registered
    /// in Program.cs; Apply is deliberately left off that list so it falls back to that filter,
    /// narrowed further to the Candidate role.
    /// </summary>
    [ApiController]
    [Route("recruitment/career-portal")]
    public class CareerPortalController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CareerPortalController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Browse paginated, published (Open, CircularType ExternalOnly/Both) job postings.
        /// </summary>
        [HttpGet("job-postings")]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResult<JobPostingResponse>>> GetAll(
            [FromQuery] PagedRequest request,
            [FromQuery] string? location,
            [FromQuery] long? departmentId,
            [FromQuery] EmploymentTypeEnum? employmentType,
            [FromQuery] int? maxExperienceYears)
        {
            var result = await _mediator.Send(new JobPostingGetAllPublicPagedQuery(request, location, departmentId, employmentType, maxExperienceYears));
            return Ok(result);
        }

        /// <summary>
        /// Get a single published job posting's detail view.
        /// </summary>
        [HttpGet("job-postings/{jobPostingId}")]
        [AllowAnonymous]
        public async Task<ActionResult<JobPostingResponse>> GetById(long jobPostingId)
        {
            var result = await _mediator.Send(new JobPostingGetPublicByIdQuery(jobPostingId));
            return Ok(result);
        }

        /// <summary>
        /// Submit an application (with optional CV upload) to a published job posting. Requires a
        /// logged-in Candidate account - no guest apply.
        /// </summary>
        [HttpPost("job-postings/{jobPostingId}/apply")]
        [Authorize(Roles = "Candidate")]
        public async Task<ActionResult<JobApplicationResponse>> Apply(long jobPostingId, [FromForm] JobApplicationSubmitRequest request)
        {
            request.JobPostingId = jobPostingId;
            var result = await _mediator.Send(new JobApplicationSubmitCommand(request, ApplicationSourceEnum.External));
            return Ok(result);
        }
    }
}
