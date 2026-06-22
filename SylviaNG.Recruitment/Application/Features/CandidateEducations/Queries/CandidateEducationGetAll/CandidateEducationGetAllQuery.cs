using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateEducations.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateEducations.Queries.CandidateEducationGetAll
{
    public class CandidateEducationGetAllQuery : IRequest<List<CandidateEducationResponse>>
    {
    }
}
