namespace SylviaNG.Recruitment.Application.Features.Interviews.Models
{
    public class InterviewBulkCancelRequest
    {
        public List<long> InterviewIds { get; set; } = new();
        public string CancellationReason { get; set; } = string.Empty;
    }
}
