namespace SylviaNG.Recruitment.Application.Features.CvBank.Models
{
    /// <summary>Bulk-add request from a CV Bank search result selection (US-045 AC5).</summary>
    public class CvBankTalentPoolAddRequest
    {
        public List<long> CandidateProfileIds { get; set; } = new();
    }

    public class CvBankTalentPoolAddResponse
    {
        public int AddedCount { get; set; }
        public int AlreadyInPoolCount { get; set; }
    }

    /// <summary>Row shape for the Talent Pool list view.</summary>
    public class CvBankTalentPoolEntryResponse
    {
        public long CandidateProfileId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? ProfilePhotoPath { get; set; }
        public DateTime? AddedAt { get; set; }
    }
}
