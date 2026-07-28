using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>
    /// Extracts best-effort profile fields from an uploaded resume (PDF/DOCX). Two providers
    /// exist - "Heuristic" (free, local, regex-based) and "Ai" (Groq-backed) - swappable via
    /// ResumeParsing:Provider, same pattern as IShortlistScoringService.
    /// </summary>
    public interface IResumeParsingService
    {
        string ProviderName { get; }

        Task<CandidateResumeParseResponse> ParseAsync(IFormFile file);

        /// <summary>Raw text only (no field parsing) - used to persist CV content for CV Bank search (US-045).</summary>
        Task<string> ExtractRawTextAsync(IFormFile file);
    }
}
