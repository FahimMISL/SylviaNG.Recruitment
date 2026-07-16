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
