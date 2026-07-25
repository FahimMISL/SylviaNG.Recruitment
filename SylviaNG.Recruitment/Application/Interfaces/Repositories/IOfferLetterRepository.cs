using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IOfferLetterRepository : IRepository<OfferLetter>
    {
        Task<List<OfferLetter>> GetAllOrderedAsync(long? jobApplicationId);
        Task<OfferLetter?> GetByIdWithDetailsAsync(long offerLetterId);
    }
}
