using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Models
{
    public class ProfileFieldConfigRequest
    {
        public CandidateProfileFieldEnum Field { get; set; }
        public long? JobPostingId { get; set; }
        public ProfileFieldVisibilityEnum Visibility { get; set; }
    }
}
