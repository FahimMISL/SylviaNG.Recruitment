using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamCandidates.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamCandidates.Queries.ExamCandidateGetById
{
    public class ExamCandidateGetByIdQuery : IRequest<ExamCandidateResponse>
    {
        public long ExamCandidateId { get; set; }

        public ExamCandidateGetByIdQuery(long examCandidateId)
        {
            ExamCandidateId = examCandidateId;
        }
    }
}
