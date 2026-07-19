using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.ExamQuestions.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Services
{
    public class ExamQuestionService : IExamQuestionService
    {
        private readonly IExamQuestionRepository _examQuestionRepository;
        private readonly IQuestionGroupRepository _questionGroupRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ExamQuestionService(
            IExamQuestionRepository examQuestionRepository,
            IQuestionGroupRepository questionGroupRepository,
            IUnitOfWork unitOfWork)
        {
            _examQuestionRepository = examQuestionRepository;
            _questionGroupRepository = questionGroupRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(ExamQuestionCreateRequest request)
        {
            await EnsureQuestionGroupExistsAsync(request.QuestionGroupId);

            var entity = request.ToEntity();
            NormalizeDisplayOrder(entity.Options);

            await _examQuestionRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.ExamQuestionId;
        }

        public async Task UpdateAsync(long examQuestionId, ExamQuestionUpdateRequest request)
        {
            var entity = await _examQuestionRepository.GetByIdWithOptionsAsync(examQuestionId)
                ?? throw new NotFoundException("ExamQuestion", examQuestionId);

            await EnsureQuestionGroupExistsAsync(request.QuestionGroupId);

            entity.QuestionGroupId = request.QuestionGroupId;
            entity.QuestionText = request.QuestionText;
            entity.QuestionType = request.QuestionType;
            entity.DifficultyLevel = request.DifficultyLevel;
            entity.Marks = request.Marks;
            entity.Explanation = request.Explanation;
            entity.ModelAnswer = request.ModelAnswer;

            // Whole-collection replace: nothing else references ExamQuestionOptionId yet, same
            // precedent as ShortlistFilterService.UpdateAsync's Criteria replace.
            entity.Options.Clear();
            var newOptions = request.Options.OrderBy(o => o.DisplayOrder).Select(o => o.ToEntity()).ToList();
            NormalizeDisplayOrder(newOptions);
            foreach (var option in newOptions)
            {
                entity.Options.Add(option);
            }

            _examQuestionRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SetActiveStatusAsync(long examQuestionId, bool isActive)
        {
            var entity = await _examQuestionRepository.GetByIdAsync(examQuestionId)
                ?? throw new NotFoundException("ExamQuestion", examQuestionId);

            entity.IsActive = isActive;

            _examQuestionRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ExamQuestionResponse> GetByIdAsync(long examQuestionId)
        {
            var entity = await _examQuestionRepository.GetByIdWithOptionsAsync(examQuestionId)
                ?? throw new NotFoundException("ExamQuestion", examQuestionId);

            return entity.ToResponse();
        }

        public async Task<PagedResult<ExamQuestionResponse>> GetPaginatedAsync(
            PagedRequest request,
            long? questionGroupId,
            QuestionTypeEnum? questionType,
            DifficultyLevelEnum? difficultyLevel,
            bool? isActive)
        {
            request.SearchProperties = new[] { "QuestionText" };

            var paged = await _examQuestionRepository.GetPaginatedAsync(request, questionGroupId, questionType, difficultyLevel, isActive);

            return new PagedResult<ExamQuestionResponse>
            {
                Data = paged.Data.Select(e => e.ToResponse()).ToList(),
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount
            };
        }

        private async Task EnsureQuestionGroupExistsAsync(long questionGroupId)
        {
            var exists = await _questionGroupRepository.GetByIdAsync(questionGroupId) != null;
            if (!exists)
                throw new NotFoundException("QuestionGroup", questionGroupId);
        }

        private static void NormalizeDisplayOrder(IEnumerable<ExamQuestionOption> options)
        {
            var order = 0;
            foreach (var option in options)
            {
                option.DisplayOrder = order++;
            }
        }
    }
}
