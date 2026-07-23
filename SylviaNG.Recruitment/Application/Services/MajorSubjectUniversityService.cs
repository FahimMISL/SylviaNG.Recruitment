using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.MajorSubjectsUniversity.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class MajorSubjectUniversityService : IMajorSubjectUniversityService
    {
        private readonly IMajorSubjectUniversityRepository _majorSubjectUniversityRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MajorSubjectUniversityService(IMajorSubjectUniversityRepository majorSubjectUniversityRepository, IUnitOfWork unitOfWork)
        {
            _majorSubjectUniversityRepository = majorSubjectUniversityRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(MajorSubjectUniversityCreateRequest request)
        {
            var exists = await _majorSubjectUniversityRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("MajorSubjectUniversity", "Name", request.Name);

            var entity = request.ToEntity();
            await _majorSubjectUniversityRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.MajorSubjectUniversityId;
        }

        public async Task UpdateAsync(long majorSubjectUniversityId, MajorSubjectUniversityUpdateRequest request)
        {
            var entity = await _majorSubjectUniversityRepository.GetByIdAsync(majorSubjectUniversityId)
                ?? throw new NotFoundException("MajorSubjectUniversity", majorSubjectUniversityId);

            var nameTaken = await _majorSubjectUniversityRepository.ExistsByNameAsync(request.Name, majorSubjectUniversityId);
            if (nameTaken)
                throw new DuplicateException("MajorSubjectUniversity", "Name", request.Name);

            entity.Name = request.Name;
            _majorSubjectUniversityRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long majorSubjectUniversityId)
        {
            var entity = await _majorSubjectUniversityRepository.GetByIdAsync(majorSubjectUniversityId)
                ?? throw new NotFoundException("MajorSubjectUniversity", majorSubjectUniversityId);

            var usageCount = await _majorSubjectUniversityRepository.CountUsageAsync(majorSubjectUniversityId);
            if (usageCount > 0)
                throw new ResourceInUseException("MajorSubjectUniversity", majorSubjectUniversityId, usageCount);

            _majorSubjectUniversityRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<MajorSubjectUniversityResponse>> GetAllAsync()
        {
            var entities = await _majorSubjectUniversityRepository.GetAllOrderedAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
