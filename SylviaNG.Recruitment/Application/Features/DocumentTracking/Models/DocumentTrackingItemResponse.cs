using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.DocumentTracking.Models
{
    // EP-10 US-085: one row per generated OfferLetter or AppointmentLetter, merged into a single
    // HR-facing tracking list. SourceId is the OfferLetterId/AppointmentLetterId depending on
    // DocumentType - use it together with DocumentType to call the FollowUp endpoint.
    public class DocumentTrackingItemResponse
    {
        public DocumentTypeEnum DocumentType { get; set; }
        public long SourceId { get; set; }
        public long JobApplicationId { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string? RecipientEmail { get; set; }
        public DateTime GeneratedAt { get; set; }
        public DocumentAcceptanceStatusEnum AcceptanceStatus { get; set; }
    }
}
