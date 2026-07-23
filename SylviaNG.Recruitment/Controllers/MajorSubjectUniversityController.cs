using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.MajorSubjectsUniversity.Commands.MajorSubjectUniversityCreate;
using SylviaNG.Recruitment.Application.Features.MajorSubjectsUniversity.Commands.MajorSubjectUniversityDelete;
using SylviaNG.Recruitment.Application.Features.MajorSubjectsUniversity.Commands.MajorSubjectUniversityUpdate;
using SylviaNG.Recruitment.Application.Features.MajorSubjectsUniversity.Models;
using SylviaNG.Recruitment.Application.Features.MajorSubjectsUniversity.Queries.MajorSubjectUniversityGetAll;

namespace SylviaNG.Recruitment.Controllers
{
    // GetAll has no [Authorize] - global AuthorizeFilter already requires login, and any
    // authenticated role (candidate filling the education form) needs this lookup. Writes
    // are Admin-only (System Administration / master data management).
    [ApiController]
    [Route("recruitment/major-subject-university")]
    public class MajorSubjectUniversityController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MajorSubjectUniversityController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<MajorSubjectUniversityResponse>>> GetAll()
        {
            var result = await _mediator.Send(new MajorSubjectUniversityGetAllQuery());
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<long>> Create([FromBody] MajorSubjectUniversityCreateRequest request)
        {
            var id = await _mediator.Send(new MajorSubjectUniversityCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{majorSubjectUniversityId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(long majorSubjectUniversityId, [FromBody] MajorSubjectUniversityUpdateRequest request)
        {
            await _mediator.Send(new MajorSubjectUniversityUpdateCommand(majorSubjectUniversityId, request));
            return Ok();
        }

        [HttpDelete("{majorSubjectUniversityId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(long majorSubjectUniversityId)
        {
            await _mediator.Send(new MajorSubjectUniversityDeleteCommand(majorSubjectUniversityId));
            return Ok();
        }
    }
}
