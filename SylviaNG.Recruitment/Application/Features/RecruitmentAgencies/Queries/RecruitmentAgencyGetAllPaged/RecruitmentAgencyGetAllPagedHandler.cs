using MediatR;
using SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Queries.RecruitmentAgencyGetAllPaged
{
    public class RecruitmentAgencyGetAllPagedHandler : IRequestHandler<RecruitmentAgencyGetAllPagedQuery, PagedResult<RecruitmentAgencyResponse>>
    {
        private readonly IRecruitmentAgencyService _recruitmentAgencyService;

        public RecruitmentAgencyGetAllPagedHandler(IRecruitmentAgencyService recruitmentAgencyService)
        {
            _recruitmentAgencyService = recruitmentAgencyService;
        }

        public async Task<PagedResult<RecruitmentAgencyResponse>> Handle(RecruitmentAgencyGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _recruitmentAgencyService.GetPaginatedAsync(query.Request);
        }
    }
}
