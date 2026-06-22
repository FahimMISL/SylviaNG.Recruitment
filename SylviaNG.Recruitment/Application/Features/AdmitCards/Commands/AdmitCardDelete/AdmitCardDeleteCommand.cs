using MediatR;

namespace SylviaNG.Recruitment.Application.Features.AdmitCards.Commands.AdmitCardDelete
{
    public class AdmitCardDeleteCommand : IRequest<Unit>
    {
        public long AdmitCardId { get; set; }

        public AdmitCardDeleteCommand(long admitCardId)
        {
            AdmitCardId = admitCardId;
        }
    }
}
