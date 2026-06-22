using MediatR;
using SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Queries.DocumentAcceptanceGetAllPaged
{
    public class DocumentAcceptanceGetAllPagedQuery : IRequest<PagedResult<DocumentAcceptanceResponse>>
    {
        public PagedRequest Request { get; set; }

        public DocumentAcceptanceGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
