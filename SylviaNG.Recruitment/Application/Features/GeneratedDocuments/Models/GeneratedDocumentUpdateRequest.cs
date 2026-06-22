namespace SylviaNG.Recruitment.Application.Features.GeneratedDocuments.Models
{
    public class GeneratedDocumentUpdateRequest
    {
        public long? DocumentTemplateId { get; set; }
        public long? CandidateId { get; set; }
        public long? JobApplicationId { get; set; }
        public string? FileUrl { get; set; }
        public string? FileName { get; set; }
        public string? FileFormat { get; set; }
        public DateTime? GeneratedAt { get; set; }
        public long? GeneratedByUserId { get; set; }
        public bool? IsActive { get; set; }
    }
}
