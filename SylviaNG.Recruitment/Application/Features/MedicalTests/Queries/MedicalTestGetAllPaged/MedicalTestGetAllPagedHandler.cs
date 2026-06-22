using MediatR;
using SylviaNG.Recruitment.Application.Features.MedicalTests.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.MedicalTests.Queries.MedicalTestGetAllPaged
{
    public class MedicalTestGetAllPagedHandler : IRequestHandler<MedicalTestGetAllPagedQuery, PagedResult<MedicalTestResponse>>
    {
        private readonly IMedicalTestService _medicalTestService;

        public MedicalTestGetAllPagedHandler(IMedicalTestService medicalTestService)
        {
            _medicalTestService = medicalTestService;
        }

        public async Task<PagedResult<MedicalTestResponse>> Handle(MedicalTestGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _medicalTestService.GetPaginatedAsync(query.Request);
        }
    }
}
