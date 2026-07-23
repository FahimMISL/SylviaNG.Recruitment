using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Commands.ExamEnrollCandidates
{
    public class ExamEnrollCandidatesHandler : IRequestHandler<ExamEnrollCandidatesCommand, List<long>>
    {
        private readonly IExamEnrollmentService _examEnrollmentService;

        public ExamEnrollCandidatesHandler(IExamEnrollmentService examEnrollmentService)
        {
            _examEnrollmentService = examEnrollmentService;
        }

        public async Task<List<long>> Handle(ExamEnrollCandidatesCommand command, CancellationToken cancellationToken)
        {
            return await _examEnrollmentService.EnrollAsync(command.ExamId, command.JobApplicationIds);
        }
    }
}
