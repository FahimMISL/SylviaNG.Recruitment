using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamVenues.Commands.ExamVenueSetActiveStatus
{
    public class ExamVenueSetActiveStatusHandler : IRequestHandler<ExamVenueSetActiveStatusCommand>
    {
        private readonly IExamVenueService _examVenueService;

        public ExamVenueSetActiveStatusHandler(IExamVenueService examVenueService)
        {
            _examVenueService = examVenueService;
        }

        public async Task Handle(ExamVenueSetActiveStatusCommand command, CancellationToken cancellationToken)
        {
            await _examVenueService.SetActiveStatusAsync(command.ExamVenueId, command.IsActive);
        }
    }
}
