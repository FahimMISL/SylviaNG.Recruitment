using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using System.Text.Json;

namespace SylviaNG.Recruitment.Application.Services
{
    /// <summary>
    /// AI-backed candidate scorer (US-046 "Ai" provider) - one Groq call per candidate (isolates
    /// failures to a single candidate, keeps the JSON response trivial to validate). Never throws
    /// out of ScoreAsync for a per-candidate failure - a bad Groq call must not take down the run.
    /// </summary>
    public class AiShortlistScoringService : IShortlistScoringService
    {
        private const string SystemPrompt =
            "You are an expert technical recruiter assistant. Evaluate how well a candidate matches " +
            "a job posting using ONLY the facts provided. Respond with a single JSON object of exactly " +
            "this shape: {\"score\": <integer 0-100>, \"explanation\": \"<1-3 sentence justification>\"}. " +
            "No other text, no markdown.";

        private readonly IGroqClient _groqClient;
        private readonly ILogger<AiShortlistScoringService> _logger;

        public AiShortlistScoringService(IGroqClient groqClient, ILogger<AiShortlistScoringService> logger)
        {
            _groqClient = groqClient;
            _logger = logger;
        }

        public string ProviderName => "Ai";

        public async Task<CandidateScoringResult> ScoreAsync(JobPosting jobPosting, CandidateFactService.CandidateFacts facts, CancellationToken ct = default)
        {
            var userPrompt = BuildUserPrompt(jobPosting, facts);

            string content;
            try
            {
                content = await _groqClient.GetJsonCompletionAsync(SystemPrompt, userPrompt, ct);
            }
            catch (GroqUnavailableException ex)
            {
                _logger.LogWarning(ex, "Groq scoring call failed for JobPostingId {JobPostingId}.", jobPosting.JobPostingId);
                return new CandidateScoringResult(null, null, true, ex.Message);
            }

            try
            {
                using var json = JsonDocument.Parse(content);
                var score = json.RootElement.GetProperty("score").GetInt32();
                var explanation = json.RootElement.GetProperty("explanation").GetString() ?? string.Empty;

                var clamped = Math.Clamp(score, 0, 100);
                return new CandidateScoringResult(clamped, explanation, false, null);
            }
            catch (Exception ex)
            {
                // Deliberately broad: any unexpected response shape must degrade to a failed
                // result for this one candidate, never throw out of ScoreAsync (see class doc).
                _logger.LogWarning(ex, "Groq returned an unparseable response: {Content}", content);
                return new CandidateScoringResult(null, null, true, "Groq returned an unparseable response.");
            }
        }

        private static string BuildUserPrompt(JobPosting jobPosting, CandidateFactService.CandidateFacts facts)
        {
            return $"""
                Job Posting:
                Title: {jobPosting.Title}
                Description: {jobPosting.Description ?? "Not specified"}
                Requirements: {jobPosting.Requirements ?? "Not specified"}
                Minimum Education Level: {(jobPosting.MinEducationLevel.HasValue ? jobPosting.MinEducationLevel.Value.ToString() : "Not specified")}
                Minimum Experience (years): {(jobPosting.MinExperienceYears.HasValue ? jobPosting.MinExperienceYears.Value.ToString() : "Not specified")}
                Required District/Location: {jobPosting.RequiredDistrict ?? "Not specified"}
                Age Range: {(jobPosting.MinAge?.ToString() ?? "Not specified")}-{(jobPosting.MaxAge?.ToString() ?? "Not specified")}

                Candidate:
                Age: {(facts.Age.HasValue ? facts.Age.Value.ToString() : "Unknown")}
                Total Experience (years): {facts.TotalExperienceYears:F1}
                Education Levels: {(facts.EducationLevels.Count > 0 ? string.Join(", ", facts.EducationLevels) : "None listed")}
                Skills: {(facts.SkillNames.Count > 0 ? string.Join(", ", facts.SkillNames) : "None listed")}
                Address: {(string.IsNullOrWhiteSpace(facts.AddressText) ? "Not provided" : facts.AddressText)}

                Score this candidate's fit from 0 (no fit) to 100 (perfect fit) and briefly explain why.
                """;
        }
    }
}
