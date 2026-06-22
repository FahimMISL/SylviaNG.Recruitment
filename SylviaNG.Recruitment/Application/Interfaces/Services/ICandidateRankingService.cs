namespace SylviaNG.Recruitment.Application.Interfaces.Services;

public interface ICandidateRankingService
{
    Task<List<CandidateRankResult>> RankCandidatesForJob(long jobPostingId);
}

public class CandidateRankResult
{
    public long CandidateId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int MatchScore { get; set; }
    public string Explanation { get; set; } = string.Empty;
    public List<string> MatchedSkills { get; set; } = new();
    public List<string> MissingSkills { get; set; } = new();
    public int? ExperienceYears { get; set; }
    public string? CurrentDesignation { get; set; }
    public int ProfileCompleteness { get; set; }
}
