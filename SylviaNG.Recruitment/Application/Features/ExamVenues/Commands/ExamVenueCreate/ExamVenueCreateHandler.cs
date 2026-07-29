using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamVenues.Commands.ExamVenueCreate
{
    public class ExamVenueCreateHandler : IRequestHandler<ExamVenueCreateCommand, long>
    {
        private readonly IExamVenueService _examVenueService;

        public ExamVenueCreateHandler(IExamVenueService examVenueService)
        {
            _examVenueService = examVenueService;
        }

        public async Task<long> Handle(ExamVenueCreateCommand command, CancellationToken cancellationToken)
        {
            return await _examVenueService.CreateAsync(command.Request);
        }
    }
}
