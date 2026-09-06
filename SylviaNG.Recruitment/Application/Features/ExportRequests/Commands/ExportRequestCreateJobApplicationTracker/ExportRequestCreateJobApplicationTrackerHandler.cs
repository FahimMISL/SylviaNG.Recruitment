using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExportRequests.Commands.ExportRequestCreateJobApplicationTracker
{
    public class ExportRequestCreateJobApplicationTrackerHandler : IRequestHandler<ExportRequestCreateJobApplicationTrackerCommand, long>
    {
        private readonly IExportRequestService _exportRequestService;

        public ExportRequestCreateJobApplicationTrackerHandler(IExportRequestService exportRequestService)
        {
            _exportRequestService = exportRequestService;
        }

        public async Task<long> Handle(ExportRequestCreateJobApplicationTrackerCommand command, CancellationToken cancellationToken)
        {
            return await _exportRequestService.RequestJobApplicationTrackerExportAsync(command.Request.Filter, command.Request.Format);
        }
    }
}
