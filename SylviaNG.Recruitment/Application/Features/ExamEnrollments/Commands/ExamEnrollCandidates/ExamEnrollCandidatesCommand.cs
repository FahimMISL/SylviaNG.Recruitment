using MediatR;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Commands.ExamEnrollCandidates
{
    public class ExamEnrollCandidatesCommand : IRequest<List<long>>
    {
        public long ExamId { get; set; }
        public List<long> JobApplicationIds { get; set; }

        public ExamEnrollCandidatesCommand(long examId, List<long> jobApplicationIds)
        {
            ExamId = examId;
            JobApplicationIds = jobApplicationIds;
        }
    }
}
