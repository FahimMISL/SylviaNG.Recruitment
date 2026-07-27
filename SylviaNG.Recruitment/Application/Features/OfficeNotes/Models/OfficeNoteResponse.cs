namespace SylviaNG.Recruitment.Application.Features.OfficeNotes.Models
{
    public class OfficeNoteResponse
    {
        public long OfficeNoteId { get; set; }
        public long JobApplicationId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public long DocumentTemplateId { get; set; }
        public string DocumentTemplateName { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public string EnclosuresSummary { get; set; } = string.Empty;
        public string GeneratedPdfPath { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
    }
}
