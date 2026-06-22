using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.SavedReports.Commands.SavedReportCreate
{
    public class SavedReportCreateHandler : IRequestHandler<SavedReportCreateCommand, long>
    {
        private readonly ISavedReportService _service;

        public SavedReportCreateHandler(ISavedReportService service)
        {
            _service = service;
        }

        public async Task<long> Handle(SavedReportCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
