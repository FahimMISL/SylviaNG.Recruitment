using MediatR;
using SylviaNG.Recruitment.Application.Features.AdmitCards.Models;

namespace SylviaNG.Recruitment.Application.Features.AdmitCards.Commands.AdmitCardUpdate
{
    public class AdmitCardUpdateCommand : IRequest<Unit>
    {
        public long AdmitCardId { get; set; }
        public AdmitCardUpdateRequest Request { get; set; }

        public AdmitCardUpdateCommand(long admitCardId, AdmitCardUpdateRequest request)
        {
            AdmitCardId = admitCardId;
            Request = request;
        }
    }
}
