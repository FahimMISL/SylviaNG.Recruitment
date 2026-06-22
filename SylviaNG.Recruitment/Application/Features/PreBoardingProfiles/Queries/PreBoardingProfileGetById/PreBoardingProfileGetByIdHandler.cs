using MediatR;
using SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Queries.PreBoardingProfileGetById
{
    public class PreBoardingProfileGetByIdHandler : IRequestHandler<PreBoardingProfileGetByIdQuery, PreBoardingProfileResponse>
    {
        private readonly IPreBoardingProfileService _service;

        public PreBoardingProfileGetByIdHandler(IPreBoardingProfileService service)
        {
            _service = service;
        }

        public async Task<PreBoardingProfileResponse> Handle(PreBoardingProfileGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.PreBoardingProfileId);
        }
    }
}
