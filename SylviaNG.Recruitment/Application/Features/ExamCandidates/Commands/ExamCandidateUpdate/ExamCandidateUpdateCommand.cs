using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamCandidates.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamCandidates.Commands.ExamCandidateUpdate
{
    public class ExamCandidateUpdateCommand : IRequest<Unit>
    {
        public long ExamCandidateId { get; set; }
        public ExamCandidateUpdateRequest Request { get; set; }

        public ExamCandidateUpdateCommand(long examCandidateId, ExamCandidateUpdateRequest request)
        {
            ExamCandidateId = examCandidateId;
            Request = request;
        }
    }
}
