using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Commands.FinalSelectionPoolMarkHasJoined;
using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Commands.FinalSelectionPoolUpdateBatch;
using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Models;
using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Queries.FinalSelectionPoolGetAll;
using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Queries.FinalSelectionPoolGetById;

namespace SylviaNG.Recruitment.Controllers
{
    // EP-12 US-094: HR's onboarding queue - entries are created internally (OfferLetterService.
    // AcceptAsync), never via a Create endpoint here.
    [ApiController]
    [Route("recruitment/final-selection-pool")]
    [Authorize(Roles = "Admin,HR")]
    public class FinalSelectionPoolController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FinalSelectionPoolController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<FinalSelectionPoolResponse>>> GetAll()
        {
            return Ok(await _mediator.Send(new FinalSelectionPoolGetAllQuery()));
        }

        [HttpGet("{finalSelectionPoolId}")]
        public async Task<ActionResult<FinalSelectionPoolResponse>> GetById(long finalSelectionPoolId)
        {
            return Ok(await _mediator.Send(new FinalSelectionPoolGetByIdQuery(finalSelectionPoolId)));
        }

        [HttpPost("{finalSelectionPoolId}/mark-joined")]
        public async Task<ActionResult<FinalSelectionPoolResponse>> MarkHasJoined(long finalSelectionPoolId)
        {
            return Ok(await _mediator.Send(new FinalSelectionPoolMarkHasJoinedCommand(finalSelectionPoolId)));
        }

        [HttpPut("{finalSelectionPoolId}/batch")]
        public async Task<ActionResult<FinalSelectionPoolResponse>> UpdateBatch(long finalSelectionPoolId, [FromBody] FinalSelectionPoolUpdateBatchRequest request)
        {
            return Ok(await _mediator.Send(new FinalSelectionPoolUpdateBatchCommand(finalSelectionPoolId, request)));
        }
    }
}
