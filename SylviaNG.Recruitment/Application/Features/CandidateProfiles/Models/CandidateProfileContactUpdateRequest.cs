using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models
{
    public class CandidateProfileContactUpdateRequest
    {
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public MobileOperatorEnum? MobileOperator { get; set; }

        public long? PresentDivisionId { get; set; }
        public long? PresentDistrictId { get; set; }
        public long? PresentThanaId { get; set; }
        public string? PresentAddressDetail { get; set; }

        public long? HomeDivisionId { get; set; }
        public long? HomeDistrictId { get; set; }
        public long? HomeThanaId { get; set; }
        public string? PermanentAddressDetail { get; set; }
    }
}
