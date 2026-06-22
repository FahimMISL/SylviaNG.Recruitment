using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.CandidateDocuments.Models
{
    public class CandidateDocumentUpdateRequest
    {
        public long? CandidateId { get; set; }
        public CandidateDocumentTypeEnum? DocumentType { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
        public string? ContentType { get; set; }
        public long? FileSizeBytes { get; set; }
        public bool? IsDefault { get; set; }
        public bool? IsActive { get; set; }
    }
}
