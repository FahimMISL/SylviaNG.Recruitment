namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models
{
    /// <summary>US-103: system-generated profile summary PDF for one candidate.</summary>
    public class CandidateProfileDownloadResponse
    {
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }
}
