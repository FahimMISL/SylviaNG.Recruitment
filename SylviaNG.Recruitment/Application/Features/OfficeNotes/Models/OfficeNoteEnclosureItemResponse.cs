using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.OfficeNotes.Models
{
    public class OfficeNoteEnclosureItemResponse
    {
        public DocumentTypeEnum DocumentType { get; set; }
        public bool Exists { get; set; }
        public DateTime? GeneratedAt { get; set; }
    }
}
