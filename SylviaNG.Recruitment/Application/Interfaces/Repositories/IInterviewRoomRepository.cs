using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IInterviewRoomRepository : IRepository<InterviewRoom>
    {
        /// <summary>Name uniqueness is scoped to the venue - two venues may each have a "Room A".</summary>
        Task<bool> ExistsByNameAsync(long interviewVenueId, string name, long? excludeId = null);

        Task<List<InterviewRoom>> GetAllByVenueIdAsync(long interviewVenueId);
        Task<List<InterviewRoom>> GetActiveByVenueIdAsync(long interviewVenueId);
        Task<int> GetRoomCountByVenueIdAsync(long interviewVenueId);
    }
}
