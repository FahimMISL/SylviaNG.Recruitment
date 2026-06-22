namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models
{
    public class JoiningBookletResponse
    {
        public long JoiningBookletId { get; set; }
        public long CandidateId { get; set; }
        public long JobApplicationId { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string? FileName { get; set; }
        public DateTime GeneratedAt { get; set; }
        public long GeneratedByUserId { get; set; }
        public bool IsActive { get; set; }
    }
}
