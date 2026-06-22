using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Queries.RequisitionStageConfigGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Commands.RequisitionStageConfigCreate;
using SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Commands.RequisitionStageConfigDelete;
using SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Commands.RequisitionStageConfigUpdate;
using SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Models;
using SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Queries.RequisitionStageConfigGetAll;
using SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Queries.RequisitionStageConfigGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/requisition-stage-config")]
    public class RequisitionStageConfigController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RequisitionStageConfigController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<RequisitionStageConfigResponse>>> GetAll()
        {
            var result = await _mediator.Send(new RequisitionStageConfigGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<RequisitionStageConfigResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new RequisitionStageConfigGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{requisitionStageConfigId}")]
        public async Task<ActionResult<RequisitionStageConfigResponse>> GetById(long requisitionStageConfigId)
        {
            var result = await _mediator.Send(new RequisitionStageConfigGetByIdQuery(requisitionStageConfigId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] RequisitionStageConfigCreateRequest request)
        {
            var id = await _mediator.Send(new RequisitionStageConfigCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{requisitionStageConfigId}")]
        public async Task<ActionResult> Update(long requisitionStageConfigId, [FromBody] RequisitionStageConfigUpdateRequest request)
        {
            await _mediator.Send(new RequisitionStageConfigUpdateCommand(requisitionStageConfigId, request));
            return Ok();
        }

        [HttpDelete("{requisitionStageConfigId}")]
        public async Task<ActionResult> Delete(long requisitionStageConfigId)
        {
            await _mediator.Send(new RequisitionStageConfigDeleteCommand(requisitionStageConfigId));
            return Ok();
        }
    }
}
