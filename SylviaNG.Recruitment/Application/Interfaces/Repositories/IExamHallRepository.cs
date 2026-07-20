using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IExamHallRepository : IRepository<ExamHall>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);

        /// <summary>Full aggregate load: hall and its assigned invigilators.</summary>
        Task<ExamHall?> GetByIdWithInvigilatorsAsync(long examHallId);
        Task<List<ExamHall>> GetAllWithInvigilatorsAsync();

        /// <summary>Active halls only (Id + Name), for a future "pick an exam hall" dropdown (seat plan).</summary>
        Task<List<ExamHall>> GetActiveAsync();

        /// <summary>Which of the given employee IDs actually exist, for validating invigilator assignments.</summary>
        Task<HashSet<long>> GetExistingEmployeeIdsAsync(IEnumerable<long> employeeIds);
    }
}
