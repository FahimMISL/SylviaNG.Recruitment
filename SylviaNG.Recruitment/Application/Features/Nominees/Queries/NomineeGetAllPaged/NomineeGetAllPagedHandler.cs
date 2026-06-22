using MediatR;
using SylviaNG.Recruitment.Application.Features.Nominees.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.Nominees.Queries.NomineeGetAllPaged
{
    public class NomineeGetAllPagedHandler : IRequestHandler<NomineeGetAllPagedQuery, PagedResult<NomineeResponse>>
    {
        private readonly INomineeService _nomineeService;

        public NomineeGetAllPagedHandler(INomineeService nomineeService)
        {
            _nomineeService = nomineeService;
        }

        public async Task<PagedResult<NomineeResponse>> Handle(NomineeGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _nomineeService.GetPaginatedAsync(query.Request);
        }
    }
}
