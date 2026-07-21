using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models
{
    public class CandidateProfileResponse
    {
        public long CandidateProfileId { get; set; }

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
        public MobileOperatorEnum? MobileOperator { get; set; }

        public long? PresentDivisionId { get; set; }
        public long? PresentDistrictId { get; set; }
        public long? PresentThanaId { get; set; }
        public string? PresentAddressDetail { get; set; }

        public long? HomeDivisionId { get; set; }
        public long? HomeDistrictId { get; set; }
        public long? HomeThanaId { get; set; }
        public string? PermanentAddressDetail { get; set; }

        // Photo/Signature
        public string? ProfilePhotoPath { get; set; }
        public string? SignaturePath { get; set; }

        public int CompletenessPercentage { get; set; }

        /// <summary>
        /// True once the candidate has at least one submitted JobApplication. Email, Phone, and
        /// NationalId become read-only in that state (US-003 AC4) - JobApplication has no FK to
        /// CandidateProfile and matches self-service lookups by Email, so changing these fields
        /// after applying would orphan the candidate's own application history (US-003 AC1/AC2).
        /// </summary>
        public bool HasSubmittedApplication { get; set; }

        // Internal candidate / Core HR pre-population (US-005).
        /// <summary>True when this profile was matched to a Core HR Employee record (AC4).</summary>
        public bool IsInternal { get; set; }
        public string? DepartmentName { get; set; }
        public string? DesignationName { get; set; }

        /// <summary>
        /// True when the candidate has edited a Core-HR-pre-populated field (FullName/Phone)
        /// since it was fetched - surfaced to HR (AC2). Always false for external candidates.
        /// </summary>
        public bool HasPrepopulatedFieldEdits { get; set; }
    }
}
