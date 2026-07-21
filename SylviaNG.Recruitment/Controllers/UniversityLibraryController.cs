using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.UniversityLibraryGetAll;

namespace SylviaNG.Recruitment.Controllers
{
    // No class-level [Authorize] - global AuthorizeFilter already requires login, and this is a
    // read-only lookup any authenticated role (candidate filling the education form) may hit.
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
    }
}
