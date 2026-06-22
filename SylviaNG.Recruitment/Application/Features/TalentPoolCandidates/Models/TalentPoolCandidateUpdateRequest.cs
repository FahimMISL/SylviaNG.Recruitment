namespace SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Models
{
    public class TalentPoolCandidateUpdateRequest
    {
        public long? TalentPoolId { get; set; }
        public long? CandidateId { get; set; }
        public int? Rank { get; set; }
        public bool? IsActive { get; set; }
    }
}
