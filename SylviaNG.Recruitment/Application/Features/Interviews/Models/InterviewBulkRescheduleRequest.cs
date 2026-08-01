namespace SylviaNG.Recruitment.Application.Features.Interviews.Models
{
    /// <summary>Shifts each listed interview to a new staggered slot starting at StartAt, keeping
    /// each interview's own venue/room/duration - only the time moves.</summary>
    public class InterviewBulkRescheduleRequest
    {
        public List<long> InterviewIds { get; set; } = new();
        public DateTime StartAt { get; set; }
        public int GapMinutes { get; set; } = 0;
    }
}
