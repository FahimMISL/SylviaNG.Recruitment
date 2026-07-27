using MediatR;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models;

namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Queries.JoiningBookletGetEligibleCandidates
{
    public class JoiningBookletGetEligibleCandidatesQuery : IRequest<List<JoiningBookletEligibleCandidateResponse>>
    {
    }
}
