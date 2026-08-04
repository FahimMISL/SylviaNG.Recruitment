using MediatR;
using SylviaNG.Recruitment.Application.Features.AppointmentLetters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AppointmentLetters.Queries.AppointmentLetterGetByIdForCandidate
{
    public class AppointmentLetterGetByIdForCandidateHandler : IRequestHandler<AppointmentLetterGetByIdForCandidateQuery, AppointmentLetterResponse>
    {
        private readonly IAppointmentLetterService _appointmentLetterService;

        public AppointmentLetterGetByIdForCandidateHandler(IAppointmentLetterService appointmentLetterService)
        {
            _appointmentLetterService = appointmentLetterService;
        }

        public async Task<AppointmentLetterResponse> Handle(AppointmentLetterGetByIdForCandidateQuery query, CancellationToken cancellationToken)
        {
            return await _appointmentLetterService.GetByIdForCandidateAsync(query.AppointmentLetterId);
        }
    }
}
