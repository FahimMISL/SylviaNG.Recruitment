namespace SylviaNG.Recruitment.Application.Features.AppointmentLetters.Models
{
    // AC3: FinalBody is the body HR already previewed/edited client-side via the existing
    // DocumentTemplateController.POST /preview endpoint - submitted here as the text to actually
    // PDF-and-send, no server-side re-render.
    public class AppointmentLetterGenerateRequest
    {
        public long OfferLetterId { get; set; }
        public long DocumentTemplateId { get; set; }
        public string FinalBody { get; set; } = string.Empty;
    }
}
