using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class AssessmentWorkflowService : IAssessmentWorkflowService
    {
        private readonly IAssessmentWorkflowRepository _assessmentWorkflowRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AssessmentWorkflowService(IAssessmentWorkflowRepository assessmentWorkflowRepository, IUnitOfWork unitOfWork)
        {
            _assessmentWorkflowRepository = assessmentWorkflowRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(AssessmentWorkflowCreateRequest request)
        {
            var exists = await _assessmentWorkflowRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("AssessmentWorkflow", "Name", request.Name);

            var entity = request.ToEntity();
            NormalizeDisplayOrder(entity.Stages);

            await _assessmentWorkflowRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.AssessmentWorkflowId;
        }

        public async Task UpdateAsync(long assessmentWorkflowId, AssessmentWorkflowUpdateRequest request)
        {
            var entity = await _assessmentWorkflowRepository.GetByIdWithStagesAsync(assessmentWorkflowId)
                ?? throw new NotFoundException("AssessmentWorkflow", assessmentWorkflowId);

            var nameTaken = await _assessmentWorkflowRepository.ExistsByNameAsync(request.Name, assessmentWorkflowId);
            if (nameTaken)
                throw new DuplicateException("AssessmentWorkflow", "Name", request.Name);

            entity.Name = request.Name;
            entity.Description = request.Description;

            // Whole-collection replace: no other entity references AssessmentStageId yet, so a
            // clean clear-and-rebuild is simpler and safer than diffing add/edit/remove/reorder.
            entity.Stages.Clear();
            var newStages = request.Stages.OrderBy(s => s.DisplayOrder).Select(s => s.ToEntity()).ToList();
            NormalizeDisplayOrder(newStages);
            foreach (var stage in newStages)
            {
                entity.Stages.Add(stage);
            }

            _assessmentWorkflowRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long assessmentWorkflowId)
        {
            var entity = await _assessmentWorkflowRepository.GetByIdWithStagesAsync(assessmentWorkflowId)
                ?? throw new NotFoundException("AssessmentWorkflow", assessmentWorkflowId);

            if (entity.JobPostings.Count > 0)
                throw new ResourceInUseException("AssessmentWorkflow", assessmentWorkflowId, entity.JobPostings.Count);

            _assessmentWorkflowRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SetActiveAsync(long assessmentWorkflowId, bool isActive)
        {
            var entity = await _assessmentWorkflowRepository.GetByIdAsync(assessmentWorkflowId)
                ?? throw new NotFoundException("AssessmentWorkflow", assessmentWorkflowId);

            entity.IsActive = isActive;
            _assessmentWorkflowRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<AssessmentWorkflowResponse> GetByIdAsync(long assessmentWorkflowId)
        {
            var entity = await _assessmentWorkflowRepository.GetByIdWithStagesAsync(assessmentWorkflowId)
                ?? throw new NotFoundException("AssessmentWorkflow", assessmentWorkflowId);

            return entity.ToResponse();
        }

        public async Task<List<AssessmentWorkflowResponse>> GetAllAsync()
        {
            var entities = await _assessmentWorkflowRepository.GetAllWithStagesAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<List<AssessmentWorkflowLookupResponse>> GetActiveLookupAsync()
        {
            var entities = await _assessmentWorkflowRepository.GetActiveAsync();
            return entities.Select(e => e.ToLookupResponse()).ToList();
        }

        private static void NormalizeDisplayOrder(IEnumerable<Domain.Entities.AssessmentStage> stages)
        {
            var order = 0;
            foreach (var stage in stages)
            {
                stage.DisplayOrder = order++;
            }
        }
    }
}
