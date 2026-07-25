using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamQuestions.Models;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.ExamQuestions.Queries.ExamQuestionGetAllPaged
{
    public class ExamQuestionGetAllPagedQuery : IRequest<PagedResult<ExamQuestionResponse>>
    {
        public PagedRequest Request { get; set; }
        public long? QuestionGroupId { get; set; }
        public QuestionTypeEnum? QuestionType { get; set; }
        public DifficultyLevelEnum? DifficultyLevel { get; set; }
        public bool? IsActive { get; set; }

        public ExamQuestionGetAllPagedQuery(
            PagedRequest request,
            long? questionGroupId,
            QuestionTypeEnum? questionType,
            DifficultyLevelEnum? difficultyLevel,
            bool? isActive)
        {
            Request = request;
            QuestionGroupId = questionGroupId;
            QuestionType = questionType;
            DifficultyLevel = difficultyLevel;
            IsActive = isActive;
        }
    }
}
