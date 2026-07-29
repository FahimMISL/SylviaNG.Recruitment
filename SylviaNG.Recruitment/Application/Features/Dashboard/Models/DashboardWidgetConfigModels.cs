namespace SylviaNG.Recruitment.Application.Features.Dashboard.Models
{
    public class DashboardWidgetConfigResponse
    {
        public string WidgetKey { get; set; } = string.Empty;
        public bool IsVisibleForAdmin { get; set; }
        public bool IsVisibleForHR { get; set; }
    }

    public class DashboardWidgetConfigUpdateRequest
    {
        public bool IsVisibleForAdmin { get; set; }
        public bool IsVisibleForHR { get; set; }
    }
}
