namespace SylviaNG.Recruitment.Application.Features.DashboardWidgets.Models
{
    public class DashboardWidgetUpdateRequest
    {
        public long? UserId { get; set; }
        public string? WidgetType { get; set; }
        public string? Title { get; set; }
        public string? ConfigJson { get; set; }
        public int? SortOrder { get; set; }
        public bool? IsVisible { get; set; }
        public bool? IsActive { get; set; }
    }
}
