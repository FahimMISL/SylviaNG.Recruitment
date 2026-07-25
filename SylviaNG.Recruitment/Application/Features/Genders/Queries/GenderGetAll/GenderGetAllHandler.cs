using MediatR;
using SylviaNG.Recruitment.Application.Features.Genders.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Genders.Queries.GenderGetAll
{
    public class GenderGetAllHandler : IRequestHandler<GenderGetAllQuery, List<GenderResponse>>
    {
        private readonly IGenderService _genderService;

        public GenderGetAllHandler(IGenderService genderService)
        {
            _genderService = genderService;
        }

        public async Task<List<GenderResponse>> Handle(GenderGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _genderService.GetAllAsync();
        }
    }
}
