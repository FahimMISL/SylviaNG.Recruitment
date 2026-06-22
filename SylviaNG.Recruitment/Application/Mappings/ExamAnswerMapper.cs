using SylviaNG.Recruitment.Application.Features.ExamAnswers.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class ExamAnswerMapper
    {
        public static ExamAnswer ToEntity(this ExamAnswerCreateRequest request)
        {
            return new ExamAnswer
            {
                ExamCandidateId = request.ExamCandidateId,
                QuestionId = request.QuestionId,
                SelectedOptionId = request.SelectedOptionId,
                AnswerText = request.AnswerText,
                MarksAwarded = request.MarksAwarded,
                IsCorrect = request.IsCorrect,
            };
        }

        public static void ApplyUpdate(this ExamAnswer entity, ExamAnswerUpdateRequest request)
        {
            if (request.ExamCandidateId.HasValue) entity.ExamCandidateId = request.ExamCandidateId.Value;
            if (request.QuestionId.HasValue) entity.QuestionId = request.QuestionId.Value;
            if (request.SelectedOptionId.HasValue) entity.SelectedOptionId = request.SelectedOptionId.Value;
            if (request.AnswerText is not null) entity.AnswerText = request.AnswerText;
            if (request.MarksAwarded.HasValue) entity.MarksAwarded = request.MarksAwarded.Value;
            if (request.IsCorrect.HasValue) entity.IsCorrect = request.IsCorrect.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static ExamAnswerResponse ToResponse(this ExamAnswer entity)
        {
            return new ExamAnswerResponse
            {
                ExamAnswerId = entity.ExamAnswerId,
                ExamCandidateId = entity.ExamCandidateId,
                QuestionId = entity.QuestionId,
                SelectedOptionId = entity.SelectedOptionId,
                AnswerText = entity.AnswerText,
                MarksAwarded = entity.MarksAwarded,
                IsCorrect = entity.IsCorrect,
                IsActive = entity.IsActive,
            };
        }
    }
}
