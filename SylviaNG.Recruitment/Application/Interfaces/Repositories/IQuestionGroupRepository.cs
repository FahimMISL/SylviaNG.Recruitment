using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IQuestionGroupRepository : IRepository<QuestionGroup>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);

        /// <summary>Active groups only (Id + Name), for a "pick a question group" dropdown.</summary>
        Task<List<QuestionGroup>> GetActiveAsync();
    }
}
