using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamCandidates.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamCandidates.Queries.ExamCandidateGetAll
{
    public class ExamCandidateGetAllQuery : IRequest<List<ExamCandidateResponse>>
    {
    }
}
