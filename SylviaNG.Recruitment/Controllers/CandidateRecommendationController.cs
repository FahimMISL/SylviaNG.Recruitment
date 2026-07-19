using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.CandidateRecommendations.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Controllers
{
    /// <summary>HR recommends a shortlisted candidate for final selection; the Hiring Manager
    /// (the existing Admin role) reviews it (US-049).</summary>
    [ApiController]
    [Authorize(Roles = "Admin,HR")]
    public class CandidateRecommendationController : ControllerBase
    {
        private readonly ICandidateRecommendationService _candidateRecommendationService;

        public CandidateRecommendationController(ICandidateRecommendationService candidateRecommendationService)
        {
            _candidateRecommendationService = candidateRecommendationService;
        }

        /// <summary>Latest recommendation for an application, or null if none exists (AC4).</summary>
        [HttpGet("recruitment/job-application/{jobApplicationId}/recommendation")]
        public async Task<ActionResult<CandidateRecommendationResponse?>> GetLatest(long jobApplicationId)
        {
            var result = await _candidateRecommendationService.GetLatestByJobApplicationIdAsync(jobApplicationId);
            return Ok(result);
        }

        /// <summary>Recommend a candidate for final selection with a written justification (AC1/AC2).</summary>
        [HttpPost("recruitment/job-application/{jobApplicationId}/recommendation")]
        public async Task<ActionResult<long>> Create(long jobApplicationId, [FromBody] CandidateRecommendationCreateRequest request)
        {
            var id = await _candidateRecommendationService.CreateAsync(jobApplicationId, request);
            return Ok(id);
        }

        /// <summary>Hiring Manager's review queue - all Pending recommendations (AC3). Admin-only.</summary>
        [HttpGet("recruitment/candidate-recommendation/pending")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<CandidateRecommendationPendingListItemResponse>>> GetPending()
        {
            var result = await _candidateRecommendationService.GetPendingAsync();
            return Ok(result);
        }

        /// <summary>Accept or reject a recommendation with comments (AC5). Admin-only.</summary>
        [HttpPut("recruitment/candidate-recommendation/{candidateRecommendationId}/review")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Review(long candidateRecommendationId, [FromBody] CandidateRecommendationReviewRequest request)
        {
            await _candidateRecommendationService.ReviewAsync(candidateRecommendationId, request);
            return Ok();
        }
    }
}
