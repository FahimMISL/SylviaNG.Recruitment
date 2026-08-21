namespace SylviaNG.Recruitment.Application.Features.JobPostings.Models
{
    /// <summary>US-101 AC1: bulk-download the CVs of selected applications from the ATS
    /// application list as a single ZIP.</summary>
    public class JobApplicationCvBulkDownloadRequest
    {
        public List<long> JobApplicationIds { get; set; } = new();
    }

    public class JobApplicationCvBulkDownloadResponse
    {
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }
}
