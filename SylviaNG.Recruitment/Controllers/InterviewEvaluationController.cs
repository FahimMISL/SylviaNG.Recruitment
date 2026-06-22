using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Queries.InterviewEvaluationGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Commands.InterviewEvaluationCreate;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Commands.InterviewEvaluationDelete;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Commands.InterviewEvaluationUpdate;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Models;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Queries.InterviewEvaluationGetAll;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Queries.InterviewEvaluationGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/interview-evaluation")]
    public class InterviewEvaluationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InterviewEvaluationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<InterviewEvaluationResponse>>> GetAll()
        {
            var result = await _mediator.Send(new InterviewEvaluationGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<InterviewEvaluationResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new InterviewEvaluationGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{interviewEvaluationId}")]
        public async Task<ActionResult<InterviewEvaluationResponse>> GetById(long interviewEvaluationId)
        {
            var result = await _mediator.Send(new InterviewEvaluationGetByIdQuery(interviewEvaluationId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] InterviewEvaluationCreateRequest request)
        {
            var id = await _mediator.Send(new InterviewEvaluationCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{interviewEvaluationId}")]
        public async Task<ActionResult> Update(long interviewEvaluationId, [FromBody] InterviewEvaluationUpdateRequest request)
        {
            await _mediator.Send(new InterviewEvaluationUpdateCommand(interviewEvaluationId, request));
            return Ok();
        }

        [HttpDelete("{interviewEvaluationId}")]
        public async Task<ActionResult> Delete(long interviewEvaluationId)
        {
            await _mediator.Send(new InterviewEvaluationDeleteCommand(interviewEvaluationId));
            return Ok();
        }
    }
}
