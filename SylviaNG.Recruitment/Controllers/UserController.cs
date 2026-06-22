using Microsoft.AspNetCore.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.Users.Commands.UserCreate;
using SylviaNG.Recruitment.Application.Features.Users.Commands.UserDelete;
using SylviaNG.Recruitment.Application.Features.Users.Commands.UserUpdate;
using SylviaNG.Recruitment.Application.Features.Users.Models;
using SylviaNG.Recruitment.Application.Features.Users.Queries.UserGetAll;
using SylviaNG.Recruitment.Application.Features.Users.Queries.UserGetById;
using SylviaNG.Recruitment.Application.Features.Users.Queries.UserGetAllPaged;
using SylviaNG.Recruitment.Application.Features.Roles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("recruitment/user")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _userService;

        public UserController(IMediator mediator, IUserService userService)
        {
            _mediator = mediator;
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserResponse>>> GetAll()
        {
            var result = await _mediator.Send(new UserGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<UserResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new UserGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserResponse>> GetById(long userId)
        {
            var result = await _mediator.Send(new UserGetByIdQuery(userId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] UserCreateRequest request)
        {
            var id = await _mediator.Send(new UserCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult> Update(long userId, [FromBody] UserUpdateRequest request)
        {
            await _mediator.Send(new UserUpdateCommand(userId, request));
            return Ok();
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> Delete(long userId)
        {
            await _mediator.Send(new UserDeleteCommand(userId));
            return Ok();
        }

        [HttpPost("{userId}/roles")]
        public async Task<ActionResult> AssignRole(long userId, [FromBody] UserRoleAssignRequest request)
        {
            await _userService.AssignRoleAsync(userId, request);
            return Ok();
        }

        [HttpDelete("{userId}/roles/{roleId}")]
        public async Task<ActionResult> RemoveRole(long userId, long roleId)
        {
            await _userService.RemoveRoleAsync(userId, roleId);
            return Ok();
        }
    }
}
