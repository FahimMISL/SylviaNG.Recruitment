using MediatR;
using SylviaNG.Recruitment.Application.Features.CvBank.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.CvBank.Queries.CvBankSearch
{
    public class CvBankSearchQuery : IRequest<PagedResult<CvBankSearchResultResponse>>
    {
        public CvBankSearchRequest Request { get; }

        public CvBankSearchQuery(CvBankSearchRequest request)
        {
            Request = request;
        }
    }
}
