using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.DocumentTracking.Commands.DocumentTrackingFollowUp
{
    public class DocumentTrackingFollowUpHandler : IRequestHandler<DocumentTrackingFollowUpCommand, Unit>
    {
        private readonly IDocumentTrackingService _documentTrackingService;

        public DocumentTrackingFollowUpHandler(IDocumentTrackingService documentTrackingService)
        {
            _documentTrackingService = documentTrackingService;
        }

        public async Task<Unit> Handle(DocumentTrackingFollowUpCommand command, CancellationToken cancellationToken)
        {
            await _documentTrackingService.FollowUpAsync(command.DocumentType, command.SourceId);
            return Unit.Value;
        }
    }
}
