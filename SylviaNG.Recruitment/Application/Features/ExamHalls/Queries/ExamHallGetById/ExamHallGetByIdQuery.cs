using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamHalls.Queries.ExamHallGetById
{
    public class ExamHallGetByIdQuery : IRequest<ExamHallResponse>
    {
        public long ExamHallId { get; set; }

        public ExamHallGetByIdQuery(long examHallId)
        {
            ExamHallId = examHallId;
        }
    }
}
