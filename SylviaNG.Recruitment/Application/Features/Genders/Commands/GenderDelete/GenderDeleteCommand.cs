using MediatR;

namespace SylviaNG.Recruitment.Application.Features.Genders.Commands.GenderDelete
{
    public class GenderDeleteCommand : IRequest<Unit>
    {
        public long GenderId { get; set; }

        public GenderDeleteCommand(long genderId)
        {
            GenderId = genderId;
        }
    }
}
