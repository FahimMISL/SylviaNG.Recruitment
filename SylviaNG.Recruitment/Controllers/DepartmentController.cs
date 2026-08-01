using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.Departments.Commands.DepartmentCreate;
using SylviaNG.Recruitment.Application.Features.Departments.Commands.DepartmentDelete;
using SylviaNG.Recruitment.Application.Features.Departments.Commands.DepartmentUpdate;
using SylviaNG.Recruitment.Application.Features.Departments.Models;
using SylviaNG.Recruitment.Application.Features.Departments.Queries.DepartmentGetAll;

namespace SylviaNG.Recruitment.Controllers
{
    [ApiController]
    [Route("recruitment/department")]
    public class DepartmentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DepartmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<DepartmentResponse>>> GetAll()
        {
            var result = await _mediator.Send(new DepartmentGetAllQuery());
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<long>> Create([FromBody] DepartmentCreateRequest request)
        {
            var id = await _mediator.Send(new DepartmentCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{departmentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(long departmentId, [FromBody] DepartmentUpdateRequest request)
        {
            await _mediator.Send(new DepartmentUpdateCommand(departmentId, request));
            return Ok();
        }

        [HttpDelete("{departmentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(long departmentId)
        {
            await _mediator.Send(new DepartmentDeleteCommand(departmentId));
            return Ok();
        }
    }
}
