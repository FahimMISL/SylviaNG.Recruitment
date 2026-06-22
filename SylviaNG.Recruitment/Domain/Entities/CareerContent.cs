using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class CareerContent : Audit
{
    public long CareerContentId { get; set; }
    public CareerContentTypeEnum ContentType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Body { get; set; }
    public string? MediaUrl { get; set; }
    public int SortOrder { get; set; }
    public bool IsPublished { get; set; }
    public bool IsActive { get; set; } = true;
}
