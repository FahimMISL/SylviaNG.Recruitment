using MediatR;
using SylviaNG.Recruitment.Application.Features.VerificationItems.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.VerificationItems.Queries.VerificationItemGetById
{
    public class VerificationItemGetByIdHandler : IRequestHandler<VerificationItemGetByIdQuery, VerificationItemResponse>
    {
        private readonly IVerificationItemService _service;

        public VerificationItemGetByIdHandler(IVerificationItemService service)
        {
            _service = service;
        }

        public async Task<VerificationItemResponse> Handle(VerificationItemGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.VerificationItemId);
        }
    }
}
