using MediatR;
using SylviaNG.Recruitment.Application.Features.AppointmentLetters.Models;

namespace SylviaNG.Recruitment.Application.Features.AppointmentLetters.Commands.AppointmentLetterGenerate
{
    public class AppointmentLetterGenerateCommand : IRequest<AppointmentLetterResponse>
    {
        public AppointmentLetterGenerateRequest Request { get; set; }

        public AppointmentLetterGenerateCommand(AppointmentLetterGenerateRequest request)
        {
            Request = request;
        }
    }
}
