namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models
{
    // AC1/AC2 batch stub: no FinalSelectionPool entity exists yet, so HR just lists the
    // Accepted-offer OfferLetterIds it selected plus a batch label/joining date entered ad hoc.
    public class JoiningBookletBulkGenerateRequest
    {
        public List<long> OfferLetterIds { get; set; } = new();
        public long DocumentTemplateId { get; set; }
        public string BatchLabel { get; set; } = string.Empty;
        public DateTime JoiningDate { get; set; }
    }
}
