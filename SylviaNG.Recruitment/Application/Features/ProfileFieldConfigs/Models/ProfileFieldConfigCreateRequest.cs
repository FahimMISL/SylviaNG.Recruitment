namespace SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Models
{
    public class ProfileFieldConfigCreateRequest
    {
        public string FieldName { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string FieldType { get; set; } = "Text";
        public bool IsRequired { get; set; }
        public bool IsVisible { get; set; } = true;
        public int SortOrder { get; set; }
        public string? OptionsJson { get; set; }
    }
}
