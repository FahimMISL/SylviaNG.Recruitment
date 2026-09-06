using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Common.Authorization;
using SylviaNG.Recruitment.Application.Features.UserAccounts.Commands.UserAccountCreate;
using SylviaNG.Recruitment.Application.Features.UserAccounts.Commands.UserAccountSetActive;
using SylviaNG.Recruitment.Application.Features.UserAccounts.Commands.UserAccountUpdate;
using SylviaNG.Recruitment.Application.Features.UserAccounts.Models;
using SylviaNG.Recruitment.Application.Features.UserAccounts.Queries.UserAccountGetAll;
using SylviaNG.Recruitment.Application.Features.UserAccounts.Queries.UserAccountGetById;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Controllers
{
    // EP-15/US-111: HR user CRUD with Keycloak invite + multi-role assign. Gated per-action via
    // RequirePermission (Admin module) rather than [Authorize(Roles="Admin")] - Admin/SuperAdmin
    // always pass (superuser bypass), but a custom role explicitly granted Admin/View etc. can
    // also reach these actions, which a fixed role-name attribute couldn't express.
    [ApiController]
    [Route("recruitment/user-accounts")]
    public class UserAccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserAccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [RequirePermission(AccessControlModuleEnum.Admin, PermissionActionEnum.View)]
        public async Task<ActionResult<List<UserAccountResponse>>> GetAll()
        {
            var result = await _mediator.Send(new UserAccountGetAllQuery());
            return Ok(result);
        }

        [HttpGet("{userAccountId}")]
        [RequirePermission(AccessControlModuleEnum.Admin, PermissionActionEnum.View)]
        public async Task<ActionResult<UserAccountResponse>> GetById(long userAccountId)
        {
            var result = await _mediator.Send(new UserAccountGetByIdQuery(userAccountId));
            return Ok(result);
        }

        /// <summary>Invites a new HR/Admin/SuperAdmin/custom-role user: creates the
        /// Keycloak realm user and assigns all selected roles, then mirrors it locally.</summary>
        [HttpPost]
        [RequirePermission(AccessControlModuleEnum.Admin, PermissionActionEnum.Create)]
        public async Task<ActionResult<long>> Create([FromBody] UserAccountCreateRequest request)
        {
            var id = await _mediator.Send(new UserAccountCreateCommand(request));
            return Ok(id);
        }

        /// <summary>Updates name and role assignment. Role list is a full replacement.</summary>
        [HttpPut("{userAccountId}")]
        [RequirePermission(AccessControlModuleEnum.Admin, PermissionActionEnum.Edit)]
        public async Task<ActionResult> Update(long userAccountId, [FromBody] UserAccountUpdateRequest request)
        {
            await _mediator.Send(new UserAccountUpdateCommand(userAccountId, request));
            return Ok();
        }

        [HttpPatch("{userAccountId}/active")]
        [RequirePermission(AccessControlModuleEnum.Admin, PermissionActionEnum.Edit)]
        public async Task<ActionResult> SetActive(long userAccountId, [FromQuery] bool isActive)
        {
            await _mediator.Send(new UserAccountSetActiveCommand(userAccountId, isActive));
            return Ok();
        }
    }
}
