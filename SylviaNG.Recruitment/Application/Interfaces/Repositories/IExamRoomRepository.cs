using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IExamRoomRepository : IRepository<ExamRoom>
    {
        /// <summary>Name uniqueness is scoped to the venue - two venues may each have a "Room 101".</summary>
        Task<bool> ExistsByNameAsync(long examVenueId, string name, long? excludeId = null);

        /// <summary>Full aggregate load: room and its assigned invigilators.</summary>
        Task<ExamRoom?> GetByIdWithInvigilatorsAsync(long examRoomId);
        Task<List<ExamRoom>> GetAllByVenueIdAsync(long examVenueId);
        Task<List<ExamRoom>> GetActiveByVenueIdAsync(long examVenueId);
        Task<int> GetRoomCountByVenueIdAsync(long examVenueId);

        /// <summary>Which of the given employee IDs actually exist, for validating invigilator assignments.</summary>
        Task<HashSet<long>> GetExistingEmployeeIdsAsync(IEnumerable<long> employeeIds);
    }
}
