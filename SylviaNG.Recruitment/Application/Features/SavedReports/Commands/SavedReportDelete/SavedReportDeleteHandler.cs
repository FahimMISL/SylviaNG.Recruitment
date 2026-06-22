using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.SavedReports.Commands.SavedReportDelete
{
    public class SavedReportDeleteHandler : IRequestHandler<SavedReportDeleteCommand, Unit>
    {
        private readonly ISavedReportService _service;

        public SavedReportDeleteHandler(ISavedReportService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(SavedReportDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.SavedReportId);
            return Unit.Value;
        }
    }
}
