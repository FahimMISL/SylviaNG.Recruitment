using MediatR;
using SylviaNG.Recruitment.Application.Features.Nominees.Models;

namespace SylviaNG.Recruitment.Application.Features.Nominees.Commands.NomineeUpdate
{
    public class NomineeUpdateCommand : IRequest<Unit>
    {
        public long NomineeId { get; set; }
        public NomineeUpdateRequest Request { get; set; }

        public NomineeUpdateCommand(long nomineeId, NomineeUpdateRequest request)
        {
            NomineeId = nomineeId;
            Request = request;
        }
    }
}
