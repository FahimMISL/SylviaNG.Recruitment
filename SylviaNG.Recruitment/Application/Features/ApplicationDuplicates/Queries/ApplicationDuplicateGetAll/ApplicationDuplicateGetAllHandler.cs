using MediatR;
using SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Queries.ApplicationDuplicateGetAll
{
    public class ApplicationDuplicateGetAllHandler : IRequestHandler<ApplicationDuplicateGetAllQuery, List<ApplicationDuplicateResponse>>
    {
        private readonly IApplicationDuplicateService _service;

        public ApplicationDuplicateGetAllHandler(IApplicationDuplicateService service)
        {
            _service = service;
        }

        public async Task<List<ApplicationDuplicateResponse>> Handle(ApplicationDuplicateGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
