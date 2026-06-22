using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.QuestionGroups.Queries.QuestionGroupGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.QuestionGroups.Commands.QuestionGroupCreate;
using SylviaNG.Recruitment.Application.Features.QuestionGroups.Commands.QuestionGroupDelete;
using SylviaNG.Recruitment.Application.Features.QuestionGroups.Commands.QuestionGroupUpdate;
using SylviaNG.Recruitment.Application.Features.QuestionGroups.Models;
using SylviaNG.Recruitment.Application.Features.QuestionGroups.Queries.QuestionGroupGetAll;
using SylviaNG.Recruitment.Application.Features.QuestionGroups.Queries.QuestionGroupGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/question-group")]
    public class QuestionGroupController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuestionGroupController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<QuestionGroupResponse>>> GetAll()
        {
            var result = await _mediator.Send(new QuestionGroupGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<QuestionGroupResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new QuestionGroupGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{questionGroupId}")]
        public async Task<ActionResult<QuestionGroupResponse>> GetById(long questionGroupId)
        {
            var result = await _mediator.Send(new QuestionGroupGetByIdQuery(questionGroupId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] QuestionGroupCreateRequest request)
        {
            var id = await _mediator.Send(new QuestionGroupCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{questionGroupId}")]
        public async Task<ActionResult> Update(long questionGroupId, [FromBody] QuestionGroupUpdateRequest request)
        {
            await _mediator.Send(new QuestionGroupUpdateCommand(questionGroupId, request));
            return Ok();
        }

        [HttpDelete("{questionGroupId}")]
        public async Task<ActionResult> Delete(long questionGroupId)
        {
            await _mediator.Send(new QuestionGroupDeleteCommand(questionGroupId));
            return Ok();
        }
    }
}
