using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.ExportRequests.Models
{
    public class ExportRequestResponse
    {
        public long ExportRequestId { get; set; }
        public ExportTypeEnum ExportType { get; set; }
        public ExportFormatEnum Format { get; set; }
        public ExportRequestStatusEnum Status { get; set; }
        public string? RequestedByUserName { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string? FileName { get; set; }
        public int? RowCount { get; set; }
        public string? FailureReason { get; set; }
    }

    public class ExportRequestCreateRequest
    {
        public JobApplicationAttributeFilterRequest Filter { get; set; } = new();
        public ExportFormatEnum Format { get; set; } = ExportFormatEnum.Xlsx;
    }

    public class ExportRequestFilterRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public ExportRequestStatusEnum? Status { get; set; }
    }
}
