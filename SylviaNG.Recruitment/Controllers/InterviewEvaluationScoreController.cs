using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Queries.InterviewEvaluationScoreGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Commands.InterviewEvaluationScoreCreate;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Commands.InterviewEvaluationScoreDelete;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Commands.InterviewEvaluationScoreUpdate;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Models;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Queries.InterviewEvaluationScoreGetAll;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Queries.InterviewEvaluationScoreGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/interview-evaluation-score")]
    public class InterviewEvaluationScoreController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InterviewEvaluationScoreController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<InterviewEvaluationScoreResponse>>> GetAll()
        {
            var result = await _mediator.Send(new InterviewEvaluationScoreGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<InterviewEvaluationScoreResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new InterviewEvaluationScoreGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{interviewEvaluationScoreId}")]
        public async Task<ActionResult<InterviewEvaluationScoreResponse>> GetById(long interviewEvaluationScoreId)
        {
            var result = await _mediator.Send(new InterviewEvaluationScoreGetByIdQuery(interviewEvaluationScoreId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] InterviewEvaluationScoreCreateRequest request)
        {
            var id = await _mediator.Send(new InterviewEvaluationScoreCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{interviewEvaluationScoreId}")]
        public async Task<ActionResult> Update(long interviewEvaluationScoreId, [FromBody] InterviewEvaluationScoreUpdateRequest request)
        {
            await _mediator.Send(new InterviewEvaluationScoreUpdateCommand(interviewEvaluationScoreId, request));
            return Ok();
        }

        [HttpDelete("{interviewEvaluationScoreId}")]
        public async Task<ActionResult> Delete(long interviewEvaluationScoreId)
        {
            await _mediator.Send(new InterviewEvaluationScoreDeleteCommand(interviewEvaluationScoreId));
            return Ok();
        }
    }
}
