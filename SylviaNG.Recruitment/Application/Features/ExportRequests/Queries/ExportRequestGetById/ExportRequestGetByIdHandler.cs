using MediatR;
using SylviaNG.Recruitment.Application.Features.ExportRequests.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExportRequests.Queries.ExportRequestGetById
{
    public class ExportRequestGetByIdHandler : IRequestHandler<ExportRequestGetByIdQuery, ExportRequestResponse>
    {
        private readonly IExportRequestService _service;

        public ExportRequestGetByIdHandler(IExportRequestService service)
        {
            _service = service;
        }

        public async Task<ExportRequestResponse> Handle(ExportRequestGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.ExportRequestId);
        }
    }
}
