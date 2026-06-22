using MediatR;
using SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Models;

namespace SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Queries.ApplicationScreeningResultGetById
{
    public class ApplicationScreeningResultGetByIdQuery : IRequest<ApplicationScreeningResultResponse>
    {
        public long ApplicationScreeningResultId { get; set; }

        public ApplicationScreeningResultGetByIdQuery(long applicationScreeningResultId)
        {
            ApplicationScreeningResultId = applicationScreeningResultId;
        }
    }
}
