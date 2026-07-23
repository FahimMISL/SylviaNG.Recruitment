using MediatR;
using SylviaNG.Recruitment.Application.Features.MaritalStatuses.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.MaritalStatuses.Queries.MaritalStatusGetAll
{
    public class MaritalStatusGetAllHandler : IRequestHandler<MaritalStatusGetAllQuery, List<MaritalStatusResponse>>
    {
        private readonly IMaritalStatusService _genderService;

        public MaritalStatusGetAllHandler(IMaritalStatusService genderService)
        {
            _genderService = genderService;
        }

        public async Task<List<MaritalStatusResponse>> Handle(MaritalStatusGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _genderService.GetAllAsync();
        }
    }
}
