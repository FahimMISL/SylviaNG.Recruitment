using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.QuestionGroups.Commands.QuestionGroupCreate;
using SylviaNG.Recruitment.Application.Features.QuestionGroups.Commands.QuestionGroupSetActiveStatus;
using SylviaNG.Recruitment.Application.Features.QuestionGroups.Commands.QuestionGroupUpdate;
using SylviaNG.Recruitment.Application.Features.QuestionGroups.Models;
using SylviaNG.Recruitment.Application.Features.QuestionGroups.Queries.QuestionGroupGetActiveLookup;
using SylviaNG.Recruitment.Application.Features.QuestionGroups.Queries.QuestionGroupGetAll;
using SylviaNG.Recruitment.Application.Features.QuestionGroups.Queries.QuestionGroupGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [ApiController]
    [Route("recruitment/question-group")]
    [Authorize(Roles = "Admin,HR")]
    public class QuestionGroupController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuestionGroupController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Get all question groups.</summary>
        [HttpGet]
        public async Task<ActionResult<List<QuestionGroupResponse>>> GetAll()
        {
            var result = await _mediator.Send(new QuestionGroupGetAllQuery());
            return Ok(result);
        }

        /// <summary>Active groups only (Id + Name), for a "pick a question group" dropdown.</summary>
        [HttpGet("lookup")]
        public async Task<ActionResult<List<QuestionGroupLookupResponse>>> GetActiveLookup()
        {
            var result = await _mediator.Send(new QuestionGroupGetActiveLookupQuery());
            return Ok(result);
        }

        /// <summary>Get a question group by ID.</summary>
        [HttpGet("{questionGroupId}")]
        public async Task<ActionResult<QuestionGroupResponse>> GetById(long questionGroupId)
        {
            var result = await _mediator.Send(new QuestionGroupGetByIdQuery(questionGroupId));
            return Ok(result);
        }

        /// <summary>Create a new question group.</summary>
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] QuestionGroupCreateRequest request)
        {
            var id = await _mediator.Send(new QuestionGroupCreateCommand(request));
            return Ok(id);
        }

        /// <summary>Update a question group.</summary>
        [HttpPut("{questionGroupId}")]
        public async Task<ActionResult> Update(long questionGroupId, [FromBody] QuestionGroupUpdateRequest request)
        {
            await _mediator.Send(new QuestionGroupUpdateCommand(questionGroupId, request));
            return Ok();
        }

        /// <summary>Activate or deactivate a question group (US-053 AC5).</summary>
        [HttpPatch("{questionGroupId}/active-status")]
        public async Task<ActionResult> SetActiveStatus(long questionGroupId, [FromBody] QuestionGroupSetActiveStatusRequest request)
        {
            await _mediator.Send(new QuestionGroupSetActiveStatusCommand(questionGroupId, request.IsActive));
            return Ok();
        }
    }
}
