using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamTaking.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamTaking.Queries.ExamTakingGetMyEnrollments
{
    public class ExamTakingGetMyEnrollmentsHandler : IRequestHandler<ExamTakingGetMyEnrollmentsQuery, List<MyExamEnrollmentResponse>>
    {
        private readonly IExamTakingService _examTakingService;

        public ExamTakingGetMyEnrollmentsHandler(IExamTakingService examTakingService)
        {
            _examTakingService = examTakingService;
        }

        public async Task<List<MyExamEnrollmentResponse>> Handle(ExamTakingGetMyEnrollmentsQuery query, CancellationToken cancellationToken)
        {
            return await _examTakingService.GetMyEnrollmentsAsync();
        }
    }
}
