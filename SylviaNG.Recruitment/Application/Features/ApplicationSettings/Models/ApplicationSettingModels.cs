namespace SylviaNG.Recruitment.Application.Features.ApplicationSettings.Models
{
    public class ApplicationSettingResponse
    {
        public int MinimumProfileCompletenessPercentage { get; set; }
    }

    public class ApplicationSettingUpdateRequest
    {
        public int MinimumProfileCompletenessPercentage { get; set; }
    }
}
