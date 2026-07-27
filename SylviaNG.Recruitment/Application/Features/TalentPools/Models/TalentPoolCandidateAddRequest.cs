namespace SylviaNG.Recruitment.Application.Features.TalentPools.Models
{
    /// <summary>Bulk-add candidates to a pool, from any point in the pipeline (US-039 AC1).</summary>
    public class TalentPoolCandidateAddRequest
    {
        public List<long> CandidateProfileIds { get; set; } = new();
    }
}
