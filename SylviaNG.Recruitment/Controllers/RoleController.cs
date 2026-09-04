using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Common.Authorization;
using SylviaNG.Recruitment.Application.Features.Roles.Commands.RoleCreate;
using SylviaNG.Recruitment.Application.Features.Roles.Commands.RoleDelete;
using SylviaNG.Recruitment.Application.Features.Roles.Commands.RoleUpdate;
using SylviaNG.Recruitment.Application.Features.Roles.Models;
using SylviaNG.Recruitment.Application.Features.Roles.Queries.RoleGetAll;
using SylviaNG.Recruitment.Application.Features.Roles.Queries.RoleGetById;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Controllers
{
    // EP-15/US-112: custom role creation with a resource-level permission matrix
    // (Module x Action). System roles (Admin/HR/Candidate/SuperAdmin) appear here
    // read-only (IsSystemRole=true) but cannot be edited/deleted through this API.
    [ApiController]
    [Route("recruitment/roles")]
    public class RoleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RoleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [RequirePermission(AccessControlModuleEnum.Admin, PermissionActionEnum.View)]
        public async Task<ActionResult<List<RoleResponse>>> GetAll()
        {
            var result = await _mediator.Send(new RoleGetAllQuery());
            return Ok(result);
        }

        [HttpGet("{roleId}")]
        [RequirePermission(AccessControlModuleEnum.Admin, PermissionActionEnum.View)]
        public async Task<ActionResult<RoleResponse>> GetById(long roleId)
        {
            var result = await _mediator.Send(new RoleGetByIdQuery(roleId));
            return Ok(result);
        }

        [HttpPost]
        [RequirePermission(AccessControlModuleEnum.Admin, PermissionActionEnum.Create)]
        public async Task<ActionResult<long>> Create([FromBody] RoleCreateRequest request)
        {
            var id = await _mediator.Send(new RoleCreateCommand(request));
            return Ok(id);
        }

        /// <summary>Updates name and permission matrix. Permission list is a full replacement.</summary>
        [HttpPut("{roleId}")]
        [RequirePermission(AccessControlModuleEnum.Admin, PermissionActionEnum.Edit)]
        public async Task<ActionResult> Update(long roleId, [FromBody] RoleUpdateRequest request)
        {
            await _mediator.Send(new RoleUpdateCommand(roleId, request));
            return Ok();
        }

        [HttpDelete("{roleId}")]
        [RequirePermission(AccessControlModuleEnum.Admin, PermissionActionEnum.Delete)]
        public async Task<ActionResult> Delete(long roleId)
        {
            await _mediator.Send(new RoleDeleteCommand(roleId));
            return Ok();
        }
    }
}
