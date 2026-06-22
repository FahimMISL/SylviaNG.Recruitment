using MediatR;
using SylviaNG.Recruitment.Application.Features.OfferCompensations.Models;

namespace SylviaNG.Recruitment.Application.Features.OfferCompensations.Queries.OfferCompensationGetAll
{
    public class OfferCompensationGetAllQuery : IRequest<List<OfferCompensationResponse>>
    {
    }
}
