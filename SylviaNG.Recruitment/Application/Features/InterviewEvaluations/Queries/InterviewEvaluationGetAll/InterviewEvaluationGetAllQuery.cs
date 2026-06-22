using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Queries.InterviewEvaluationGetAll
{
    public class InterviewEvaluationGetAllQuery : IRequest<List<InterviewEvaluationResponse>>
    {
    }
}
