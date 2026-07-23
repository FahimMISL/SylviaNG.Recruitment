using SylviaNG.Recruitment.Application.Features.Interviews.Models;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IInterviewService
    {
        Task<long> ScheduleAsync(InterviewScheduleRequest request);
        Task<List<long>> BulkScheduleAsync(InterviewBulkScheduleRequest request);
        Task RescheduleAsync(long interviewId, InterviewRescheduleRequest request);
        Task BulkRescheduleAsync(InterviewBulkRescheduleRequest request);
        Task CancelAsync(long interviewId, InterviewCancelRequest request);
        Task BulkCancelAsync(InterviewBulkCancelRequest request);
        Task MarkResultAsync(long interviewId, InterviewMarkResultRequest request);

        Task<PagedResult<InterviewResponse>> GetPagedAsync(
            PagedRequest request,
            long? jobPostingId,
            InterviewStatusEnum? status,
            DateTime? dateFrom,
            DateTime? dateTo);

        Task<InterviewResponse> GetByIdAsync(long interviewId);
        Task<List<InterviewResponse>> GetByJobApplicationIdAsync(long jobApplicationId);
    }
}
