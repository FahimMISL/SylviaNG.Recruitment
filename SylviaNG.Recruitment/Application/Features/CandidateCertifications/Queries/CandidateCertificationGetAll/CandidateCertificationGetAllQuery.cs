using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateCertifications.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateCertifications.Queries.CandidateCertificationGetAll
{
    public class CandidateCertificationGetAllQuery : IRequest<List<CandidateCertificationResponse>>
    {
    }
}
