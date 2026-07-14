namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models
{
    public class CandidateProfilePersonalInfoUpdateRequest
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? NationalId { get; set; }
        public string? FatherName { get; set; }
        public string? MotherName { get; set; }
        public string? MaritalStatus { get; set; }
        public string? Religion { get; set; }
        public string? Nationality { get; set; }
    }
}
