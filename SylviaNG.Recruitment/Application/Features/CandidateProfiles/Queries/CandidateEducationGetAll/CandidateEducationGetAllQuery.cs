using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateEducationGetAll
{
    public class CandidateEducationGetAllQuery : IRequest<List<CandidateEducationResponse>>
    {
    }
}
