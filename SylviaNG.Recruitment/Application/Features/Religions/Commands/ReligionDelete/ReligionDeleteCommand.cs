using MediatR;

namespace SylviaNG.Recruitment.Application.Features.Religions.Commands.ReligionDelete
{
    public class ReligionDeleteCommand : IRequest<Unit>
    {
        public long ReligionId { get; set; }

        public ReligionDeleteCommand(long religionId)
        {
            ReligionId = religionId;
        }
    }
}
