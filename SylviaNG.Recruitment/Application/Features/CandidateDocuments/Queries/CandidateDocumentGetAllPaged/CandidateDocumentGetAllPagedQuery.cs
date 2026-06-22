using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateDocuments.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.CandidateDocuments.Queries.CandidateDocumentGetAllPaged
{
    public class CandidateDocumentGetAllPagedQuery : IRequest<PagedResult<CandidateDocumentResponse>>
    {
        public PagedRequest Request { get; set; }

        public CandidateDocumentGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
