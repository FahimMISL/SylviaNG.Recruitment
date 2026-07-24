using MediatR;
using SylviaNG.Recruitment.Application.Features.Religions.Models;

namespace SylviaNG.Recruitment.Application.Features.Religions.Commands.ReligionUpdate
{
    public class ReligionUpdateCommand : IRequest<Unit>
    {
        public long ReligionId { get; set; }
        public ReligionUpdateRequest Request { get; set; }

        public ReligionUpdateCommand(long religionId, ReligionUpdateRequest request)
        {
            ReligionId = religionId;
            Request = request;
        }
    }
}
