using MediatR;
using SylviaNG.Recruitment.Application.Features.VerificationItems.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.VerificationItems.Queries.VerificationItemGetAll
{
    public class VerificationItemGetAllHandler : IRequestHandler<VerificationItemGetAllQuery, List<VerificationItemResponse>>
    {
        private readonly IVerificationItemService _service;

        public VerificationItemGetAllHandler(IVerificationItemService service)
        {
            _service = service;
        }

        public async Task<List<VerificationItemResponse>> Handle(VerificationItemGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
