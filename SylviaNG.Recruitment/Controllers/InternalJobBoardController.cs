using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.JobPostings.Commands.JobApplicationSubmit;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Features.JobPostings.Queries.JobPostingGetAllInternalPaged;
using SylviaNG.Recruitment.Application.Features.JobPostings.Queries.JobPostingGetInternalById;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Controllers
{
    /// <summary>
    /// Internal job board: browsing of internally-visible job postings and applying, open to any
    /// authenticated user regardless of role. EP-03. Deliberately carries no [Authorize]/[AllowAnonymous]
    /// attribute - it inherits the global RequireAuthenticatedUser() MVC filter registered in
    /// Program.cs, which is exactly the "any logged-in user" requirement here (there is no distinct
    /// "Employee" role in UserRoleEnum to restrict to).
    /// </summary>
    [ApiController]
    [Route("recruitment/internal-job-board")]
    public class InternalJobBoardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InternalJobBoardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Browse paginated, published (Open, CircularType InternalOnly/Both) job postings.
        /// </summary>
        [HttpGet("job-postings")]
        public async Task<ActionResult<PagedResult<JobPostingResponse>>> GetAll(
            [FromQuery] PagedRequest request,
            [FromQuery] string? location,
            [FromQuery] long? departmentId,
            [FromQuery] EmploymentTypeEnum? employmentType,
            [FromQuery] int? maxExperienceYears)
        {
            var result = await _mediator.Send(new JobPostingGetAllInternalPagedQuery(request, location, departmentId, employmentType, maxExperienceYears));
            return Ok(result);
        }

        /// <summary>
        /// Get a single published job posting's detail view.
        /// </summary>
        [HttpGet("job-postings/{jobPostingId}")]
        public async Task<ActionResult<JobPostingResponse>> GetById(long jobPostingId)
        {
            var result = await _mediator.Send(new JobPostingGetInternalByIdQuery(jobPostingId));
            return Ok(result);
        }

        /// <summary>
        /// Submit an internal application (with optional CV upload) to a published job posting.
        /// </summary>
        [HttpPost("job-postings/{jobPostingId}/apply")]
        public async Task<ActionResult<JobApplicationResponse>> Apply(long jobPostingId, [FromForm] JobApplicationSubmitRequest request)
        {
            request.JobPostingId = jobPostingId;
            var result = await _mediator.Send(new JobApplicationSubmitCommand(request, ApplicationSourceEnum.Internal));
            return Ok(result);
        }
    }
}
