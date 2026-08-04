using MediatR;
using SylviaNG.Recruitment.Application.Features.AppointmentLetters.Models;

namespace SylviaNG.Recruitment.Application.Features.AppointmentLetters.Queries.AppointmentLetterGetByIdForCandidate
{
    public class AppointmentLetterGetByIdForCandidateQuery : IRequest<AppointmentLetterResponse>
    {
        public long AppointmentLetterId { get; set; }

        public AppointmentLetterGetByIdForCandidateQuery(long appointmentLetterId)
        {
            AppointmentLetterId = appointmentLetterId;
        }
    }
}
