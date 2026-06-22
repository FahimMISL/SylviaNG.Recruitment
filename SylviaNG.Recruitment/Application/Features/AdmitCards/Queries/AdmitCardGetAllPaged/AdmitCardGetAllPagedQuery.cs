using MediatR;
using SylviaNG.Recruitment.Application.Features.AdmitCards.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.AdmitCards.Queries.AdmitCardGetAllPaged
{
    public class AdmitCardGetAllPagedQuery : IRequest<PagedResult<AdmitCardResponse>>
    {
        public PagedRequest Request { get; set; }

        public AdmitCardGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
