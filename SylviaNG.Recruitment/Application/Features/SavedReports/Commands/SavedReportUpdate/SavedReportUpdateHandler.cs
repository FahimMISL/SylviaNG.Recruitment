using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.SavedReports.Commands.SavedReportUpdate
{
    public class SavedReportUpdateHandler : IRequestHandler<SavedReportUpdateCommand, Unit>
    {
        private readonly ISavedReportService _service;

        public SavedReportUpdateHandler(ISavedReportService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(SavedReportUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.SavedReportId, command.Request);
            return Unit.Value;
        }
    }
}
