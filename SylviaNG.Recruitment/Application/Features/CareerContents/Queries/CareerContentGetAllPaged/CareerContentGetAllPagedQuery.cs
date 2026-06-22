using MediatR;
using SylviaNG.Recruitment.Application.Features.CareerContents.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.CareerContents.Queries.CareerContentGetAllPaged
{
    public class CareerContentGetAllPagedQuery : IRequest<PagedResult<CareerContentResponse>>
    {
        public PagedRequest Request { get; set; }

        public CareerContentGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
