using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.Countries.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CountryService(ICountryRepository countryRepository, IUnitOfWork unitOfWork)
        {
            _countryRepository = countryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(CountryCreateRequest request)
        {
            if (await _countryRepository.ExistsByNameAsync(request.Name))
                throw new DuplicateException("Country", "Name", request.Name);
            if (await _countryRepository.ExistsByCodeAsync(request.Code))
                throw new DuplicateException("Country", "Code", request.Code);

            var entity = request.ToEntity();
            await _countryRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.CountryId;
        }

        public async Task UpdateAsync(long countryId, CountryUpdateRequest request)
        {
            var entity = await _countryRepository.GetByIdAsync(countryId)
                ?? throw new NotFoundException("Country", countryId);

            if (await _countryRepository.ExistsByNameAsync(request.Name, countryId))
                throw new DuplicateException("Country", "Name", request.Name);
            if (await _countryRepository.ExistsByCodeAsync(request.Code, countryId))
                throw new DuplicateException("Country", "Code", request.Code);

            entity.Name = request.Name;
            entity.Code = request.Code;
            entity.DialCode = request.DialCode;
            _countryRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long countryId)
        {
            var entity = await _countryRepository.GetByIdAsync(countryId)
                ?? throw new NotFoundException("Country", countryId);

            var usageCount = await _countryRepository.CountUsageAsync(countryId);
            if (usageCount > 0)
                throw new ResourceInUseException("Country", countryId, usageCount);

            _countryRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<CountryResponse>> GetAllAsync()
        {
            var entities = await _countryRepository.GetAllOrderedAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
