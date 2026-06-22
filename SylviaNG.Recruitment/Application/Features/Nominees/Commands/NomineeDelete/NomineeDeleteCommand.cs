using MediatR;

namespace SylviaNG.Recruitment.Application.Features.Nominees.Commands.NomineeDelete
{
    public class NomineeDeleteCommand : IRequest<Unit>
    {
        public long NomineeId { get; set; }

        public NomineeDeleteCommand(long nomineeId)
        {
            NomineeId = nomineeId;
        }
    }
}
