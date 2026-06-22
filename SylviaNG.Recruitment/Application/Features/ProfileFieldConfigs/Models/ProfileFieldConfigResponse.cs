namespace SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Models
{
    public class ProfileFieldConfigResponse
    {
        public long ProfileFieldConfigId { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string FieldType { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        public bool IsVisible { get; set; }
        public int SortOrder { get; set; }
        public string? OptionsJson { get; set; }
        public bool IsActive { get; set; }
    }
}
