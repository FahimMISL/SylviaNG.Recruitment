using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Queries.JoiningBookletGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Commands.JoiningBookletCreate;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Commands.JoiningBookletDelete;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Commands.JoiningBookletUpdate;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Queries.JoiningBookletGetAll;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Queries.JoiningBookletGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/joining-booklet")]
    public class JoiningBookletController : ControllerBase
    {
        private readonly IMediator _mediator;

        public JoiningBookletController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<JoiningBookletResponse>>> GetAll()
        {
            var result = await _mediator.Send(new JoiningBookletGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<JoiningBookletResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new JoiningBookletGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{joiningBookletId}")]
        public async Task<ActionResult<JoiningBookletResponse>> GetById(long joiningBookletId)
        {
            var result = await _mediator.Send(new JoiningBookletGetByIdQuery(joiningBookletId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] JoiningBookletCreateRequest request)
        {
            var id = await _mediator.Send(new JoiningBookletCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{joiningBookletId}")]
        public async Task<ActionResult> Update(long joiningBookletId, [FromBody] JoiningBookletUpdateRequest request)
        {
            await _mediator.Send(new JoiningBookletUpdateCommand(joiningBookletId, request));
            return Ok();
        }

        [HttpDelete("{joiningBookletId}")]
        public async Task<ActionResult> Delete(long joiningBookletId)
        {
            await _mediator.Send(new JoiningBookletDeleteCommand(joiningBookletId));
            return Ok();
        }
    }
}
