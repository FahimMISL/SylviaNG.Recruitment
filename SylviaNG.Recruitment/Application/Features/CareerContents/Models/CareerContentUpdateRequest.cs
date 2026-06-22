using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.CareerContents.Models
{
    public class CareerContentUpdateRequest
    {
        public CareerContentTypeEnum? ContentType { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; }
        public string? MediaUrl { get; set; }
        public int? SortOrder { get; set; }
        public bool? IsPublished { get; set; }
        public bool? IsActive { get; set; }
    }
}
