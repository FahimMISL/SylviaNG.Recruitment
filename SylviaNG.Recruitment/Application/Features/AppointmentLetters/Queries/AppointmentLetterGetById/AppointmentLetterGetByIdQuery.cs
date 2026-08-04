using MediatR;
using SylviaNG.Recruitment.Application.Features.AppointmentLetters.Models;

namespace SylviaNG.Recruitment.Application.Features.AppointmentLetters.Queries.AppointmentLetterGetById
{
    public class AppointmentLetterGetByIdQuery : IRequest<AppointmentLetterResponse>
    {
        public long AppointmentLetterId { get; set; }

        public AppointmentLetterGetByIdQuery(long appointmentLetterId)
        {
            AppointmentLetterId = appointmentLetterId;
        }
    }
}
