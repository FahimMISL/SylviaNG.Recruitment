using MediatR;
using SylviaNG.Recruitment.Application.Features.AppointmentLetters.Models;

namespace SylviaNG.Recruitment.Application.Features.AppointmentLetters.Queries.AppointmentLetterGetAll
{
    public class AppointmentLetterGetAllQuery : IRequest<List<AppointmentLetterResponse>>
    {
        public long? JobApplicationId { get; set; }

        public AppointmentLetterGetAllQuery(long? jobApplicationId)
        {
            JobApplicationId = jobApplicationId;
        }
    }
}
