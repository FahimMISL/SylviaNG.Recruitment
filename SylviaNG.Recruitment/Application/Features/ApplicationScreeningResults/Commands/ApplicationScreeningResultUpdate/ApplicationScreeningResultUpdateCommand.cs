using MediatR;
using SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Models;

namespace SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Commands.ApplicationScreeningResultUpdate
{
    public class ApplicationScreeningResultUpdateCommand : IRequest<Unit>
    {
        public long ApplicationScreeningResultId { get; set; }
        public ApplicationScreeningResultUpdateRequest Request { get; set; }

        public ApplicationScreeningResultUpdateCommand(long applicationScreeningResultId, ApplicationScreeningResultUpdateRequest request)
        {
            ApplicationScreeningResultId = applicationScreeningResultId;
            Request = request;
        }
    }
}
