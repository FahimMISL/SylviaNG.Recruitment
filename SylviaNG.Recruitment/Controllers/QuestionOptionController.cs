using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.QuestionOptions.Queries.QuestionOptionGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.QuestionOptions.Commands.QuestionOptionCreate;
using SylviaNG.Recruitment.Application.Features.QuestionOptions.Commands.QuestionOptionDelete;
using SylviaNG.Recruitment.Application.Features.QuestionOptions.Commands.QuestionOptionUpdate;
using SylviaNG.Recruitment.Application.Features.QuestionOptions.Models;
using SylviaNG.Recruitment.Application.Features.QuestionOptions.Queries.QuestionOptionGetAll;
using SylviaNG.Recruitment.Application.Features.QuestionOptions.Queries.QuestionOptionGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/question-option")]
    public class QuestionOptionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuestionOptionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<QuestionOptionResponse>>> GetAll()
        {
            var result = await _mediator.Send(new QuestionOptionGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<QuestionOptionResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new QuestionOptionGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{questionOptionId}")]
        public async Task<ActionResult<QuestionOptionResponse>> GetById(long questionOptionId)
        {
            var result = await _mediator.Send(new QuestionOptionGetByIdQuery(questionOptionId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] QuestionOptionCreateRequest request)
        {
            var id = await _mediator.Send(new QuestionOptionCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{questionOptionId}")]
        public async Task<ActionResult> Update(long questionOptionId, [FromBody] QuestionOptionUpdateRequest request)
        {
            await _mediator.Send(new QuestionOptionUpdateCommand(questionOptionId, request));
            return Ok();
        }

        [HttpDelete("{questionOptionId}")]
        public async Task<ActionResult> Delete(long questionOptionId)
        {
            await _mediator.Send(new QuestionOptionDeleteCommand(questionOptionId));
            return Ok();
        }
    }
}
