namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models
{
    public class JoiningBookletBulkItemResult
    {
        public long OfferLetterId { get; set; }
        public bool Success { get; set; }
        public long? JoiningBookletId { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
