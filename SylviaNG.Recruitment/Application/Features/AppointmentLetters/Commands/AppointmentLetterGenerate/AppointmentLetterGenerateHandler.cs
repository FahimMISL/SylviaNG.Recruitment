using MediatR;
using SylviaNG.Recruitment.Application.Features.AppointmentLetters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AppointmentLetters.Commands.AppointmentLetterGenerate
{
    public class AppointmentLetterGenerateHandler : IRequestHandler<AppointmentLetterGenerateCommand, AppointmentLetterResponse>
    {
        private readonly IAppointmentLetterService _appointmentLetterService;

        public AppointmentLetterGenerateHandler(IAppointmentLetterService appointmentLetterService)
        {
            _appointmentLetterService = appointmentLetterService;
        }

        public async Task<AppointmentLetterResponse> Handle(AppointmentLetterGenerateCommand command, CancellationToken cancellationToken)
        {
            return await _appointmentLetterService.GenerateAsync(command.Request);
        }
    }
}
