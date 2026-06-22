using MediatR;
using SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Queries.DocumentTemplateVersionGetAllPaged
{
    public class DocumentTemplateVersionGetAllPagedQuery : IRequest<PagedResult<DocumentTemplateVersionResponse>>
    {
        public PagedRequest Request { get; set; }

        public DocumentTemplateVersionGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
