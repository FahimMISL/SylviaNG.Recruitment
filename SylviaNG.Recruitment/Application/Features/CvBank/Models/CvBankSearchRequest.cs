using System.ComponentModel.DataAnnotations;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.CvBank.Models
{
    /// <summary>Request for the CV Bank Boolean search (US-045).</summary>
    public class CvBankSearchRequest
    {
        /// <summary>Boolean query (AND/OR/NOT, parentheses, "quoted phrases"). Optional - filters alone are a valid search.</summary>
        public string? BooleanQuery { get; set; }

        public EducationLevelEnum? EducationLevel { get; set; }
        public double? MinExperienceYears { get; set; }
        public double? MaxExperienceYears { get; set; }
        public string? Location { get; set; }

        /// <summary>Derived from the candidate's JobApplication.Source history - see CvBankSearchHandler.</summary>
        public ApplicationSourceEnum? CandidateType { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
        public int Page { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100")]
        public int PageSize { get; set; } = 10;
    }
}
