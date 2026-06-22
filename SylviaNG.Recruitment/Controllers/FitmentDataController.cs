using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.FitmentDatas.Queries.FitmentDataGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.FitmentDatas.Commands.FitmentDataCreate;
using SylviaNG.Recruitment.Application.Features.FitmentDatas.Commands.FitmentDataDelete;
using SylviaNG.Recruitment.Application.Features.FitmentDatas.Commands.FitmentDataUpdate;
using SylviaNG.Recruitment.Application.Features.FitmentDatas.Models;
using SylviaNG.Recruitment.Application.Features.FitmentDatas.Queries.FitmentDataGetAll;
using SylviaNG.Recruitment.Application.Features.FitmentDatas.Queries.FitmentDataGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/fitment-data")]
    public class FitmentDataController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FitmentDataController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<FitmentDataResponse>>> GetAll()
        {
            var result = await _mediator.Send(new FitmentDataGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<FitmentDataResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new FitmentDataGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{fitmentDataId}")]
        public async Task<ActionResult<FitmentDataResponse>> GetById(long fitmentDataId)
        {
            var result = await _mediator.Send(new FitmentDataGetByIdQuery(fitmentDataId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] FitmentDataCreateRequest request)
        {
            var id = await _mediator.Send(new FitmentDataCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{fitmentDataId}")]
        public async Task<ActionResult> Update(long fitmentDataId, [FromBody] FitmentDataUpdateRequest request)
        {
            await _mediator.Send(new FitmentDataUpdateCommand(fitmentDataId, request));
            return Ok();
        }

        [HttpDelete("{fitmentDataId}")]
        public async Task<ActionResult> Delete(long fitmentDataId)
        {
            await _mediator.Send(new FitmentDataDeleteCommand(fitmentDataId));
            return Ok();
        }
    }
}
