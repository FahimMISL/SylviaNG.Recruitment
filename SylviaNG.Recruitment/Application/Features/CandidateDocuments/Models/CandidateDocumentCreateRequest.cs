using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.CandidateDocuments.Models
{
    public class CandidateDocumentCreateRequest
    {
        public long CandidateId { get; set; }
        public CandidateDocumentTypeEnum DocumentType { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string? ContentType { get; set; }
        public long? FileSizeBytes { get; set; }
        public byte[]? FileData { get; set; }
        public bool IsDefault { get; set; }
    }
}
