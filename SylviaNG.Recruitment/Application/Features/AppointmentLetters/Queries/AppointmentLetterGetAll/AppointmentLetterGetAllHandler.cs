using MediatR;
using SylviaNG.Recruitment.Application.Features.AppointmentLetters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AppointmentLetters.Queries.AppointmentLetterGetAll
{
    public class AppointmentLetterGetAllHandler : IRequestHandler<AppointmentLetterGetAllQuery, List<AppointmentLetterResponse>>
    {
        private readonly IAppointmentLetterService _appointmentLetterService;

        public AppointmentLetterGetAllHandler(IAppointmentLetterService appointmentLetterService)
        {
            _appointmentLetterService = appointmentLetterService;
        }

        public async Task<List<AppointmentLetterResponse>> Handle(AppointmentLetterGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _appointmentLetterService.GetAllAsync(query.JobApplicationId);
        }
    }
}
