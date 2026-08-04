using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.Analytics.Models
{
    public class RecruitmentFunnelResponse
    {
        public List<FunnelStageResponse> Stages { get; set; } = new();
    }

    /// <summary>EP-14 US-106: one row per real ApplicationStatusEnum funnel stage (Applied through
    /// Hired - terminal statuses Rejected/Withdrawn/AwaitingPayment/DuplicateDismissed are not
    /// funnel rows, see the feature doc's scope decision on stage vocabulary).</summary>
    public class FunnelStageResponse
    {
        public ApplicationStatusEnum Status { get; set; }
        public string Label { get; set; } = string.Empty;

        /// <summary>AC1/AC3: distinct applications that ever reached this status within scope.</summary>
        public int Count { get; set; }

        /// <summary>AC3: Count / previous stage's Count * 100. Null for the first stage.</summary>
        public double? ConversionFromPreviousPercent { get; set; }

        /// <summary>AC4: of the applications that reached this stage, how many also reached the
        /// next funnel stage (or Hired).</summary>
        public int PassedCount { get; set; }

        /// <summary>AC4: of the applications that reached this stage, how many terminated here
        /// (Rejected/Withdrawn) without progressing further.</summary>
        public int DroppedCount { get; set; }

        /// <summary>AC4: drop-off reason breakdown, only where ApplicationStatusHistory.ReasonId
        /// was recorded on the terminating Rejected/Withdrawn transition.</summary>
        public List<FunnelDropOffReasonResponse> DropOffReasons { get; set; } = new();
    }

    public class FunnelDropOffReasonResponse
    {
        public string ReasonLabel { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
