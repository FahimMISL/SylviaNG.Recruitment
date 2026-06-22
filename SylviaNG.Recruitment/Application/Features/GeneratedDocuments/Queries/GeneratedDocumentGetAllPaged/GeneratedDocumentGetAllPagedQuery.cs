using MediatR;
using SylviaNG.Recruitment.Application.Features.GeneratedDocuments.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.GeneratedDocuments.Queries.GeneratedDocumentGetAllPaged
{
    public class GeneratedDocumentGetAllPagedQuery : IRequest<PagedResult<GeneratedDocumentResponse>>
    {
        public PagedRequest Request { get; set; }

        public GeneratedDocumentGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
