using MediatR;
using SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Queries.RecruitmentAgencyGetById
{
    public class RecruitmentAgencyGetByIdHandler : IRequestHandler<RecruitmentAgencyGetByIdQuery, RecruitmentAgencyResponse>
    {
        private readonly IRecruitmentAgencyService _service;

        public RecruitmentAgencyGetByIdHandler(IRecruitmentAgencyService service)
        {
            _service = service;
        }

        public async Task<RecruitmentAgencyResponse> Handle(RecruitmentAgencyGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.RecruitmentAgencyId);
        }
    }
}
