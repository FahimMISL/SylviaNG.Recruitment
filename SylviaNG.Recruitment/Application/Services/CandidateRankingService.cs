using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Services;

public class CandidateRankingService : ICandidateRankingService
{
    private readonly IJobPostingRepository _jobPostingRepo;
    private readonly ICandidateRepository _candidateRepo;
    private readonly IJobApplicationRepository _jobApplicationRepo;
    private readonly ILogger<CandidateRankingService> _logger;

    private const double SkillsWeight = 0.40;
    private const double ExperienceWeight = 0.30;
    private const double ProfileCompletenessWeight = 0.15;
    private const double EducationWeight = 0.15;

    public CandidateRankingService(
        IJobPostingRepository jobPostingRepo,
        ICandidateRepository candidateRepo,
        IJobApplicationRepository jobApplicationRepo,
        ILogger<CandidateRankingService> logger)
    {
        _jobPostingRepo = jobPostingRepo;
        _candidateRepo = candidateRepo;
        _jobApplicationRepo = jobApplicationRepo;
        _logger = logger;
    }

    public async Task<List<CandidateRankResult>> RankCandidatesForJob(long jobPostingId)
    {
        var jobPosting = await _jobPostingRepo.GetByIdAsync(jobPostingId);

        if (jobPosting == null)
            throw new KeyNotFoundException($"Job posting with ID {jobPostingId} not found.");

        var jobKeywords = ExtractKeywords(jobPosting.Title, jobPosting.Description, jobPosting.Requirements);
        var jobEducationKeywords = ExtractEducationKeywords(jobPosting.Description, jobPosting.Requirements);

        var eligibleStatuses = new[]
        {
            ApplicationStatusEnum.Applied,
            ApplicationStatusEnum.Screening
        };

        var appliedCandidateIds = await _jobApplicationRepo.GetQueryable()
            .AsNoTracking()
            .Where(a => a.JobPostingId == jobPostingId
                        && a.IsActive
                        && a.CandidateId.HasValue
                        && eligibleStatuses.Contains(a.ApplicationStatus))
            .Select(a => a.CandidateId!.Value)
            .Distinct()
            .ToListAsync();

        if (appliedCandidateIds.Count == 0)
            return new List<CandidateRankResult>();

        var candidates = await _candidateRepo.GetQueryable()
            .AsNoTracking()
            .Where(c => c.IsActive && appliedCandidateIds.Contains(c.CandidateId))
            .Include(c => c.Skills.Where(s => s.IsActive))
            .Include(c => c.Experiences.Where(e => e.IsActive))
            .Include(c => c.Educations.Where(e => e.IsActive))
            .ToListAsync();

        var results = new List<CandidateRankResult>();

        foreach (var candidate in candidates)
        {
            var result = ScoreCandidate(candidate, jobPosting, jobKeywords, jobEducationKeywords);
            results.Add(result);
        }

        return results
            .OrderByDescending(r => r.MatchScore)
            .ToList();
    }

    private CandidateRankResult ScoreCandidate(
        Candidate candidate,
        JobPosting jobPosting,
        HashSet<string> jobKeywords,
        HashSet<string> jobEducationKeywords)
    {
        var candidateSkillNames = candidate.Skills
            .Select(s => s.SkillName.Trim().ToLowerInvariant())
            .ToList();

        var matchedSkills = new List<string>();
        var missingSkills = new List<string>();

        foreach (var keyword in jobKeywords)
        {
            bool found = candidateSkillNames.Any(cs =>
                cs.Contains(keyword) || keyword.Contains(cs));

            if (found)
                matchedSkills.Add(keyword);
            else
                missingSkills.Add(keyword);
        }

        double skillScore = jobKeywords.Count > 0
            ? (double)matchedSkills.Count / jobKeywords.Count * 100
            : 50;

        double experienceScore = CalculateExperienceScore(candidate, jobPosting);
        double profileScore = candidate.ProfileCompletenessPercent;
        double educationScore = CalculateEducationScore(candidate, jobEducationKeywords);

        int totalScore = (int)Math.Round(
            skillScore * SkillsWeight +
            experienceScore * ExperienceWeight +
            profileScore * ProfileCompletenessWeight +
            educationScore * EducationWeight
        );

        totalScore = Math.Clamp(totalScore, 0, 100);

        var explanation = GenerateExplanation(candidate, matchedSkills, missingSkills, totalScore);

        var displayMatched = matchedSkills.Select(s => CapitalizeFirst(s)).ToList();
        var displayMissing = missingSkills.Select(s => CapitalizeFirst(s)).ToList();

        return new CandidateRankResult
        {
            CandidateId = candidate.CandidateId,
            FullName = candidate.FullName,
            Email = candidate.Email,
            MatchScore = totalScore,
            Explanation = explanation,
            MatchedSkills = displayMatched,
            MissingSkills = displayMissing,
            ExperienceYears = candidate.TotalExperienceYears,
            CurrentDesignation = candidate.CurrentDesignation,
            ProfileCompleteness = candidate.ProfileCompletenessPercent
        };
    }

    private double CalculateExperienceScore(Candidate candidate, JobPosting jobPosting)
    {
        double score = 0;

        int years = candidate.TotalExperienceYears ?? 0;
        if (years > 0)
        {
            score += Math.Min(years, 10) * 5;
        }

        var jobTitleWords = Tokenize(jobPosting.Title);
        var jobDescWords = Tokenize(jobPosting.Description);
        var allJobWords = new HashSet<string>(jobTitleWords.Concat(jobDescWords));

        if (!string.IsNullOrWhiteSpace(candidate.CurrentDesignation))
        {
            var designationWords = Tokenize(candidate.CurrentDesignation);
            int designationMatches = designationWords.Count(d => allJobWords.Contains(d));
            if (designationMatches > 0)
                score += Math.Min(designationMatches * 15, 30);
        }

        foreach (var exp in candidate.Experiences)
        {
            var responsibilityWords = Tokenize(exp.Responsibilities);
            var designationWords = Tokenize(exp.Designation);
            var allExpWords = new HashSet<string>(responsibilityWords.Concat(designationWords));

            int expMatches = allExpWords.Count(w => allJobWords.Contains(w));
            if (expMatches > 0)
            {
                score += Math.Min(expMatches * 5, 20);
                break;
            }
        }

        return Math.Min(score, 100);
    }

    private double CalculateEducationScore(Candidate candidate, HashSet<string> educationKeywords)
    {
        if (!candidate.Educations.Any())
            return 20;

        double score = 30;

        foreach (var edu in candidate.Educations)
        {
            var degreeWords = Tokenize(edu.Degree);
            var fieldWords = Tokenize(edu.FieldOfStudy);
            var allEduWords = new HashSet<string>(degreeWords.Concat(fieldWords));

            int matches = allEduWords.Count(w => educationKeywords.Contains(w));
            if (matches > 0)
            {
                score += Math.Min(matches * 20, 70);
                break;
            }
        }

        var degrees = candidate.Educations.Select(e => e.Degree.ToLowerInvariant()).ToList();
        if (degrees.Any(d => d.Contains("phd") || d.Contains("doctorate")))
            score += 15;
        else if (degrees.Any(d => d.Contains("master") || d.Contains("msc") || d.Contains("mba") || d.Contains("ms ")))
            score += 10;
        else if (degrees.Any(d => d.Contains("bachelor") || d.Contains("bsc") || d.Contains("bba") || d.Contains("bs ")))
            score += 5;

        return Math.Min(score, 100);
    }

    private string GenerateExplanation(
        Candidate candidate,
        List<string> matchedSkills,
        List<string> missingSkills,
        int totalScore)
    {
        var parts = new List<string>();

        string tier = totalScore >= 70 ? "Strong match" : totalScore >= 40 ? "Moderate match" : "Low match";
        parts.Add(tier);

        if (matchedSkills.Count > 0)
        {
            var topSkills = matchedSkills.Take(4).Select(s => CapitalizeFirst(s));
            parts.Add($"has {string.Join(", ", topSkills)} skills");
        }

        if (candidate.TotalExperienceYears.HasValue && candidate.TotalExperienceYears > 0)
        {
            string expContext = !string.IsNullOrWhiteSpace(candidate.CurrentDesignation)
                ? $"{candidate.TotalExperienceYears} years experience as {candidate.CurrentDesignation}"
                : $"{candidate.TotalExperienceYears} years of professional experience";
            parts.Add(expContext);
        }

        if (missingSkills.Count > 0 && missingSkills.Count <= 3)
        {
            var topMissing = missingSkills.Take(3).Select(s => CapitalizeFirst(s));
            parts.Add($"missing {string.Join(", ", topMissing)}");
        }
        else if (missingSkills.Count > 3)
        {
            parts.Add($"missing {missingSkills.Count} required skills");
        }

        return string.Join(". ", parts) + ".";
    }

    private HashSet<string> ExtractKeywords(string title, string? description, string? requirements)
    {
        var combined = $"{title} {description ?? ""} {requirements ?? ""}";
        var tokens = Tokenize(combined);

        return tokens
            .Where(t => t.Length >= 2 && !StopWords.Contains(t))
            .ToHashSet();
    }

    private HashSet<string> ExtractEducationKeywords(string? description, string? requirements)
    {
        var combined = $"{description ?? ""} {requirements ?? ""}";
        var tokens = Tokenize(combined);

        var educationTerms = new HashSet<string>
        {
            "bachelor", "master", "phd", "doctorate", "mba", "bsc", "msc", "bba",
            "engineering", "computer", "science", "business", "management", "finance",
            "accounting", "marketing", "economics", "mathematics", "statistics",
            "information", "technology", "software", "electrical", "mechanical",
            "civil", "architecture", "design", "arts", "communications", "law",
            "medicine", "pharmacy", "nursing", "psychology", "sociology",
            "degree", "diploma", "certification", "certified"
        };

        return tokens.Where(t => educationTerms.Contains(t)).ToHashSet();
    }

    private static List<string> Tokenize(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new List<string>();

        text = Regex.Replace(text, @"<[^>]+>", " ");
        return Regex.Split(text.ToLowerInvariant(), @"[^a-z0-9#+]+")
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .ToList();
    }

    private static string CapitalizeFirst(string s)
    {
        if (string.IsNullOrEmpty(s)) return s;
        return char.ToUpperInvariant(s[0]) + s[1..];
    }

    private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "a", "an", "the", "and", "or", "but", "in", "on", "at", "to", "for",
        "of", "with", "by", "from", "is", "are", "was", "were", "be", "been",
        "being", "have", "has", "had", "do", "does", "did", "will", "would",
        "could", "should", "may", "might", "shall", "can", "need", "must",
        "it", "its", "this", "that", "these", "those", "we", "our", "you",
        "your", "they", "their", "he", "she", "his", "her", "who", "whom",
        "which", "what", "where", "when", "how", "why", "all", "each",
        "every", "both", "few", "more", "most", "other", "some", "such",
        "no", "not", "only", "own", "same", "so", "than", "too", "very",
        "just", "about", "above", "after", "again", "also", "any", "as",
        "because", "before", "below", "between", "during", "if", "into",
        "out", "over", "then", "under", "until", "up", "while",
        "able", "looking", "join", "team", "work", "working", "role",
        "position", "candidate", "required", "requirements", "preferred",
        "experience", "years", "strong", "good", "excellent", "knowledge",
        "skills", "skill", "understanding", "familiar", "proficient",
        "responsibilities", "including", "etc", "using", "used", "use"
    };
}
