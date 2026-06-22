using MediatR;
using SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Queries.ProfileFieldConfigGetAll
{
    public class ProfileFieldConfigGetAllHandler : IRequestHandler<ProfileFieldConfigGetAllQuery, List<ProfileFieldConfigResponse>>
    {
        private readonly IProfileFieldConfigService _service;

        public ProfileFieldConfigGetAllHandler(IProfileFieldConfigService service)
        {
            _service = service;
        }

        public async Task<List<ProfileFieldConfigResponse>> Handle(ProfileFieldConfigGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
