using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ExamAnswers.Queries.ExamAnswerGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.ExamAnswers.Commands.ExamAnswerCreate;
using SylviaNG.Recruitment.Application.Features.ExamAnswers.Commands.ExamAnswerDelete;
using SylviaNG.Recruitment.Application.Features.ExamAnswers.Commands.ExamAnswerUpdate;
using SylviaNG.Recruitment.Application.Features.ExamAnswers.Models;
using SylviaNG.Recruitment.Application.Features.ExamAnswers.Queries.ExamAnswerGetAll;
using SylviaNG.Recruitment.Application.Features.ExamAnswers.Queries.ExamAnswerGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/exam-answer")]
    public class ExamAnswerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExamAnswerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<ExamAnswerResponse>>> GetAll()
        {
            var result = await _mediator.Send(new ExamAnswerGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<ExamAnswerResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new ExamAnswerGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{examAnswerId}")]
        public async Task<ActionResult<ExamAnswerResponse>> GetById(long examAnswerId)
        {
            var result = await _mediator.Send(new ExamAnswerGetByIdQuery(examAnswerId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] ExamAnswerCreateRequest request)
        {
            var id = await _mediator.Send(new ExamAnswerCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{examAnswerId}")]
        public async Task<ActionResult> Update(long examAnswerId, [FromBody] ExamAnswerUpdateRequest request)
        {
            await _mediator.Send(new ExamAnswerUpdateCommand(examAnswerId, request));
            return Ok();
        }

        [HttpDelete("{examAnswerId}")]
        public async Task<ActionResult> Delete(long examAnswerId)
        {
            await _mediator.Send(new ExamAnswerDeleteCommand(examAnswerId));
            return Ok();
        }
    }
}
