using MediatR;
using SylviaNG.Recruitment.Application.Features.AdmitCards.Models;

namespace SylviaNG.Recruitment.Application.Features.AdmitCards.Queries.AdmitCardGetById
{
    public class AdmitCardGetByIdQuery : IRequest<AdmitCardResponse>
    {
        public long AdmitCardId { get; set; }

        public AdmitCardGetByIdQuery(long admitCardId)
        {
            AdmitCardId = admitCardId;
        }
    }
}
