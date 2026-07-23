using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamVenues.Commands.ExamVenueUpdate
{
    public class ExamVenueUpdateHandler : IRequestHandler<ExamVenueUpdateCommand>
    {
        private readonly IExamVenueService _examVenueService;

        public ExamVenueUpdateHandler(IExamVenueService examVenueService)
        {
            _examVenueService = examVenueService;
        }

        public async Task Handle(ExamVenueUpdateCommand command, CancellationToken cancellationToken)
        {
            await _examVenueService.UpdateAsync(command.ExamVenueId, command.Request);
        }
    }
}
