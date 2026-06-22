using MediatR;
using SylviaNG.Recruitment.Application.Features.AdmitCards.Models;

namespace SylviaNG.Recruitment.Application.Features.AdmitCards.Queries.AdmitCardGetAll
{
    public class AdmitCardGetAllQuery : IRequest<List<AdmitCardResponse>>
    {
    }
}
