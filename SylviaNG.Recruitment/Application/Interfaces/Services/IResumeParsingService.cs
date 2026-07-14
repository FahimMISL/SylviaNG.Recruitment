using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>
    /// Extracts best-effort profile fields from an uploaded resume (PDF/DOCX). Free, local,
    /// regex/heuristic-based - no external AI API call.
    /// </summary>
    public interface IResumeParsingService
    {
        Task<CandidateResumeParseResponse> ParseAsync(IFormFile file);
    }
}
