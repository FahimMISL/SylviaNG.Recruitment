using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.Analytics.Models
{
    public class TimeToHireResponse
    {
        /// <summary>AC1/AC2: days from JobPosting.PostingDate to OfferLetter.DecisionAt, one
        /// sample per accepted offer (not per vacancy - see feature doc's scope decision on
        /// multi-position postings).</summary>
        public double? AverageDays { get; set; }
        public int? MinDays { get; set; }
        public int? MaxDays { get; set; }

        /// <summary>Distinct JobPostingId count contributing at least one sample, for context
        /// alongside the per-offer average above.</summary>
        public int VacancyCount { get; set; }

        /// <summary>AC3: average time spent immediately before transitioning into each status,
        /// computed from consecutive ApplicationStatusHistory.ChangedAt deltas.</summary>
        public List<StageDurationResponse> StageBreakdown { get; set; } = new();
    }

    public class StageDurationResponse
    {
        public ApplicationStatusEnum ToStatus { get; set; }
        public double AverageDays { get; set; }
        public int SampleSize { get; set; }
    }
}
