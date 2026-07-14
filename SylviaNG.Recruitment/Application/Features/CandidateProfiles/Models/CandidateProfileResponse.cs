namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models
{
    public class CandidateProfileResponse
    {
        public long CandidateProfileId { get; set; }

        // Personal info
        public string FullName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? NationalId { get; set; }
        public string? FatherName { get; set; }
        public string? MotherName { get; set; }
        public string? MaritalStatus { get; set; }
        public string? Religion { get; set; }
        public string? Nationality { get; set; }

        // Contact
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? PresentAddress { get; set; }
        public string? PermanentAddress { get; set; }

        // Photo/Signature
        public string? ProfilePhotoPath { get; set; }
        public string? SignaturePath { get; set; }

        public int CompletenessPercentage { get; set; }
    }
}
