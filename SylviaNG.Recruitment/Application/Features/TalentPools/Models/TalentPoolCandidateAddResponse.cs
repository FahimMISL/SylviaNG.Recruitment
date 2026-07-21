namespace SylviaNG.Recruitment.Application.Features.TalentPools.Models
{
    public class TalentPoolCandidateAddResponse
    {
        public int AddedCount { get; set; }
        public int AlreadyInPoolCount { get; set; }
        public int NotFoundCount { get; set; }
    }
}
