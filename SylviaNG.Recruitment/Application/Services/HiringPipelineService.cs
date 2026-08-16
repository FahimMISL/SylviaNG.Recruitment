using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.HiringPipelines.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class HiringPipelineService : IHiringPipelineService
    {
        private readonly IHiringPipelineRepository _hiringPipelineRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public HiringPipelineService(IHiringPipelineRepository hiringPipelineRepository, ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _hiringPipelineRepository = hiringPipelineRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(HiringPipelineCreateRequest request)
        {
            var exists = await _hiringPipelineRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("HiringPipeline", "Name", request.Name);

            var entity = request.ToEntity();
            entity.CompanyId = await _currentUserService.GetCurrentUserCompanyIdAsync();
            foreach (var stage in entity.Stages)
                stage.CompanyId = entity.CompanyId;
            NormalizeDisplayOrder(entity.Stages);

            await _hiringPipelineRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.HiringPipelineId;
        }

        public async Task UpdateAsync(long hiringPipelineId, HiringPipelineUpdateRequest request)
        {
            var entity = await _hiringPipelineRepository.GetByIdWithStagesAsync(hiringPipelineId)
                ?? throw new NotFoundException("HiringPipeline", hiringPipelineId);

            var nameTaken = await _hiringPipelineRepository.ExistsByNameAsync(request.Name, hiringPipelineId);
            if (nameTaken)
                throw new DuplicateException("HiringPipeline", "Name", request.Name);

            entity.Name = request.Name;
            entity.Description = request.Description;

            // Whole-collection replace: no other entity references PipelineStageId yet in this
            // phase (candidate-stage tracking is a later, separate feature), so a clean
            // clear-and-rebuild is simpler and safer than diffing add/edit/remove/reorder.
            entity.Stages.Clear();
            var newStages = request.Stages.OrderBy(s => s.DisplayOrder).Select(s => s.ToEntity()).ToList();
            NormalizeDisplayOrder(newStages);
            foreach (var stage in newStages)
            {
                stage.CompanyId = entity.CompanyId;
                entity.Stages.Add(stage);
            }

            _hiringPipelineRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long hiringPipelineId)
        {
            var entity = await _hiringPipelineRepository.GetByIdWithStagesAsync(hiringPipelineId)
                ?? throw new NotFoundException("HiringPipeline", hiringPipelineId);

            if (entity.JobPostings.Count > 0)
                throw new ResourceInUseException("HiringPipeline", hiringPipelineId, entity.JobPostings.Count);

            _hiringPipelineRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<long> DuplicateAsync(long hiringPipelineId)
        {
            var source = await _hiringPipelineRepository.GetByIdWithStagesAsync(hiringPipelineId)
                ?? throw new NotFoundException("HiringPipeline", hiringPipelineId);

            var copyName = await NextAvailableCopyNameAsync(source.Name);

            var clone = new Domain.Entities.HiringPipeline
            {
                Name = copyName,
                Description = source.Description,
                IsActive = false, // duplicates land inactive until reviewed/published by an admin
                CompanyId = source.CompanyId,
                Stages = source.Stages.Select(s => new Domain.Entities.PipelineStage
                {
                    CompanyId = source.CompanyId,
                    Name = s.Name,
                    StageType = s.StageType,
                    DisplayOrder = s.DisplayOrder,
                    Description = s.Description,
                    PassingCriteria = s.PassingCriteria,
                    IsActive = s.IsActive,
                    IsMandatory = s.IsMandatory,
                    EstimatedDurationMinutes = s.EstimatedDurationMinutes,
                    SlaDays = s.SlaDays,
                    MaxMarks = s.MaxMarks,
                    PassMarks = s.PassMarks,
                    RequiredDocuments = s.RequiredDocuments,
                    AutoProgressionRule = s.AutoProgressionRule,
                    AutoProgressionTargetDisplayOrder = s.AutoProgressionTargetDisplayOrder,
                    ManualApprovalRequired = s.ManualApprovalRequired
                }).ToList()
            };

            await _hiringPipelineRepository.AddAsync(clone);
            await _unitOfWork.SaveChangesAsync();

            return clone.HiringPipelineId;
        }

        public async Task SetActiveAsync(long hiringPipelineId, bool isActive)
        {
            var entity = await _hiringPipelineRepository.GetByIdAsync(hiringPipelineId)
                ?? throw new NotFoundException("HiringPipeline", hiringPipelineId);

            entity.IsActive = isActive;
            _hiringPipelineRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<HiringPipelineResponse> GetByIdAsync(long hiringPipelineId)
        {
            var entity = await _hiringPipelineRepository.GetByIdWithStagesAsync(hiringPipelineId)
                ?? throw new NotFoundException("HiringPipeline", hiringPipelineId);

            return entity.ToResponse();
        }

        public async Task<List<HiringPipelineResponse>> GetAllAsync()
        {
            var entities = await _hiringPipelineRepository.GetAllWithStagesAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<List<HiringPipelineLookupResponse>> GetActiveLookupAsync()
        {
            var entities = await _hiringPipelineRepository.GetActiveAsync();
            return entities.Select(e => e.ToLookupResponse()).ToList();
        }

        private static void NormalizeDisplayOrder(IEnumerable<Domain.Entities.PipelineStage> stages)
        {
            var order = 0;
            foreach (var stage in stages)
            {
                stage.DisplayOrder = order++;
            }
        }

        private async Task<string> NextAvailableCopyNameAsync(string sourceName)
        {
            var candidate = $"{sourceName} (Copy)";
            var suffix = 2;
            while (await _hiringPipelineRepository.ExistsByNameAsync(candidate))
            {
                candidate = $"{sourceName} (Copy {suffix})";
                suffix++;
            }
            return candidate;
        }
    }
}
