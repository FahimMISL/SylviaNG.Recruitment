using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Commands.ExamHallCreate;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Commands.ExamHallSetActiveStatus;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Commands.ExamHallUpdate;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Models;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Queries.ExamHallGetActiveLookup;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Queries.ExamHallGetAll;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Queries.ExamHallGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [ApiController]
    [Route("recruitment/exam-hall")]
    [Authorize(Roles = "Admin,HR")]
    public class ExamHallController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExamHallController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Get all exam halls.</summary>
        [HttpGet]
        public async Task<ActionResult<List<ExamHallResponse>>> GetAll()
        {
            var result = await _mediator.Send(new ExamHallGetAllQuery());
            return Ok(result);
        }

        /// <summary>Active halls only (Id + Name), for a "pick an exam hall" dropdown.</summary>
        [HttpGet("lookup")]
        public async Task<ActionResult<List<ExamHallLookupResponse>>> GetActiveLookup()
        {
            var result = await _mediator.Send(new ExamHallGetActiveLookupQuery());
            return Ok(result);
        }

        /// <summary>Get an exam hall by ID.</summary>
        [HttpGet("{examHallId}")]
        public async Task<ActionResult<ExamHallResponse>> GetById(long examHallId)
        {
            var result = await _mediator.Send(new ExamHallGetByIdQuery(examHallId));
            return Ok(result);
        }

        /// <summary>Create a new exam hall.</summary>
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] ExamHallCreateRequest request)
        {
            var id = await _mediator.Send(new ExamHallCreateCommand(request));
            return Ok(id);
        }

        /// <summary>Update an exam hall.</summary>
        [HttpPut("{examHallId}")]
        public async Task<ActionResult> Update(long examHallId, [FromBody] ExamHallUpdateRequest request)
        {
            await _mediator.Send(new ExamHallUpdateCommand(examHallId, request));
            return Ok();
        }

        /// <summary>Activate or deactivate an exam hall (US-062 AC2).</summary>
        [HttpPatch("{examHallId}/active-status")]
        public async Task<ActionResult> SetActiveStatus(long examHallId, [FromBody] ExamHallSetActiveStatusRequest request)
        {
            await _mediator.Send(new ExamHallSetActiveStatusCommand(examHallId, request.IsActive));
            return Ok();
        }
    }
}
