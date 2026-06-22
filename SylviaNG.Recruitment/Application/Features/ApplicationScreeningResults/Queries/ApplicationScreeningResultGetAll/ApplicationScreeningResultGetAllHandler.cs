using MediatR;
using SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Queries.ApplicationScreeningResultGetAll
{
    public class ApplicationScreeningResultGetAllHandler : IRequestHandler<ApplicationScreeningResultGetAllQuery, List<ApplicationScreeningResultResponse>>
    {
        private readonly IApplicationScreeningResultService _service;

        public ApplicationScreeningResultGetAllHandler(IApplicationScreeningResultService service)
        {
            _service = service;
        }

        public async Task<List<ApplicationScreeningResultResponse>> Handle(ApplicationScreeningResultGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
