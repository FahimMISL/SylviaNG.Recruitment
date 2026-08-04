using MediatR;
using SylviaNG.Recruitment.Application.Features.AppointmentLetters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AppointmentLetters.Queries.AppointmentLetterGetById
{
    public class AppointmentLetterGetByIdHandler : IRequestHandler<AppointmentLetterGetByIdQuery, AppointmentLetterResponse>
    {
        private readonly IAppointmentLetterService _appointmentLetterService;

        public AppointmentLetterGetByIdHandler(IAppointmentLetterService appointmentLetterService)
        {
            _appointmentLetterService = appointmentLetterService;
        }

        public async Task<AppointmentLetterResponse> Handle(AppointmentLetterGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _appointmentLetterService.GetByIdAsync(query.AppointmentLetterId);
        }
    }
}
