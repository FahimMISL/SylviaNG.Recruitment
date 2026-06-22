using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.EmergencyContacts.Queries.EmergencyContactGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.EmergencyContacts.Commands.EmergencyContactCreate;
using SylviaNG.Recruitment.Application.Features.EmergencyContacts.Commands.EmergencyContactDelete;
using SylviaNG.Recruitment.Application.Features.EmergencyContacts.Commands.EmergencyContactUpdate;
using SylviaNG.Recruitment.Application.Features.EmergencyContacts.Models;
using SylviaNG.Recruitment.Application.Features.EmergencyContacts.Queries.EmergencyContactGetAll;
using SylviaNG.Recruitment.Application.Features.EmergencyContacts.Queries.EmergencyContactGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/emergency-contact")]
    public class EmergencyContactController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmergencyContactController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<EmergencyContactResponse>>> GetAll()
        {
            var result = await _mediator.Send(new EmergencyContactGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<EmergencyContactResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new EmergencyContactGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{emergencyContactId}")]
        public async Task<ActionResult<EmergencyContactResponse>> GetById(long emergencyContactId)
        {
            var result = await _mediator.Send(new EmergencyContactGetByIdQuery(emergencyContactId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] EmergencyContactCreateRequest request)
        {
            var id = await _mediator.Send(new EmergencyContactCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{emergencyContactId}")]
        public async Task<ActionResult> Update(long emergencyContactId, [FromBody] EmergencyContactUpdateRequest request)
        {
            await _mediator.Send(new EmergencyContactUpdateCommand(emergencyContactId, request));
            return Ok();
        }

        [HttpDelete("{emergencyContactId}")]
        public async Task<ActionResult> Delete(long emergencyContactId)
        {
            await _mediator.Send(new EmergencyContactDeleteCommand(emergencyContactId));
            return Ok();
        }
    }
}
