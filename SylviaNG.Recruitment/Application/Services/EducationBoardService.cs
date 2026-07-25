using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.EducationBoards.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class EducationBoardService : IEducationBoardService
    {
        private readonly IEducationBoardRepository _educationBoardRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EducationBoardService(IEducationBoardRepository educationBoardRepository, IUnitOfWork unitOfWork)
        {
            _educationBoardRepository = educationBoardRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(EducationBoardCreateRequest request)
        {
            var exists = await _educationBoardRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("EducationBoard", "Name", request.Name);

            var entity = request.ToEntity();
            await _educationBoardRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.EducationBoardId;
        }

        public async Task UpdateAsync(long educationBoardId, EducationBoardUpdateRequest request)
        {
            var entity = await _educationBoardRepository.GetByIdAsync(educationBoardId)
                ?? throw new NotFoundException("EducationBoard", educationBoardId);

            var nameTaken = await _educationBoardRepository.ExistsByNameAsync(request.Name, educationBoardId);
            if (nameTaken)
                throw new DuplicateException("EducationBoard", "Name", request.Name);

            entity.Code = request.Code;
            entity.Name = request.Name;
            _educationBoardRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long educationBoardId)
        {
            var entity = await _educationBoardRepository.GetByIdAsync(educationBoardId)
                ?? throw new NotFoundException("EducationBoard", educationBoardId);

            var usageCount = await _educationBoardRepository.CountUsageAsync(educationBoardId);
            if (usageCount > 0)
                throw new ResourceInUseException("EducationBoard", educationBoardId, usageCount);

            _educationBoardRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<EducationBoardResponse>> GetAllAsync()
        {
            var entities = await _educationBoardRepository.GetAllOrderedAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
