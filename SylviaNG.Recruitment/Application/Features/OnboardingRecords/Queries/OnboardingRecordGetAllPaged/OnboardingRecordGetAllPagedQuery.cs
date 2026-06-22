using MediatR;
using SylviaNG.Recruitment.Application.Features.OnboardingRecords.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.OnboardingRecords.Queries.OnboardingRecordGetAllPaged
{
    public class OnboardingRecordGetAllPagedQuery : IRequest<PagedResult<OnboardingRecordResponse>>
    {
        public PagedRequest Request { get; set; }

        public OnboardingRecordGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
