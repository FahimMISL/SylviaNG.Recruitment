using System.Text.Json.Serialization;
using SylviaNG.Recruitment.SharedKernel.Utils;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Models
{
    public class InterviewRescheduleRequest
    {
        [JsonConverter(typeof(LocalDateTimeJsonConverter))]
        public DateTime ScheduledStartAt { get; set; }
        [JsonConverter(typeof(LocalDateTimeJsonConverter))]
        public DateTime ScheduledEndAt { get; set; }

        /// <summary>Optional - if omitted, the interview keeps its current venue/room/meeting link
        /// and only the time changes.</summary>
        public long? InterviewVenueId { get; set; }
        public long? InterviewRoomId { get; set; }
        public string? MeetingLink { get; set; }
    }
}
