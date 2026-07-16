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

        /// <summary>Raw text only (no field parsing) - used to persist CV content for CV Bank search (US-045).</summary>
        Task<string> ExtractRawTextAsync(IFormFile file);
    }
}
