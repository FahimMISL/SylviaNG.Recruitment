using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewSessions.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewSessions.Queries.InterviewSessionGetAll
{
    public class InterviewSessionGetAllQuery : IRequest<List<InterviewSessionResponse>>
    {
    }
}
