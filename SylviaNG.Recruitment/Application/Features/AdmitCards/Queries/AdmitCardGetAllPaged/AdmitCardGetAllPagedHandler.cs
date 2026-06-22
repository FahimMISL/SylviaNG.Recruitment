using MediatR;
using SylviaNG.Recruitment.Application.Features.AdmitCards.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.AdmitCards.Queries.AdmitCardGetAllPaged
{
    public class AdmitCardGetAllPagedHandler : IRequestHandler<AdmitCardGetAllPagedQuery, PagedResult<AdmitCardResponse>>
    {
        private readonly IAdmitCardService _admitCardService;

        public AdmitCardGetAllPagedHandler(IAdmitCardService admitCardService)
        {
            _admitCardService = admitCardService;
        }

        public async Task<PagedResult<AdmitCardResponse>> Handle(AdmitCardGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _admitCardService.GetPaginatedAsync(query.Request);
        }
    }
}
