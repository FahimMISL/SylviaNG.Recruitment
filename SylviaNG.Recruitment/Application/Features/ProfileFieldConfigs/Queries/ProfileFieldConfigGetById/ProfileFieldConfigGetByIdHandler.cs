using MediatR;
using SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Queries.ProfileFieldConfigGetById
{
    public class ProfileFieldConfigGetByIdHandler : IRequestHandler<ProfileFieldConfigGetByIdQuery, ProfileFieldConfigResponse>
    {
        private readonly IProfileFieldConfigService _service;

        public ProfileFieldConfigGetByIdHandler(IProfileFieldConfigService service)
        {
            _service = service;
        }

        public async Task<ProfileFieldConfigResponse> Handle(ProfileFieldConfigGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.Id);
        }
    }
}
