using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using System.Text;

namespace SylviaNG.Recruitment.Application.Services
{
    /// <summary>
    /// Deterministic, rule-based candidate scorer (US-046 "Manual" provider) - no external calls.
    /// 100-point weighted formula: Education 25, Experience 25, Skills 30, Location 10, Age 10.
    /// A criterion the job posting doesn't specify scores as neutral (full points, not zero) and
    /// is omitted from the explanation, since "no requirement" isn't evidence against a candidate.
    /// </summary>
    public class ManualShortlistScoringService : IShortlistScoringService
    {
        private const int EducationMaxPoints = 25;
        private const int ExperienceMaxPoints = 25;
        private const int SkillsMaxPoints = 30;
        private const int LocationMaxPoints = 10;
        private const int AgeMaxPoints = 10;
        private const int PointsPerMatchedSkill = 6;

        public string ProviderName => "Manual";

        public Task<CandidateScoringResult> ScoreAsync(JobPosting jobPosting, CandidateFactService.CandidateFacts facts, CancellationToken ct = default)
        {
            var clauses = new List<string>();
            var total = 0;

            total += ScoreEducation(jobPosting, facts, clauses);
            total += ScoreExperience(jobPosting, facts, clauses);
            total += ScoreSkills(jobPosting, facts, clauses);
            total += ScoreLocation(jobPosting, facts, clauses);
            total += ScoreAge(jobPosting, facts, clauses);

            var explanation = new StringBuilder();
            foreach (var clause in clauses)
                explanation.Append(clause).Append(' ');
            explanation.Append($"Total: {total}/100.");

            return Task.FromResult(new CandidateScoringResult(total, explanation.ToString().Trim(), false, null));
        }

        private static int ScoreEducation(JobPosting jobPosting, CandidateFactService.CandidateFacts facts, List<string> clauses)
        {
            if (!jobPosting.MinEducationLevel.HasValue)
                return EducationMaxPoints;

            var meets = facts.EducationLevels.Any(l => (int)l >= (int)jobPosting.MinEducationLevel.Value);
            var points = meets ? EducationMaxPoints : 0;
            clauses.Add(meets
                ? $"Meets education requirement ({jobPosting.MinEducationLevel}+): {points}/{EducationMaxPoints}."
                : $"Does not meet education requirement ({jobPosting.MinEducationLevel}+): {points}/{EducationMaxPoints}.");
            return points;
        }

        private static int ScoreExperience(JobPosting jobPosting, CandidateFactService.CandidateFacts facts, List<string> clauses)
        {
            if (!jobPosting.MinExperienceYears.HasValue || jobPosting.MinExperienceYears.Value <= 0)
                return ExperienceMaxPoints;

            var required = jobPosting.MinExperienceYears.Value;
            var points = facts.TotalExperienceYears >= required
                ? ExperienceMaxPoints
                : Math.Max(0, (int)Math.Round(ExperienceMaxPoints * (facts.TotalExperienceYears / required)));

            clauses.Add($"Experience {facts.TotalExperienceYears:F1} yrs vs required {required}: {points}/{ExperienceMaxPoints}.");
            return points;
        }

        private static int ScoreSkills(JobPosting jobPosting, CandidateFactService.CandidateFacts facts, List<string> clauses)
        {
            if (string.IsNullOrWhiteSpace($"{jobPosting.Requirements} {jobPosting.Description}".Trim()))
                return SkillsMaxPoints;

            var matched = CandidateMatchAnalyzer.GetMatchedSkills(jobPosting, facts);
            var points = Math.Min(SkillsMaxPoints, matched.Count * PointsPerMatchedSkill);
            clauses.Add(matched.Count > 0
                ? $"Skills: {matched.Count}/{facts.SkillNames.Count} candidate skills matched in requirements ({string.Join(", ", matched)}): {points}/{SkillsMaxPoints}."
                : $"Skills: no candidate skills matched in requirements: {points}/{SkillsMaxPoints}.");
            return points;
        }

        private static int ScoreLocation(JobPosting jobPosting, CandidateFactService.CandidateFacts facts, List<string> clauses)
        {
            if (string.IsNullOrWhiteSpace(jobPosting.RequiredDistrict))
                return LocationMaxPoints;

            var meets = facts.AddressText.Contains(jobPosting.RequiredDistrict, StringComparison.OrdinalIgnoreCase);
            var points = meets ? LocationMaxPoints : 0;
            clauses.Add(meets
                ? $"Location matches required district '{jobPosting.RequiredDistrict}': {points}/{LocationMaxPoints}."
                : $"Location does not match required district '{jobPosting.RequiredDistrict}': {points}/{LocationMaxPoints}.");
            return points;
        }

        private static int ScoreAge(JobPosting jobPosting, CandidateFactService.CandidateFacts facts, List<string> clauses)
        {
            if (!jobPosting.MinAge.HasValue && !jobPosting.MaxAge.HasValue)
                return AgeMaxPoints;

            if (!facts.Age.HasValue)
            {
                clauses.Add($"Age not on file, cannot verify against required range: 0/{AgeMaxPoints}.");
                return 0;
            }

            var meets = (!jobPosting.MinAge.HasValue || facts.Age.Value >= jobPosting.MinAge.Value)
                && (!jobPosting.MaxAge.HasValue || facts.Age.Value <= jobPosting.MaxAge.Value);
            var points = meets ? AgeMaxPoints : 0;
            clauses.Add(meets
                ? $"Age {facts.Age} within required range: {points}/{AgeMaxPoints}."
                : $"Age {facts.Age} outside required range: {points}/{AgeMaxPoints}.");
            return points;
        }
    }
}
