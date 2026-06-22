using MediatR;
using SylviaNG.Recruitment.Application.Features.AdmitCards.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AdmitCards.Queries.AdmitCardGetAll
{
    public class AdmitCardGetAllHandler : IRequestHandler<AdmitCardGetAllQuery, List<AdmitCardResponse>>
    {
        private readonly IAdmitCardService _service;

        public AdmitCardGetAllHandler(IAdmitCardService service)
        {
            _service = service;
        }

        public async Task<List<AdmitCardResponse>> Handle(AdmitCardGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
