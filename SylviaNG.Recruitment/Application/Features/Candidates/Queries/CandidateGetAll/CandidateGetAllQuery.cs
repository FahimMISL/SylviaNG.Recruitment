using MediatR;
using SylviaNG.Recruitment.Application.Features.Candidates.Models;

namespace SylviaNG.Recruitment.Application.Features.Candidates.Queries.CandidateGetAll
{
    public class CandidateGetAllQuery : IRequest<List<CandidateResponse>>
    {
    }
}
