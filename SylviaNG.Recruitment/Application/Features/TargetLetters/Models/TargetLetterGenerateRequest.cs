namespace SylviaNG.Recruitment.Application.Features.TargetLetters.Models
{
    // AC2: FinalBody is the body HR already previewed/edited client-side via the existing
    // DocumentTemplateController.POST /preview endpoint - submitted here as the text to actually
    // PDF-and-send, no server-side re-render (same convention as AppointmentLetterGenerateRequest).
    public class TargetLetterGenerateRequest
    {
        public long OfferLetterId { get; set; }
        public long DocumentTemplateId { get; set; }
        public string Kpis { get; set; } = string.Empty;
        public string Objectives { get; set; } = string.Empty;
        public string FinalBody { get; set; } = string.Empty;
    }
}
