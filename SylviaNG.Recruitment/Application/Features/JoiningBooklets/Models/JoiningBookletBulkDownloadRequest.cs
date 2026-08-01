namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models
{
    public class JoiningBookletBulkDownloadRequest
    {
        public List<long> JoiningBookletIds { get; set; } = new();
    }
}
