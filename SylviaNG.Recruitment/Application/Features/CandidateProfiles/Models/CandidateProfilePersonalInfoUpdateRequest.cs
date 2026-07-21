using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models
{
    public class CandidateProfilePersonalInfoUpdateRequest
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public GenderEnum? Gender { get; set; }
        public string? NationalId { get; set; }
        public string? FatherName { get; set; }
        public string? MotherName { get; set; }
        public MaritalStatusEnum? MaritalStatus { get; set; }
        public ReligionEnum? Religion { get; set; }
        public string? Nationality { get; set; }
        public BloodGroupEnum? BloodGroup { get; set; }
    }
}
