using MediatR;
using SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Queries.PreBoardingProfileGetAllPaged
{
    public class PreBoardingProfileGetAllPagedHandler : IRequestHandler<PreBoardingProfileGetAllPagedQuery, PagedResult<PreBoardingProfileResponse>>
    {
        private readonly IPreBoardingProfileService _preBoardingProfileService;

        public PreBoardingProfileGetAllPagedHandler(IPreBoardingProfileService preBoardingProfileService)
        {
            _preBoardingProfileService = preBoardingProfileService;
        }

        public async Task<PagedResult<PreBoardingProfileResponse>> Handle(PreBoardingProfileGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _preBoardingProfileService.GetPaginatedAsync(query.Request);
        }
    }
}
