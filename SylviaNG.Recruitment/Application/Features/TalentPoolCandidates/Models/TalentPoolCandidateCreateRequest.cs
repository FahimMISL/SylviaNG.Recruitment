namespace SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Models
{
    public class TalentPoolCandidateCreateRequest
    {
        public long TalentPoolId { get; set; }
        public long CandidateId { get; set; }
        public int? Rank { get; set; }
    }
}
