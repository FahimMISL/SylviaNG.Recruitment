using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IOfficeNoteRepository : IRepository<OfficeNote>
    {
        Task<List<OfficeNote>> GetAllOrderedAsync(long? jobApplicationId);
        Task<OfficeNote?> GetByIdWithDetailsAsync(long officeNoteId);
    }
}
