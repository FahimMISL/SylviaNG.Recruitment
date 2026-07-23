using MediatR;
using SylviaNG.Recruitment.Application.Features.Religions.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Religions.Queries.ReligionGetAll
{
    public class ReligionGetAllHandler : IRequestHandler<ReligionGetAllQuery, List<ReligionResponse>>
    {
        private readonly IReligionService _genderService;

        public ReligionGetAllHandler(IReligionService genderService)
        {
            _genderService = genderService;
        }

        public async Task<List<ReligionResponse>> Handle(ReligionGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _genderService.GetAllAsync();
        }
    }
}
