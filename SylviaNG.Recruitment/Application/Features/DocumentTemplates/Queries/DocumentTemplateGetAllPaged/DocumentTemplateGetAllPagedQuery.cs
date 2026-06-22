using MediatR;
using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplates.Queries.DocumentTemplateGetAllPaged
{
    public class DocumentTemplateGetAllPagedQuery : IRequest<PagedResult<DocumentTemplateResponse>>
    {
        public PagedRequest Request { get; set; }

        public DocumentTemplateGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
