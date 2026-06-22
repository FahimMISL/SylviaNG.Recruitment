using MediatR;
using SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Queries.ProfileFieldConfigGetAllPaged
{
    public class ProfileFieldConfigGetAllPagedQuery : IRequest<PagedResult<ProfileFieldConfigResponse>>
    {
        public PagedRequest Request { get; set; }

        public ProfileFieldConfigGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
