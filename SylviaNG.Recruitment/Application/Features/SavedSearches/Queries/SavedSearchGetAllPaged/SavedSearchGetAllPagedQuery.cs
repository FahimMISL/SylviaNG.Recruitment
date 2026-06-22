using MediatR;
using SylviaNG.Recruitment.Application.Features.SavedSearches.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.SavedSearches.Queries.SavedSearchGetAllPaged
{
    public class SavedSearchGetAllPagedQuery : IRequest<PagedResult<SavedSearchResponse>>
    {
        public PagedRequest Request { get; set; }

        public SavedSearchGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
