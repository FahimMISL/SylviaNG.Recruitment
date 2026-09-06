namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models
{
    public class JoiningBookletGenerateRequest
    {
        public long OfferLetterId { get; set; }
        public long DocumentTemplateId { get; set; }
        public string BatchLabel { get; set; } = string.Empty;
        public DateTime JoiningDate { get; set; }
    }
}
