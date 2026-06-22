using MediatR;

namespace SylviaNG.Recruitment.Application.Features.ExamCandidates.Commands.ExamCandidateDelete
{
    public class ExamCandidateDeleteCommand : IRequest<Unit>
    {
        public long ExamCandidateId { get; set; }

        public ExamCandidateDeleteCommand(long examCandidateId)
        {
            ExamCandidateId = examCandidateId;
        }
    }
}
