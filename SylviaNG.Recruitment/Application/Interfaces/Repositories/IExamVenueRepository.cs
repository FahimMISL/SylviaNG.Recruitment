using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IExamVenueRepository : IRepository<ExamVenue>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);

        /// <summary>Full aggregate load: venue and its rooms.</summary>
        Task<ExamVenue?> GetByIdWithRoomsAsync(long examVenueId);
        Task<List<ExamVenue>> GetAllWithRoomsAsync();

        /// <summary>Active venues only (Id + Name), for a future "pick a venue" dropdown (seat plan).</summary>
        Task<List<ExamVenue>> GetActiveAsync();
    }
}
