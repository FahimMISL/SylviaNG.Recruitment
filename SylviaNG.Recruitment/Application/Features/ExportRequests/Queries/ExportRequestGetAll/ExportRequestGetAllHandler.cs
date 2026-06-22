using MediatR;
using SylviaNG.Recruitment.Application.Features.ExportRequests.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExportRequests.Queries.ExportRequestGetAll
{
    public class ExportRequestGetAllHandler : IRequestHandler<ExportRequestGetAllQuery, List<ExportRequestResponse>>
    {
        private readonly IExportRequestService _service;

        public ExportRequestGetAllHandler(IExportRequestService service)
        {
            _service = service;
        }

        public async Task<List<ExportRequestResponse>> Handle(ExportRequestGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
