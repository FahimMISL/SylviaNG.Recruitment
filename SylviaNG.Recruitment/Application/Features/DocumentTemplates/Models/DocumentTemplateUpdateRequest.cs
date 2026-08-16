namespace SylviaNG.Recruitment.Application.Features.DocumentTemplates.Models
{
    // DocumentType and Code are immutable after create - changing either would silently break any
    // OfferLetter (or future generation feature) already pointing at this template's type/identity.
    public class DocumentTemplateUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
