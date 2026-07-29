using MediatR;
using SylviaNG.Recruitment.Application.Features.BloodGroups.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.BloodGroups.Queries.BloodGroupGetAll
{
    public class BloodGroupGetAllHandler : IRequestHandler<BloodGroupGetAllQuery, List<BloodGroupResponse>>
    {
        private readonly IBloodGroupService _genderService;

        public BloodGroupGetAllHandler(IBloodGroupService genderService)
        {
            _genderService = genderService;
        }

        public async Task<List<BloodGroupResponse>> Handle(BloodGroupGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _genderService.GetAllAsync();
        }
    }
}
