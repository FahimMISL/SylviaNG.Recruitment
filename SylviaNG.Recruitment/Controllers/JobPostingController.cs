using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.JobPostings.Commands.JobPostingCreate;
using SylviaNG.Recruitment.Application.Features.JobPostings.Commands.JobPostingDelete;
using SylviaNG.Recruitment.Application.Features.JobPostings.Commands.JobPostingUpdate;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Features.JobPostings.Queries.JobPostingGetAll;
using SylviaNG.Recruitment.Application.Features.JobPostings.Queries.JobPostingGetAllPaged;
using SylviaNG.Recruitment.Application.Features.JobPostings.Queries.JobPostingGetById;
using SylviaNG.Recruitment.Application.Features.JobPostings.Queries.JobPostingGetMyPostings;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Controllers
{
    [ApiController]
    [Route("recruitment/job-posting")]
    [Authorize(Roles = "Admin,HR")]
    public class JobPostingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public JobPostingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all job postings.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<JobPostingResponse>>> GetAll()
        {
            var result = await _mediator.Send(new JobPostingGetAllQuery());
            return Ok(result);
        }

        /// <summary>
        /// Get a job posting by ID.
        /// </summary>
        [HttpGet("{jobPostingId}")]
        public async Task<ActionResult<JobPostingResponse>> GetById(long jobPostingId)
        {
            var result = await _mediator.Send(new JobPostingGetByIdQuery(jobPostingId));
            return Ok(result);
        }

        /// <summary>
        /// EP-15/US-113: postings created by the current user - additive alongside GetAll/GetPaged,
        /// does not restrict them. Empty for users with no resolvable local UserAccount (e.g. the
        /// hardcoded-auth scheme, or Keycloak users invited before EP-15 added the UserAccount table).
        /// </summary>
        [HttpGet("my-postings")]
        public async Task<ActionResult<List<JobPostingResponse>>> GetMyPostings()
        {
            var result = await _mediator.Send(new JobPostingGetMyPostingsQuery());
            return Ok(result);
        }

        /// <summary>
        /// Get paginated job postings with search and sort.
        /// </summary>
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<JobPostingResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new JobPostingGetAllPagedQuery(request));
            return Ok(result);
        }

        /// <summary>
        /// Create a new job posting.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] JobPostingCreateRequest request)
        {
            var id = await _mediator.Send(new JobPostingCreateCommand(request));
            return Ok(id);
        }

        /// <summary>
        /// Update an existing job posting.
        /// </summary>
        [HttpPut("{jobPostingId}")]
        public async Task<ActionResult> Update(long jobPostingId, [FromBody] JobPostingUpdateRequest request)
        {
            await _mediator.Send(new JobPostingUpdateCommand(jobPostingId, request));
            return Ok();
        }

        /// <summary>
        /// Delete a job posting.
        /// </summary>
        [HttpDelete("{jobPostingId}")]
        public async Task<ActionResult> Delete(long jobPostingId)
        {
            await _mediator.Send(new JobPostingDeleteCommand(jobPostingId));
            return Ok();
        }
    }
}
