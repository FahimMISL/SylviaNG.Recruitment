using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Queries.InterviewEvaluationScoreGetAll
{
    public class InterviewEvaluationScoreGetAllQuery : IRequest<List<InterviewEvaluationScoreResponse>>
    {
    }
}
