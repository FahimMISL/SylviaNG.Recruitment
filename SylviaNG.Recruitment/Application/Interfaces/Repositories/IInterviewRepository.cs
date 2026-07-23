using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IInterviewRepository : IRepository<Interview>
    {
        Task<PagedResult<Interview>> GetPagedAsync(
            PagedRequest request,
            long? jobPostingId,
            InterviewStatusEnum? status,
            DateTime? dateFrom,
            DateTime? dateTo);

        /// <summary>Single interview with JobApplication/Venue/Room/PanelMembers(+Employee) included.</summary>
        Task<Interview?> GetByIdWithDetailsAsync(long interviewId);

        Task<List<Interview>> GetByIdsWithDetailsAsync(List<long> interviewIds);

        /// <summary>Every interview scheduled for one job application, newest first - feeds the
        /// candidate's pipeline-progress tracker.</summary>
        Task<List<Interview>> GetByJobApplicationIdAsync(long jobApplicationId);

        /// <summary>Count of non-cancelled interviews in this room whose time range overlaps
        /// [start, end) - the room-capacity/double-booking conflict check.</summary>
        Task<int> GetOverlappingCountByRoomIdAsync(long interviewRoomId, DateTime start, DateTime end, long? excludeInterviewId = null);

        /// <summary>Of the given employee IDs, which already have a non-cancelled interview whose
        /// time range overlaps [start, end) - the panelist double-booking conflict check.</summary>
        Task<List<long>> GetConflictingPanelistEmployeeIdsAsync(List<long> employeeIds, DateTime start, DateTime end, long? excludeInterviewId = null);
    }
}
