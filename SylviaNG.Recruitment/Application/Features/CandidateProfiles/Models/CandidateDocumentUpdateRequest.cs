using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models
{
    public class CandidateDocumentUpdateRequest
    {
        public CandidateDocumentTypeEnum DocumentType { get; set; }

        /// <summary>
        /// If provided, replaces the existing file (old one is deleted). If null, the existing
        /// file is left untouched and only DocumentType is updated.
        /// </summary>
        public IFormFile? File { get; set; }
    }
}
