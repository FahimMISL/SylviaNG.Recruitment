namespace SylviaNG.Recruitment.Application.Features.TalentPools.Models
{
    public class TalentPoolUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public long? JobPostingId { get; set; }
    }
}
