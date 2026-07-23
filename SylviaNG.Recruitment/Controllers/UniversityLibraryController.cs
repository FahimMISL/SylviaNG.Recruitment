using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.UniversityLibraryCreate;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.UniversityLibraryDelete;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.UniversityLibraryUpdate;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.UniversityLibraryGetAll;

namespace SylviaNG.Recruitment.Controllers
{
    // GetAll has no [Authorize] - global AuthorizeFilter already requires login, and any
    // authenticated role (candidate filling the education form's University typeahead) needs
    // this lookup. Writes are Admin-only (System Administration / master data management).
    [ApiController]
    [Route("recruitment/university-library")]
    public class UniversityLibraryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UniversityLibraryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<UniversityLibraryItemResponse>>> GetAll()
        {
            var result = await _mediator.Send(new UniversityLibraryGetAllQuery());
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<long>> Create([FromBody] UniversityLibraryItemCreateRequest request)
        {
            var id = await _mediator.Send(new UniversityLibraryCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{universityLibraryItemId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(long universityLibraryItemId, [FromBody] UniversityLibraryItemUpdateRequest request)
        {
            await _mediator.Send(new UniversityLibraryUpdateCommand(universityLibraryItemId, request));
            return Ok();
        }

        [HttpDelete("{universityLibraryItemId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(long universityLibraryItemId)
        {
            await _mediator.Send(new UniversityLibraryDeleteCommand(universityLibraryItemId));
            return Ok();
        }
    }
}
