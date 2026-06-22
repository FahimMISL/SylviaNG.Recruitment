namespace SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Models
{
    public class ProfileFieldConfigUpdateRequest
    {
        public string? Label { get; set; }
        public string? FieldType { get; set; }
        public bool? IsRequired { get; set; }
        public bool? IsVisible { get; set; }
        public int? SortOrder { get; set; }
        public string? OptionsJson { get; set; }
        public bool? IsActive { get; set; }
    }
}
