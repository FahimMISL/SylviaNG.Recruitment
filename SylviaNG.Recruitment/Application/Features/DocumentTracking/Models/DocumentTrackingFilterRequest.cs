using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.DocumentTracking.Models
{
    public class DocumentTrackingFilterRequest
    {
        public DocumentTypeEnum? DocumentType { get; set; }
        public DocumentAcceptanceStatusEnum? AcceptanceStatus { get; set; }
        public long? JobApplicationId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
