using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models
{
    public class CandidateDocumentUploadRequest
    {
        public CandidateDocumentTypeEnum DocumentType { get; set; }
        public IFormFile File { get; set; } = null!;
    }
}
