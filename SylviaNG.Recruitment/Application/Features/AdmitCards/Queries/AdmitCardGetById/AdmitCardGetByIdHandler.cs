using MediatR;
using SylviaNG.Recruitment.Application.Features.AdmitCards.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AdmitCards.Queries.AdmitCardGetById
{
    public class AdmitCardGetByIdHandler : IRequestHandler<AdmitCardGetByIdQuery, AdmitCardResponse>
    {
        private readonly IAdmitCardService _service;

        public AdmitCardGetByIdHandler(IAdmitCardService service)
        {
            _service = service;
        }

        public async Task<AdmitCardResponse> Handle(AdmitCardGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.AdmitCardId);
        }
    }
}
