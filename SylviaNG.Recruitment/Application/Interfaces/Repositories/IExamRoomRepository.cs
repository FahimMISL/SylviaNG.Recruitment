using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IExamRoomRepository : IRepository<ExamRoom>
    {
        /// <summary>Name uniqueness is scoped to the venue - two venues may each have a "Room 101".</summary>
        Task<bool> ExistsByNameAsync(long examVenueId, string name, long? excludeId = null);

        Task<List<ExamRoom>> GetAllByVenueIdAsync(long examVenueId);
        Task<List<ExamRoom>> GetActiveByVenueIdAsync(long examVenueId);
        Task<int> GetRoomCountByVenueIdAsync(long examVenueId);
    }
}
