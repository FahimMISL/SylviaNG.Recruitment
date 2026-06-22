using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.Candidates.Models
{
    public class CandidateUpdateRequest
    {
        public string? KeycloakUserId { get; set; }
        public CandidateTypeEnum? CandidateType { get; set; }
        public long? EmployeeId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? NationalIdNumber { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        public string? CurrentDesignation { get; set; }
        public string? CurrentOrganization { get; set; }
        public int? TotalExperienceYears { get; set; }
        public string? ExpectedSalary { get; set; }
        public int? ProfileCompletenessPercent { get; set; }
        public bool? IsEmailVerified { get; set; }
        public bool? IsPhoneVerified { get; set; }
        public bool? IsActive { get; set; }
        public string? PresentAddress { get; set; }
        public string? PermanentAddress { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? GitHubUrl { get; set; }
        public string? PortfolioUrl { get; set; }
        public string? FatherName { get; set; }
        public string? MotherName { get; set; }
        public string? MaritalStatus { get; set; }
        public string? Religion { get; set; }
        public string? BloodGroup { get; set; }
    }
}
