using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.Questions.Queries.QuestionGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.Questions.Commands.QuestionCreate;
using SylviaNG.Recruitment.Application.Features.Questions.Commands.QuestionDelete;
using SylviaNG.Recruitment.Application.Features.Questions.Commands.QuestionUpdate;
using SylviaNG.Recruitment.Application.Features.Questions.Models;
using SylviaNG.Recruitment.Application.Features.Questions.Queries.QuestionGetAll;
using SylviaNG.Recruitment.Application.Features.Questions.Queries.QuestionGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/question")]
    public class QuestionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuestionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<QuestionResponse>>> GetAll()
        {
            var result = await _mediator.Send(new QuestionGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<QuestionResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new QuestionGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{questionId}")]
        public async Task<ActionResult<QuestionResponse>> GetById(long questionId)
        {
            var result = await _mediator.Send(new QuestionGetByIdQuery(questionId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] QuestionCreateRequest request)
        {
            var id = await _mediator.Send(new QuestionCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{questionId}")]
        public async Task<ActionResult> Update(long questionId, [FromBody] QuestionUpdateRequest request)
        {
            await _mediator.Send(new QuestionUpdateCommand(questionId, request));
            return Ok();
        }

        [HttpDelete("{questionId}")]
        public async Task<ActionResult> Delete(long questionId)
        {
            await _mediator.Send(new QuestionDeleteCommand(questionId));
            return Ok();
        }
    }
}
