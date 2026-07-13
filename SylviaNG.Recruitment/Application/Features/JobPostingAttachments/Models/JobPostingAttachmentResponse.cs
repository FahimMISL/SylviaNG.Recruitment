namespace SylviaNG.Recruitment.Application.Features.JobPostingAttachments.Models
{
    public class JobPostingAttachmentResponse
    {
        public long JobPostingAttachmentId { get; set; }
        public long JobPostingId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public bool IsActive { get; set; }

        /// <summary>
        /// Relative static-file URL (e.g. "/uploads/job-postings/{jobPostingId}/{storedFileName}").
        /// The frontend prefixes this with its own API base host to download the file.
        /// </summary>
        public string DownloadUrl { get; set; } = string.Empty;

        public DateTime? CreatedAt { get; set; }
    }
}
