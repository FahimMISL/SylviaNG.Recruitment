using SylviaNG.Recruitment.Application.Features.ExportRequests.Models;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public record ExportRequestFileResponse(byte[] Content, string ContentType, string FileName);

    /// <summary>EP-13 US-100/104: queues candidate-list export requests and serves the completed
    /// results. Actual file generation happens out-of-band in ExportRequestWorker.</summary>
    public interface IExportRequestService
    {
        /// <summary>Resolves the matching JobApplicationIds now (cheap, same path the ATS dashboard
        /// already uses - also validates the filter, so bad input fails fast instead of surfacing
        /// only once the worker picks the row up) and queues a Pending row for the worker to render.</summary>
        Task<long> RequestCandidateListExportAsync(JobApplicationAttributeFilterRequest filter, ExportFormatEnum format);

        Task<PagedResult<ExportRequestResponse>> GetPagedAsync(ExportRequestFilterRequest filter);

        /// <summary>Throws NotFoundException if unknown, ValidationException if not yet Completed.</summary>
        Task<ExportRequestFileResponse> GetForDownloadAsync(long exportRequestId);
    }
}
