using MediatR;
using SylviaNG.Recruitment.Application.Features.FitmentDatas.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.FitmentDatas.Queries.FitmentDataGetAllPaged
{
    public class FitmentDataGetAllPagedQuery : IRequest<PagedResult<FitmentDataResponse>>
    {
        public PagedRequest Request { get; set; }

        public FitmentDataGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
