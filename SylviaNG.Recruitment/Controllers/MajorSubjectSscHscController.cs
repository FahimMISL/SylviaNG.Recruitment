using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.MajorSubjectsSscHsc.Commands.MajorSubjectSscHscCreate;
using SylviaNG.Recruitment.Application.Features.MajorSubjectsSscHsc.Commands.MajorSubjectSscHscDelete;
using SylviaNG.Recruitment.Application.Features.MajorSubjectsSscHsc.Commands.MajorSubjectSscHscUpdate;
using SylviaNG.Recruitment.Application.Features.MajorSubjectsSscHsc.Models;
using SylviaNG.Recruitment.Application.Features.MajorSubjectsSscHsc.Queries.MajorSubjectSscHscGetAll;

namespace SylviaNG.Recruitment.Controllers
{
    // GetAll has no [Authorize] - global AuthorizeFilter already requires login, and any
    // authenticated role (candidate filling the education form) needs this lookup. Writes
    // are Admin-only (System Administration / master data management).
    [ApiController]
    [Route("recruitment/major-subject-ssc-hsc")]
    public class MajorSubjectSscHscController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MajorSubjectSscHscController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<MajorSubjectSscHscResponse>>> GetAll()
        {
            var result = await _mediator.Send(new MajorSubjectSscHscGetAllQuery());
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<long>> Create([FromBody] MajorSubjectSscHscCreateRequest request)
        {
            var id = await _mediator.Send(new MajorSubjectSscHscCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{majorSubjectSscHscId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(long majorSubjectSscHscId, [FromBody] MajorSubjectSscHscUpdateRequest request)
        {
            await _mediator.Send(new MajorSubjectSscHscUpdateCommand(majorSubjectSscHscId, request));
            return Ok();
        }

        [HttpDelete("{majorSubjectSscHscId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(long majorSubjectSscHscId)
        {
            await _mediator.Send(new MajorSubjectSscHscDeleteCommand(majorSubjectSscHscId));
            return Ok();
        }
    }
}
