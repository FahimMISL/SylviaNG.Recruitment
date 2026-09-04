using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IQuestionGroupRepository : IRepository<QuestionGroup>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);

        /// <summary>Returns every group with the most recently created group first.</summary>
        Task<List<QuestionGroup>> GetAllNewestFirstAsync();

        /// <summary>Active groups only (Id + Name), for a "pick a question group" dropdown.</summary>
        Task<List<QuestionGroup>> GetActiveAsync();

        /// <summary>Every group matching one of the given ids - used to validate + attach a
        /// multi-select of groups onto an Exam (US-055).</summary>
        Task<List<QuestionGroup>> GetByIdsAsync(IReadOnlyList<long> questionGroupIds);
    }
}
