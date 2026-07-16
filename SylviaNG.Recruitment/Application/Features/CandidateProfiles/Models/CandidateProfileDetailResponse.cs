using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Features.TalentPools.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models
{
    /// <summary>
    /// Full read-only aggregate for the HR/Admin candidate profile view (US-009 AC1/AC3/AC4).
    /// </summary>
    public class CandidateProfileDetailResponse
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

        public List<CandidateEducationResponse> Educations { get; set; } = new();
        public List<CandidateWorkExperienceResponse> WorkExperiences { get; set; } = new();
        public List<CandidateSkillResponse> Skills { get; set; } = new();
        public List<CandidateCertificationResponse> Certifications { get; set; } = new();
        public List<CandidateDocumentResponse> Documents { get; set; } = new();

        /// <summary>Every job this candidate has applied to, across all postings (AC4).</summary>
        public List<JobApplicationResponse> ApplicationHistory { get; set; } = new();

        /// <summary>HR-only annotation (AC5) - never shown to or editable by the candidate.</summary>
        public string? HrNotes { get; set; }

        /// <summary>Named talent pools this candidate belongs to (US-039 AC2) - HR/Admin view only.</summary>
        public List<TalentPoolBadgeResponse> TalentPools { get; set; } = new();
    }
}
