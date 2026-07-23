namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models
{
    public class CandidateProfilePersonalInfoUpdateRequest
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public long? GenderId { get; set; }
        public string? NationalId { get; set; }
        public string? FatherName { get; set; }
        public string? MotherName { get; set; }
        public long? MaritalStatusId { get; set; }
        public long? ReligionId { get; set; }
        public string? Nationality { get; set; }
        public long? BloodGroupId { get; set; }
    }
}
