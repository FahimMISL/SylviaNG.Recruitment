namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models
{
    public class JoiningBookletFileResponse
    {
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }
}
