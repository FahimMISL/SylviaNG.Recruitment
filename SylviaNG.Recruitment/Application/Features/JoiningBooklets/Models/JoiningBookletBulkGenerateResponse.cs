namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models
{
    // AC5: per-candidate success/failure so a batch never gets aborted by one bad record.
    public class JoiningBookletBulkGenerateResponse
    {
        public int TotalRequested { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<JoiningBookletBulkItemResult> Results { get; set; } = new();
    }
}
