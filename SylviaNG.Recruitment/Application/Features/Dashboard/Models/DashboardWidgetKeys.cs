namespace SylviaNG.Recruitment.Application.Features.Dashboard.Models
{
    /// <summary>The 5 Admin/HR dashboard metric cards from US-105 AC1, keyed for the
    /// widget-visibility config (AC5). Shared between DashboardWidgetConfigConfiguration's seed
    /// and DashboardService's response filtering so the two never drift.</summary>
    public static class DashboardWidgetKeys
    {
        public const string OpenVacancies = "OpenVacancies";
        public const string TotalApplications = "TotalApplications";
        public const string UpcomingInterviews = "UpcomingInterviews";
        public const string PendingApprovals = "PendingApprovals";
        public const string OffersPendingAcceptance = "OffersPendingAcceptance";

        public static readonly string[] All =
        {
            OpenVacancies, TotalApplications, UpcomingInterviews, PendingApprovals, OffersPendingAcceptance
        };
    }
}
