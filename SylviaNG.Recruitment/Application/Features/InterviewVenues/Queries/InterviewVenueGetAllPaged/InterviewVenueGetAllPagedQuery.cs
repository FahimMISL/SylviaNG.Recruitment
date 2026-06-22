using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewVenues.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.InterviewVenues.Queries.InterviewVenueGetAllPaged
{
    public class InterviewVenueGetAllPagedQuery : IRequest<PagedResult<InterviewVenueResponse>>
    {
        public PagedRequest Request { get; set; }

        public InterviewVenueGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
