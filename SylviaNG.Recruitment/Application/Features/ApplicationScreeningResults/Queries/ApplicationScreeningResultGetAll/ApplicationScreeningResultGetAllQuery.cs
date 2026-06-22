using MediatR;
using SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Models;

namespace SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Queries.ApplicationScreeningResultGetAll
{
    public class ApplicationScreeningResultGetAllQuery : IRequest<List<ApplicationScreeningResultResponse>>
    {
    }
}
