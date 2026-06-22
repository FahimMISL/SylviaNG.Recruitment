using MediatR;
using SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Queries.RecruitmentAgencyGetAll
{
    public class RecruitmentAgencyGetAllHandler : IRequestHandler<RecruitmentAgencyGetAllQuery, List<RecruitmentAgencyResponse>>
    {
        private readonly IRecruitmentAgencyService _service;

        public RecruitmentAgencyGetAllHandler(IRecruitmentAgencyService service)
        {
            _service = service;
        }

        public async Task<List<RecruitmentAgencyResponse>> Handle(RecruitmentAgencyGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
