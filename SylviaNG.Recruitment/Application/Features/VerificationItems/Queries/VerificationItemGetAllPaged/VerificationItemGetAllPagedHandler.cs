using MediatR;
using SylviaNG.Recruitment.Application.Features.VerificationItems.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.VerificationItems.Queries.VerificationItemGetAllPaged
{
    public class VerificationItemGetAllPagedHandler : IRequestHandler<VerificationItemGetAllPagedQuery, PagedResult<VerificationItemResponse>>
    {
        private readonly IVerificationItemService _verificationItemService;

        public VerificationItemGetAllPagedHandler(IVerificationItemService verificationItemService)
        {
            _verificationItemService = verificationItemService;
        }

        public async Task<PagedResult<VerificationItemResponse>> Handle(VerificationItemGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _verificationItemService.GetPaginatedAsync(query.Request);
        }
    }
}
