using MediatR;
using SylviaNG.Recruitment.Application.Features.AppointmentLetters.Models;

namespace SylviaNG.Recruitment.Application.Features.AppointmentLetters.Queries.AppointmentLetterGetAllForCandidate
{
    public class AppointmentLetterGetAllForCandidateQuery : IRequest<List<AppointmentLetterResponse>>
    {
    }
}
