using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamCandidates.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamCandidates.Commands.ExamCandidateCreate
{
    public class ExamCandidateCreateCommand : IRequest<long>
    {
        public ExamCandidateCreateRequest Request { get; set; }

        public ExamCandidateCreateCommand(ExamCandidateCreateRequest request)
        {
            Request = request;
        }
    }
}
