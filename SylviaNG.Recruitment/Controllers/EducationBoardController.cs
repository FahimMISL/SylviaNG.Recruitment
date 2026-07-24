using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.EducationBoards.Commands.EducationBoardCreate;
using SylviaNG.Recruitment.Application.Features.EducationBoards.Commands.EducationBoardDelete;
using SylviaNG.Recruitment.Application.Features.EducationBoards.Commands.EducationBoardUpdate;
using SylviaNG.Recruitment.Application.Features.EducationBoards.Models;
using SylviaNG.Recruitment.Application.Features.EducationBoards.Queries.EducationBoardGetAll;

namespace SylviaNG.Recruitment.Controllers
{
    // GetAll has no [Authorize] - global AuthorizeFilter already requires login, and any
    // authenticated role (candidate filling the education form's Board dropdown) needs this
    // lookup. Writes are Admin-only (System Administration / master data management).
    [ApiController]
    [Route("recruitment/education-board")]
    public class EducationBoardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EducationBoardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<EducationBoardResponse>>> GetAll()
        {
            var result = await _mediator.Send(new EducationBoardGetAllQuery());
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<long>> Create([FromBody] EducationBoardCreateRequest request)
        {
            var id = await _mediator.Send(new EducationBoardCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{educationBoardId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(long educationBoardId, [FromBody] EducationBoardUpdateRequest request)
        {
            await _mediator.Send(new EducationBoardUpdateCommand(educationBoardId, request));
            return Ok();
        }

        [HttpDelete("{educationBoardId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(long educationBoardId)
        {
            await _mediator.Send(new EducationBoardDeleteCommand(educationBoardId));
            return Ok();
        }
    }
}
