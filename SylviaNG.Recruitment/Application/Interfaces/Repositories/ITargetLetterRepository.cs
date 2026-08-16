using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface ITargetLetterRepository : IRepository<TargetLetter>
    {
        Task<List<TargetLetter>> GetAllOrderedAsync(long? jobApplicationId);
        Task<TargetLetter?> GetByIdWithDetailsAsync(long targetLetterId);
    }
}
