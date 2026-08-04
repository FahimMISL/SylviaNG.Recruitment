namespace SylviaNG.Recruitment.Application.Features.InterviewRoundConfigs.Models
{
    public class InterviewRoundConfigRequest
    {
        /// <summary>Null/0 for a new round; an existing InterviewRoundConfigId to keep updating it.</summary>
        public long? InterviewRoundConfigId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Sequence { get; set; }
        public long? ScorecardId { get; set; }

        /// <summary>Free-typed suggested panel member Employee IDs for this round - same
        /// no-picker precedent as Interview.PanelistEmployeeIds.</summary>
        public List<long> PanelistEmployeeIds { get; set; } = new();
    }
}
