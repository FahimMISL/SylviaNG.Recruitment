using MediatR;
using SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Queries.ApplicationScreeningResultGetById
{
    public class ApplicationScreeningResultGetByIdHandler : IRequestHandler<ApplicationScreeningResultGetByIdQuery, ApplicationScreeningResultResponse>
    {
        private readonly IApplicationScreeningResultService _service;

        public ApplicationScreeningResultGetByIdHandler(IApplicationScreeningResultService service)
        {
            _service = service;
        }

        public async Task<ApplicationScreeningResultResponse> Handle(ApplicationScreeningResultGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.ApplicationScreeningResultId);
        }
    }
}
