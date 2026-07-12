using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.SkillLibraryGetAll;

namespace SylviaNG.Recruitment.Controllers
{
    // No class-level [Authorize] - global AuthorizeFilter already requires login, and this is a
    // read-only lookup any authenticated role (candidate filling the skills form) may hit.
    [ApiController]
    [Route("recruitment/skill-library")]
    public class SkillLibraryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SkillLibraryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<SkillLibraryItemResponse>>> GetAll()
        {
            var result = await _mediator.Send(new SkillLibraryGetAllQuery());
            return Ok(result);
        }
    }
}
