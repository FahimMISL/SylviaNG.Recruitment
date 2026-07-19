namespace SylviaNG.Recruitment.Application.Features.TalentPools.Models
{
    public class TalentPoolFastTrackResponse
    {
        public int ProcessedCount { get; set; }
        public int FastTrackedCount { get; set; }
        public int AlreadyAppliedCount { get; set; }

        /// <summary>Candidate not found, or has no prior application to source a resume from.</summary>
        public int SkippedCount { get; set; }
    }
}
