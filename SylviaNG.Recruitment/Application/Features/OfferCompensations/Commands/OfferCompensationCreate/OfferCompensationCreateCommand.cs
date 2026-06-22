using MediatR;
using SylviaNG.Recruitment.Application.Features.OfferCompensations.Models;

namespace SylviaNG.Recruitment.Application.Features.OfferCompensations.Commands.OfferCompensationCreate
{
    public class OfferCompensationCreateCommand : IRequest<long>
    {
        public OfferCompensationCreateRequest Request { get; set; }

        public OfferCompensationCreateCommand(OfferCompensationCreateRequest request)
        {
            Request = request;
        }
    }
}
