using MediatR;
using SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Queries.ProfileFieldConfigGetAllPaged
{
    public class ProfileFieldConfigGetAllPagedHandler : IRequestHandler<ProfileFieldConfigGetAllPagedQuery, PagedResult<ProfileFieldConfigResponse>>
    {
        private readonly IProfileFieldConfigService _service;

        public ProfileFieldConfigGetAllPagedHandler(IProfileFieldConfigService service)
        {
            _service = service;
        }

        public async Task<PagedResult<ProfileFieldConfigResponse>> Handle(ProfileFieldConfigGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetPaginatedAsync(query.Request);
        }
    }
}
