using MediatR;
using SylviaNG.Recruitment.Application.Features.AdmitCards.Models;

namespace SylviaNG.Recruitment.Application.Features.AdmitCards.Commands.AdmitCardCreate
{
    public class AdmitCardCreateCommand : IRequest<long>
    {
        public AdmitCardCreateRequest Request { get; set; }

        public AdmitCardCreateCommand(AdmitCardCreateRequest request)
        {
            Request = request;
        }
    }
}
