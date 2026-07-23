using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.QuestionGroups.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class QuestionGroupService : IQuestionGroupService
    {
        private readonly IQuestionGroupRepository _questionGroupRepository;
        private readonly IUnitOfWork _unitOfWork;

        public QuestionGroupService(IQuestionGroupRepository questionGroupRepository, IUnitOfWork unitOfWork)
        {
            _questionGroupRepository = questionGroupRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(QuestionGroupCreateRequest request)
        {
            var exists = await _questionGroupRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("QuestionGroup", "Name", request.Name);

            var entity = request.ToEntity();

            await _questionGroupRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.QuestionGroupId;
        }

        public async Task UpdateAsync(long questionGroupId, QuestionGroupUpdateRequest request)
        {
            var entity = await _questionGroupRepository.GetByIdAsync(questionGroupId)
                ?? throw new NotFoundException("QuestionGroup", questionGroupId);

            var nameTaken = await _questionGroupRepository.ExistsByNameAsync(request.Name, questionGroupId);
            if (nameTaken)
                throw new DuplicateException("QuestionGroup", "Name", request.Name);

            entity.Name = request.Name;
            entity.Description = request.Description;

            _questionGroupRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SetActiveStatusAsync(long questionGroupId, bool isActive)
        {
            var entity = await _questionGroupRepository.GetByIdAsync(questionGroupId)
                ?? throw new NotFoundException("QuestionGroup", questionGroupId);

            entity.IsActive = isActive;

            _questionGroupRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<QuestionGroupResponse> GetByIdAsync(long questionGroupId)
        {
            var entity = await _questionGroupRepository.GetByIdAsync(questionGroupId)
                ?? throw new NotFoundException("QuestionGroup", questionGroupId);

            return entity.ToResponse();
        }

        public async Task<List<QuestionGroupResponse>> GetAllAsync()
        {
            var entities = await _questionGroupRepository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<List<QuestionGroupLookupResponse>> GetActiveLookupAsync()
        {
            var entities = await _questionGroupRepository.GetActiveAsync();
            return entities.Select(e => e.ToLookupResponse()).ToList();
        }
    }
}
