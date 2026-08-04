namespace SylviaNG.Recruitment.Application.Features.OfficeNotes.Models
{
    public class OfficeNoteGenerateRequest
    {
        public long JobApplicationId { get; set; }
        public long DocumentTemplateId { get; set; }
        public string? Remarks { get; set; }
    }
}
