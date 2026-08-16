namespace SylviaNG.Recruitment.Application.Features.TalentPools.Models
{
    /// <summary>Id+Name only, for the "add candidate to pool" picker dropdown.</summary>
    public class TalentPoolLookupResponse
    {
        public long TalentPoolId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
