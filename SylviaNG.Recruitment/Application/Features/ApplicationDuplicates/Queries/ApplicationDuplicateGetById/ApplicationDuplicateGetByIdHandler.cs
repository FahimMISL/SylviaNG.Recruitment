using MediatR;
using SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Queries.ApplicationDuplicateGetById
{
    public class ApplicationDuplicateGetByIdHandler : IRequestHandler<ApplicationDuplicateGetByIdQuery, ApplicationDuplicateResponse>
    {
        private readonly IApplicationDuplicateService _service;

        public ApplicationDuplicateGetByIdHandler(IApplicationDuplicateService service)
        {
            _service = service;
        }

        public async Task<ApplicationDuplicateResponse> Handle(ApplicationDuplicateGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.ApplicationDuplicateId);
        }
    }
}
