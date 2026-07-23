using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Queries.ExamEnrollmentGetByExam
{
    public class ExamEnrollmentGetByExamHandler : IRequestHandler<ExamEnrollmentGetByExamQuery, List<ExamEnrollmentResponse>>
    {
        private readonly IExamEnrollmentService _examEnrollmentService;

        public ExamEnrollmentGetByExamHandler(IExamEnrollmentService examEnrollmentService)
        {
            _examEnrollmentService = examEnrollmentService;
        }

        public async Task<List<ExamEnrollmentResponse>> Handle(ExamEnrollmentGetByExamQuery query, CancellationToken cancellationToken)
        {
            return await _examEnrollmentService.GetByExamIdAsync(query.ExamId);
        }
    }
}
