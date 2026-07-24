using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.EventTemplateMappings.Commands.EventTemplateMappingCreate;
using SylviaNG.Recruitment.Application.Features.EventTemplateMappings.Commands.EventTemplateMappingDelete;
using SylviaNG.Recruitment.Application.Features.EventTemplateMappings.Commands.EventTemplateMappingUpdate;
using SylviaNG.Recruitment.Application.Features.EventTemplateMappings.Models;
using SylviaNG.Recruitment.Application.Features.EventTemplateMappings.Queries.EventTemplateMappingGetAll;

namespace SylviaNG.Recruitment.Controllers
{
    [ApiController]
    [Route("recruitment/event-template-mapping")]
    [Authorize(Roles = "Admin")]
    public class EventTemplateMappingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EventTemplateMappingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<EventTemplateMappingResponse>>> GetAll()
        {
            return Ok(await _mediator.Send(new EventTemplateMappingGetAllQuery()));
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] EventTemplateMappingCreateRequest request)
        {
            return Ok(await _mediator.Send(new EventTemplateMappingCreateCommand(request)));
        }

        [HttpPut("{eventTemplateMappingId}")]
        public async Task<ActionResult> Update(long eventTemplateMappingId, [FromBody] EventTemplateMappingUpdateRequest request)
        {
            await _mediator.Send(new EventTemplateMappingUpdateCommand(eventTemplateMappingId, request));
            return Ok();
        }

        [HttpDelete("{eventTemplateMappingId}")]
        public async Task<ActionResult> Delete(long eventTemplateMappingId)
        {
            await _mediator.Send(new EventTemplateMappingDeleteCommand(eventTemplateMappingId));
            return Ok();
        }
    }
}
