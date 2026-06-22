using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class ProfileFieldConfig : Audit
{
    public long ProfileFieldConfigId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string FieldType { get; set; } = "Text";
    public bool IsRequired { get; set; }
    public bool IsVisible { get; set; } = true;
    public int SortOrder { get; set; }
    public string? OptionsJson { get; set; }
    public bool IsActive { get; set; } = true;
}
