using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Controllers;

[Authorize(Roles = "Admin,HR")]
[ApiController]
[Route("recruitment/ai-shortlist")]
[EnableRateLimiting("sensitive")]
public class AiShortlistController : ControllerBase
{
    private readonly ICandidateRankingService _rankingService;
    private readonly ILogger<AiShortlistController> _logger;

    public AiShortlistController(
        ICandidateRankingService rankingService,
        ILogger<AiShortlistController> logger)
    {
        _rankingService = rankingService;
        _logger = logger;
    }

    [HttpPost("rank-candidates/{jobPostingId}")]
    public async Task<IActionResult> RankCandidates(long jobPostingId)
    {
        try
        {
            var results = await _rankingService.RankCandidatesForJob(jobPostingId);
            return Ok(results);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ranking candidates for job posting {JobPostingId}", jobPostingId);
            return StatusCode(500, new { message = "An error occurred while ranking candidates." });
        }
    }
}
