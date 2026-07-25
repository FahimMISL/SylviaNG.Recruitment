using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IInterviewVenueRepository : IRepository<InterviewVenue>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);

        /// <summary>Full aggregate load: venue and its rooms.</summary>
        Task<InterviewVenue?> GetByIdWithRoomsAsync(long interviewVenueId);
        Task<List<InterviewVenue>> GetAllWithRoomsAsync();

        /// <summary>Active venues only, for a "pick a venue" dropdown.</summary>
        Task<List<InterviewVenue>> GetActiveAsync();
    }
}
