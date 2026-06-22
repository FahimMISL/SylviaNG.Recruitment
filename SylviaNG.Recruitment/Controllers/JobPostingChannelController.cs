using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.JobPostingChannels.Queries.JobPostingChannelGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.JobPostingChannels.Commands.JobPostingChannelCreate;
using SylviaNG.Recruitment.Application.Features.JobPostingChannels.Commands.JobPostingChannelDelete;
using SylviaNG.Recruitment.Application.Features.JobPostingChannels.Commands.JobPostingChannelUpdate;
using SylviaNG.Recruitment.Application.Features.JobPostingChannels.Models;
using SylviaNG.Recruitment.Application.Features.JobPostingChannels.Queries.JobPostingChannelGetAll;
using SylviaNG.Recruitment.Application.Features.JobPostingChannels.Queries.JobPostingChannelGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/job-posting-channel")]
    public class JobPostingChannelController : ControllerBase
    {
        private readonly IMediator _mediator;

        public JobPostingChannelController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<JobPostingChannelResponse>>> GetAll()
        {
            var result = await _mediator.Send(new JobPostingChannelGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<JobPostingChannelResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new JobPostingChannelGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{jobPostingChannelId}")]
        public async Task<ActionResult<JobPostingChannelResponse>> GetById(long jobPostingChannelId)
        {
            var result = await _mediator.Send(new JobPostingChannelGetByIdQuery(jobPostingChannelId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] JobPostingChannelCreateRequest request)
        {
            var id = await _mediator.Send(new JobPostingChannelCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{jobPostingChannelId}")]
        public async Task<ActionResult> Update(long jobPostingChannelId, [FromBody] JobPostingChannelUpdateRequest request)
        {
            await _mediator.Send(new JobPostingChannelUpdateCommand(jobPostingChannelId, request));
            return Ok();
        }

        [HttpDelete("{jobPostingChannelId}")]
        public async Task<ActionResult> Delete(long jobPostingChannelId)
        {
            await _mediator.Send(new JobPostingChannelDeleteCommand(jobPostingChannelId));
            return Ok();
        }
    }
}
