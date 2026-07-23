using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.Degrees.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class DegreeService : IDegreeService
    {
        private readonly IDegreeRepository _degreeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DegreeService(IDegreeRepository degreeRepository, IUnitOfWork unitOfWork)
        {
            _degreeRepository = degreeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(DegreeCreateRequest request)
        {
            var exists = await _degreeRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("Degree", "Name", request.Name);

            var entity = request.ToEntity();
            await _degreeRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.DegreeId;
        }

        public async Task UpdateAsync(long degreeId, DegreeUpdateRequest request)
        {
            var entity = await _degreeRepository.GetByIdAsync(degreeId)
                ?? throw new NotFoundException("Degree", degreeId);

            var nameTaken = await _degreeRepository.ExistsByNameAsync(request.Name, degreeId);
            if (nameTaken)
                throw new DuplicateException("Degree", "Name", request.Name);

            entity.Name = request.Name;
            entity.FullName = request.FullName;
            entity.Position = request.Position;
            _degreeRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long degreeId)
        {
            var entity = await _degreeRepository.GetByIdAsync(degreeId)
                ?? throw new NotFoundException("Degree", degreeId);

            var usageCount = await _degreeRepository.CountUsageAsync(degreeId);
            if (usageCount > 0)
                throw new ResourceInUseException("Degree", degreeId, usageCount);

            _degreeRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<DegreeResponse>> GetAllAsync()
        {
            var entities = await _degreeRepository.GetAllOrderedAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
