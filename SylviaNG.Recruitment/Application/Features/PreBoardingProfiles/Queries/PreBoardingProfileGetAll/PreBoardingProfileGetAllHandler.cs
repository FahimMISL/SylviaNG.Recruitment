using MediatR;
using SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Queries.PreBoardingProfileGetAll
{
    public class PreBoardingProfileGetAllHandler : IRequestHandler<PreBoardingProfileGetAllQuery, List<PreBoardingProfileResponse>>
    {
        private readonly IPreBoardingProfileService _service;

        public PreBoardingProfileGetAllHandler(IPreBoardingProfileService service)
        {
            _service = service;
        }

        public async Task<List<PreBoardingProfileResponse>> Handle(PreBoardingProfileGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
