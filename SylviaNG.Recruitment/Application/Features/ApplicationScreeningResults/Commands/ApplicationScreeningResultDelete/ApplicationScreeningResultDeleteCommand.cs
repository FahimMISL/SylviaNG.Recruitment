using MediatR;

namespace SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Commands.ApplicationScreeningResultDelete
{
    public class ApplicationScreeningResultDeleteCommand : IRequest<Unit>
    {
        public long ApplicationScreeningResultId { get; set; }

        public ApplicationScreeningResultDeleteCommand(long applicationScreeningResultId)
        {
            ApplicationScreeningResultId = applicationScreeningResultId;
        }
    }
}
