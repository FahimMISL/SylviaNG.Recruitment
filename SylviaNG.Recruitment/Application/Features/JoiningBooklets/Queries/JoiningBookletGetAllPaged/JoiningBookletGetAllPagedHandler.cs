using MediatR;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Queries.JoiningBookletGetAllPaged
{
    public class JoiningBookletGetAllPagedHandler : IRequestHandler<JoiningBookletGetAllPagedQuery, PagedResult<JoiningBookletResponse>>
    {
        private readonly IJoiningBookletService _joiningBookletService;

        public JoiningBookletGetAllPagedHandler(IJoiningBookletService joiningBookletService)
        {
            _joiningBookletService = joiningBookletService;
        }

        public async Task<PagedResult<JoiningBookletResponse>> Handle(JoiningBookletGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _joiningBookletService.GetPaginatedAsync(query.Request);
        }
    }
}
