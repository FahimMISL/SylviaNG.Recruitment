using System.ComponentModel.DataAnnotations.Schema;
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

    // Internal candidate / Core HR pre-population (US-005). Set once at first provisioning
    // (CurrentCandidateService) when the logged-in user's email matches an Employee row - never
    // re-synced afterward, so it can't clobber later candidate edits. EmployeeId presence alone
    // distinguishes internal vs external (AC4); no separate flag needed. Department/Designation
    // are Core-HR-owned org data, shown read-only, not part of the editable profile form.
    public long? EmployeeId { get; set; }
    public long? DepartmentId { get; set; }
    public long? DesignationId { get; set; }

    // Admin override (this feature) - lets HR/Admin flag a candidate internal without a Core HR
    // Employee sync match, and lets the Hired transition (JobApplicationService) auto-flag a
    // newly-hired candidate. IsInternal is the single source of truth callers should read.
    public bool IsManuallyInternal { get; set; }

    [NotMapped]
    public bool IsInternal => EmployeeId.HasValue || IsManuallyInternal;

    // Immutable snapshot of what Core HR supplied at provisioning time, for the editable fields
    // (FullName, Phone) only - compared against the current value to flag drift for HR (AC2)
    // without needing a mutable flag that can go stale.
    public string? PrepopulatedFullName { get; set; }
    public string? PrepopulatedPhone { get; set; }

    // Navigation properties
    public ICollection<CandidateEducation> Educations { get; set; } = new List<CandidateEducation>();
    public ICollection<CandidateWorkExperience> WorkExperiences { get; set; } = new List<CandidateWorkExperience>();
    public ICollection<CandidateSkill> Skills { get; set; } = new List<CandidateSkill>();
    public ICollection<CandidateCertification> Certifications { get; set; } = new List<CandidateCertification>();
    public ICollection<CandidateDocument> Documents { get; set; } = new List<CandidateDocument>();
    public ICollection<CandidateTag> Tags { get; set; } = new List<CandidateTag>();
}
