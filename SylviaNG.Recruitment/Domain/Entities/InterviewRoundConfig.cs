namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// A named, sequenced interview round configured for a job posting (EP-08 US-070), e.g.
/// "Technical Round 1" (Sequence 1), "HR Round" (Sequence 2). Referenced by Interview at
/// schedule time via Interview.InterviewRoundConfigId (nullable - job postings with no
/// configured rounds keep scheduling exactly as before, free-typed Round int only).
/// </summary>
public class InterviewRoundConfig
{
    public long InterviewRoundConfigId { get; set; }
    public long JobPostingId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Sequence { get; set; }
    public long? ScorecardId { get; set; }

    // Navigation properties
    public JobPosting JobPosting { get; set; } = null!;
    public Scorecard? Scorecard { get; set; }
    public ICollection<InterviewRoundConfigPanelMember> PanelMembers { get; set; } = new List<InterviewRoundConfigPanelMember>();
}
