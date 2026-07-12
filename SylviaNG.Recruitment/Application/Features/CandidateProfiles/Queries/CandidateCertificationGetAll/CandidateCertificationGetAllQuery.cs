using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateCertificationGetAll
{
    public class CandidateCertificationGetAllQuery : IRequest<List<CandidateCertificationResponse>>
    {
    }
}
