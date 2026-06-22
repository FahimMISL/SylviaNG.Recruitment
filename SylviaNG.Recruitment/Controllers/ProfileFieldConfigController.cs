using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Commands.ProfileFieldConfigCreate;
using SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Commands.ProfileFieldConfigUpdate;
using SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Commands.ProfileFieldConfigDelete;
using SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Models;
using SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Queries.ProfileFieldConfigGetAll;
using SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Queries.ProfileFieldConfigGetById;
using SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Queries.ProfileFieldConfigGetAllPaged;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("recruitment/profile-field-config")]
    public class ProfileFieldConfigController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProfileFieldConfigController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<ProfileFieldConfigResponse>>> GetAll()
        {
            var result = await _mediator.Send(new ProfileFieldConfigGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<ProfileFieldConfigResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new ProfileFieldConfigGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProfileFieldConfigResponse>> GetById(long id)
        {
            var result = await _mediator.Send(new ProfileFieldConfigGetByIdQuery(id));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] ProfileFieldConfigCreateRequest request)
        {
            var id = await _mediator.Send(new ProfileFieldConfigCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(long id, [FromBody] ProfileFieldConfigUpdateRequest request)
        {
            await _mediator.Send(new ProfileFieldConfigUpdateCommand(id, request));
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            await _mediator.Send(new ProfileFieldConfigDeleteCommand(id));
            return Ok();
        }
    }
}
