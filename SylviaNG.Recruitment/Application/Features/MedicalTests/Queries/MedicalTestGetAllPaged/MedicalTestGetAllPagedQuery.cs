using MediatR;
using SylviaNG.Recruitment.Application.Features.MedicalTests.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.MedicalTests.Queries.MedicalTestGetAllPaged
{
    public class MedicalTestGetAllPagedQuery : IRequest<PagedResult<MedicalTestResponse>>
    {
        public PagedRequest Request { get; set; }

        public MedicalTestGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
