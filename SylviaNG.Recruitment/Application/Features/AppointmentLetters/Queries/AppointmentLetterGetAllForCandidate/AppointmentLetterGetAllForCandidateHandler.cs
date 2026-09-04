using MediatR;
using SylviaNG.Recruitment.Application.Features.AppointmentLetters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AppointmentLetters.Queries.AppointmentLetterGetAllForCandidate
{
    public class AppointmentLetterGetAllForCandidateHandler : IRequestHandler<AppointmentLetterGetAllForCandidateQuery, List<AppointmentLetterResponse>>
    {
        private readonly IAppointmentLetterService _appointmentLetterService;

        public AppointmentLetterGetAllForCandidateHandler(IAppointmentLetterService appointmentLetterService)
        {
            _appointmentLetterService = appointmentLetterService;
        }

        public async Task<List<AppointmentLetterResponse>> Handle(AppointmentLetterGetAllForCandidateQuery query, CancellationToken cancellationToken)
        {
            return await _appointmentLetterService.GetAllForCandidateAsync();
        }
    }
}
