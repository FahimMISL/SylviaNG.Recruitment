namespace SylviaNG.Recruitment.Application.Features.AutoShortlisting.Models
{
    public class AutoShortlistRunRequest
    {
        public long JobPostingId { get; set; }
        public int CutoffScore { get; set; }
    }
}
