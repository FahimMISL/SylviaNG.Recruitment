using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Queries.IntegrationConfigGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Commands.IntegrationConfigCreate;
using SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Commands.IntegrationConfigDelete;
using SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Commands.IntegrationConfigUpdate;
using SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Models;
using SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Queries.IntegrationConfigGetAll;
using SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Queries.IntegrationConfigGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("recruitment/integration-config")]
    public class IntegrationConfigController : ControllerBase
    {
        private readonly IMediator _mediator;

        public IntegrationConfigController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<IntegrationConfigResponse>>> GetAll()
        {
            var result = await _mediator.Send(new IntegrationConfigGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<IntegrationConfigResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new IntegrationConfigGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{integrationConfigId}")]
        public async Task<ActionResult<IntegrationConfigResponse>> GetById(long integrationConfigId)
        {
            var result = await _mediator.Send(new IntegrationConfigGetByIdQuery(integrationConfigId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] IntegrationConfigCreateRequest request)
        {
            var id = await _mediator.Send(new IntegrationConfigCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{integrationConfigId}")]
        public async Task<ActionResult> Update(long integrationConfigId, [FromBody] IntegrationConfigUpdateRequest request)
        {
            await _mediator.Send(new IntegrationConfigUpdateCommand(integrationConfigId, request));
            return Ok();
        }

        [HttpDelete("{integrationConfigId}")]
        public async Task<ActionResult> Delete(long integrationConfigId)
        {
            await _mediator.Send(new IntegrationConfigDeleteCommand(integrationConfigId));
            return Ok();
        }
    }
}
