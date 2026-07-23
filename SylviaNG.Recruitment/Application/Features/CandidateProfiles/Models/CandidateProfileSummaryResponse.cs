namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models
{
    /// <summary>Row shape for the HR/Admin candidate list (US-009).</summary>
    public class CandidateProfileSummaryResponse
    {
        public long CandidateProfileId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? ProfilePhotoPath { get; set; }
        public int CompletenessPercentage { get; set; }
        public bool IsInternal { get; set; }
    }
}
