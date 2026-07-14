using MediatR;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Commands.ShortlistFilterApply
{
    public class ShortlistFilterApplyCommand : IRequest<ShortlistFilterApplyResponse>
    {
        public ShortlistFilterApplyRequest Request { get; set; }

        public ShortlistFilterApplyCommand(ShortlistFilterApplyRequest request)
        {
            Request = request;
        }
    }
}
