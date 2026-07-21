using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Self-service candidate profile (EP-01/US-002). One row per Keycloak subject;
/// auto-provisioned on first GET, see CurrentCandidateService.
/// </summary>
public class CandidateProfile : Audit
{
    public long CandidateProfileId { get; set; }
    public string KeycloakSubjectId { get; set; } = string.Empty;

    // Personal info
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

    // Contact
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? PresentAddress { get; set; }
    public string? PermanentAddress { get; set; }

    // Photo/Signature (build phase 5)
    public string? ProfilePhotoPath { get; set; }
    public string? SignaturePath { get; set; }

    // HR-only free-text annotation (US-009 AC5) - never editable by the candidate.
    public string? HrNotes { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<CandidateEducation> Educations { get; set; } = new List<CandidateEducation>();
    public ICollection<CandidateWorkExperience> WorkExperiences { get; set; } = new List<CandidateWorkExperience>();
    public ICollection<CandidateSkill> Skills { get; set; } = new List<CandidateSkill>();
    public ICollection<CandidateCertification> Certifications { get; set; } = new List<CandidateCertification>();
    public ICollection<CandidateDocument> Documents { get; set; } = new List<CandidateDocument>();
}
