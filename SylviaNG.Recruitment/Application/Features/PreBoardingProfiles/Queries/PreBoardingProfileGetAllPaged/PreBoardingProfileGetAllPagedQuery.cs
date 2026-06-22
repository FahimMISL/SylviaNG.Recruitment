using MediatR;
using SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Queries.PreBoardingProfileGetAllPaged
{
    public class PreBoardingProfileGetAllPagedQuery : IRequest<PagedResult<PreBoardingProfileResponse>>
    {
        public PagedRequest Request { get; set; }

        public PreBoardingProfileGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
