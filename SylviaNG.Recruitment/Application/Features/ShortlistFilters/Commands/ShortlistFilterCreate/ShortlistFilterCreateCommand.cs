using MediatR;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Commands.ShortlistFilterCreate
{
    public class ShortlistFilterCreateCommand : IRequest<long>
    {
        public ShortlistFilterCreateRequest Request { get; set; }

        public ShortlistFilterCreateCommand(ShortlistFilterCreateRequest request)
        {
            Request = request;
        }
    }
}
