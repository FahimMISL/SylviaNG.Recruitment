using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.Dashboard.Models;
using SylviaNG.Recruitment.Application.Features.Dashboard.Queries.DashboardSummaryGet;

namespace SylviaNG.Recruitment.Controllers
{
    // No class-level [Authorize] - global AuthorizeFilter already requires login. Shared by all
    // 3 roles; response shape differs per-role (see DashboardService.GetSummaryAsync).
    [ApiController]
    [Route("recruitment/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<DashboardSummaryResponse>> GetSummary()
        {
            var result = await _mediator.Send(new DashboardSummaryGetQuery());
            return Ok(result);
        }
    }
}
