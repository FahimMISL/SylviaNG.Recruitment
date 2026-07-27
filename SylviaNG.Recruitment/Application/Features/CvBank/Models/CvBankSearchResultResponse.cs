namespace SylviaNG.Recruitment.Application.Features.CvBank.Models
{
    /// <summary>Row shape for a CV Bank search result (US-045 AC3).</summary>
    public class CvBankSearchResultResponse
    {
        public long CandidateProfileId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? ProfilePhotoPath { get; set; }
        public string? EducationSummary { get; set; }
        public double TotalExperienceYears { get; set; }

        /// <summary>Count of distinct query terms matched - higher ranks first.</summary>
        public int RelevanceScore { get; set; }
    }
}
