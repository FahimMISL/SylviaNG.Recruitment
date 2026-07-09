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
    /// Public career portal: anonymous browsing of externally-visible job postings and anonymous apply.
    /// EP-03. Every action is explicitly [AllowAnonymous] to opt out of the global
    /// RequireAuthenticatedUser() MVC filter registered in Program.cs.
    /// </summary>
    [ApiController]
    [Route("recruitment/career-portal")]
    [AllowAnonymous]
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
        public async Task<ActionResult<JobPostingResponse>> GetById(long jobPostingId)
        {
            var result = await _mediator.Send(new JobPostingGetPublicByIdQuery(jobPostingId));
            return Ok(result);
        }

        /// <summary>
        /// Submit an anonymous application (with optional CV upload) to a published job posting.
        /// </summary>
        [HttpPost("job-postings/{jobPostingId}/apply")]
        public async Task<ActionResult<JobApplicationResponse>> Apply(long jobPostingId, [FromForm] JobApplicationSubmitRequest request)
        {
            request.JobPostingId = jobPostingId;
            var result = await _mediator.Send(new JobApplicationSubmitCommand(request, ApplicationSourceEnum.External));
            return Ok(result);
        }
    }
}
