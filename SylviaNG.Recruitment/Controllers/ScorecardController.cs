using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.Scorecards.Commands.ScorecardCreate;
using SylviaNG.Recruitment.Application.Features.Scorecards.Commands.ScorecardSetActiveStatus;
using SylviaNG.Recruitment.Application.Features.Scorecards.Commands.ScorecardUpdate;
using SylviaNG.Recruitment.Application.Features.Scorecards.Models;
using SylviaNG.Recruitment.Application.Features.Scorecards.Queries.ScorecardGetActiveLookup;
using SylviaNG.Recruitment.Application.Features.Scorecards.Queries.ScorecardGetAll;
using SylviaNG.Recruitment.Application.Features.Scorecards.Queries.ScorecardGetById;

namespace SylviaNG.Recruitment.Controllers
{
    /// <summary>Weighted-criteria scorecard templates for interview evaluation (EP-08 US-066).</summary>
    [ApiController]
    [Route("recruitment/scorecard")]
    [Authorize(Roles = "Admin,HR")]
    public class ScorecardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ScorecardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Get all scorecards.</summary>
        [HttpGet]
        public async Task<ActionResult<List<ScorecardResponse>>> GetAll()
        {
            var result = await _mediator.Send(new ScorecardGetAllQuery());
            return Ok(result);
        }

        /// <summary>Active scorecards only (Id + Name), for a "pick a scorecard" dropdown.</summary>
        [HttpGet("lookup")]
        public async Task<ActionResult<List<ScorecardLookupResponse>>> GetActiveLookup()
        {
            var result = await _mediator.Send(new ScorecardGetActiveLookupQuery());
            return Ok(result);
        }

        /// <summary>Get a scorecard by ID.</summary>
        [HttpGet("{scorecardId}")]
        public async Task<ActionResult<ScorecardResponse>> GetById(long scorecardId)
        {
            var result = await _mediator.Send(new ScorecardGetByIdQuery(scorecardId));
            return Ok(result);
        }

        /// <summary>Create a new scorecard with its weighted criteria.</summary>
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] ScorecardCreateRequest request)
        {
            var id = await _mediator.Send(new ScorecardCreateCommand(request));
            return Ok(id);
        }

        /// <summary>Update a scorecard, replacing its criteria wholesale.</summary>
        [HttpPut("{scorecardId}")]
        public async Task<ActionResult> Update(long scorecardId, [FromBody] ScorecardUpdateRequest request)
        {
            await _mediator.Send(new ScorecardUpdateCommand(scorecardId, request));
            return Ok();
        }

        /// <summary>Activate or deactivate a scorecard.</summary>
        [HttpPatch("{scorecardId}/active-status")]
        public async Task<ActionResult> SetActiveStatus(long scorecardId, [FromBody] ScorecardSetActiveStatusRequest request)
        {
            await _mediator.Send(new ScorecardSetActiveStatusCommand(scorecardId, request.IsActive));
            return Ok();
        }
    }
}
