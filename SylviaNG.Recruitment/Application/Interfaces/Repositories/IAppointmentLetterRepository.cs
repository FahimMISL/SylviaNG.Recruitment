using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IAppointmentLetterRepository : IRepository<AppointmentLetter>
    {
        Task<List<AppointmentLetter>> GetAllOrderedAsync(long? jobApplicationId);
        Task<AppointmentLetter?> GetByIdWithDetailsAsync(long appointmentLetterId);
    }
}
