using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamTaking.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamTaking.Queries.ExamTakingGetMyEnrollments
{
    public class ExamTakingGetMyEnrollmentsQuery : IRequest<List<MyExamEnrollmentResponse>>
    {
    }
}
