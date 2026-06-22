using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Queries.FinalSelectionPoolGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Commands.FinalSelectionPoolCreate;
using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Commands.FinalSelectionPoolDelete;
using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Commands.FinalSelectionPoolUpdate;
using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Models;
using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Queries.FinalSelectionPoolGetAll;
using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Queries.FinalSelectionPoolGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/final-selection-pool")]
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
            var result = await _mediator.Send(new FinalSelectionPoolGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<FinalSelectionPoolResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new FinalSelectionPoolGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{finalSelectionPoolId}")]
        public async Task<ActionResult<FinalSelectionPoolResponse>> GetById(long finalSelectionPoolId)
        {
            var result = await _mediator.Send(new FinalSelectionPoolGetByIdQuery(finalSelectionPoolId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] FinalSelectionPoolCreateRequest request)
        {
            var id = await _mediator.Send(new FinalSelectionPoolCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{finalSelectionPoolId}")]
        public async Task<ActionResult> Update(long finalSelectionPoolId, [FromBody] FinalSelectionPoolUpdateRequest request)
        {
            await _mediator.Send(new FinalSelectionPoolUpdateCommand(finalSelectionPoolId, request));
            return Ok();
        }

        [HttpDelete("{finalSelectionPoolId}")]
        public async Task<ActionResult> Delete(long finalSelectionPoolId)
        {
            await _mediator.Send(new FinalSelectionPoolDeleteCommand(finalSelectionPoolId));
            return Ok();
        }
    }
}
