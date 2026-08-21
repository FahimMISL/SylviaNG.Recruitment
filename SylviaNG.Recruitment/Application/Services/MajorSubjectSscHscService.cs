using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.MajorSubjectsSscHsc.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class MajorSubjectSscHscService : IMajorSubjectSscHscService
    {
        private readonly IMajorSubjectSscHscRepository _majorSubjectSscHscRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MajorSubjectSscHscService(IMajorSubjectSscHscRepository majorSubjectSscHscRepository, IUnitOfWork unitOfWork)
        {
            _majorSubjectSscHscRepository = majorSubjectSscHscRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(MajorSubjectSscHscCreateRequest request)
        {
            var exists = await _majorSubjectSscHscRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("MajorSubjectSscHsc", "Name", request.Name);

            var entity = request.ToEntity();
            await _majorSubjectSscHscRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.MajorSubjectSscHscId;
        }

        public async Task UpdateAsync(long majorSubjectSscHscId, MajorSubjectSscHscUpdateRequest request)
        {
            var entity = await _majorSubjectSscHscRepository.GetByIdAsync(majorSubjectSscHscId)
                ?? throw new NotFoundException("MajorSubjectSscHsc", majorSubjectSscHscId);

            var nameTaken = await _majorSubjectSscHscRepository.ExistsByNameAsync(request.Name, majorSubjectSscHscId);
            if (nameTaken)
                throw new DuplicateException("MajorSubjectSscHsc", "Name", request.Name);

            entity.Name = request.Name;
            _majorSubjectSscHscRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long majorSubjectSscHscId)
        {
            var entity = await _majorSubjectSscHscRepository.GetByIdAsync(majorSubjectSscHscId)
                ?? throw new NotFoundException("MajorSubjectSscHsc", majorSubjectSscHscId);

            var usageCount = await _majorSubjectSscHscRepository.CountUsageAsync(majorSubjectSscHscId);
            if (usageCount > 0)
                throw new ResourceInUseException("MajorSubjectSscHsc", majorSubjectSscHscId, usageCount);

            _majorSubjectSscHscRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<MajorSubjectSscHscResponse>> GetAllAsync()
        {
            var entities = await _majorSubjectSscHscRepository.GetAllOrderedAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
