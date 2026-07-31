using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IMedicalLetterRepository : IRepository<MedicalLetter>
    {
        Task<List<MedicalLetter>> GetAllOrderedAsync(long? jobApplicationId);
        Task<MedicalLetter?> GetByIdWithDetailsAsync(long medicalLetterId);
    }
}
