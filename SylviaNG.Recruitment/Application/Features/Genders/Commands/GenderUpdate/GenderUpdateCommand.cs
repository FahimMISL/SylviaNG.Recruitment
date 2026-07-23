using MediatR;
using SylviaNG.Recruitment.Application.Features.Genders.Models;

namespace SylviaNG.Recruitment.Application.Features.Genders.Commands.GenderUpdate
{
    public class GenderUpdateCommand : IRequest<Unit>
    {
        public long GenderId { get; set; }
        public GenderUpdateRequest Request { get; set; }

        public GenderUpdateCommand(long genderId, GenderUpdateRequest request)
        {
            GenderId = genderId;
            Request = request;
        }
    }
}
