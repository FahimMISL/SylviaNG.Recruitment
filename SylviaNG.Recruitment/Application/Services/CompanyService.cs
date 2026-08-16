using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.Companies.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class CompanyService : ICompanyService
    {
        private const string LogoStorageSubFolder = "company-logos";

        private readonly ICompanyRepository _companyRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IUnitOfWork _unitOfWork;

        public CompanyService(ICompanyRepository companyRepository, IFileStorageService fileStorageService, IUnitOfWork unitOfWork)
        {
            _companyRepository = companyRepository;
            _fileStorageService = fileStorageService;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(CompanyCreateRequest request)
        {
            var exists = await _companyRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("Company", "Name", request.Name);

            var entity = request.ToEntity();
            await _companyRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.CompanyId;
        }

        public async Task UpdateAsync(long companyId, CompanyUpdateRequest request)
        {
            var entity = await _companyRepository.GetByIdAsync(companyId)
                ?? throw new NotFoundException("Company", companyId);

            var nameTaken = await _companyRepository.ExistsByNameAsync(request.Name, companyId);
            if (nameTaken)
                throw new DuplicateException("Company", "Name", request.Name);

            entity.Name = request.Name;
            entity.Email = request.Email;
            entity.Phone = request.Phone;
            entity.Address = request.Address;
            entity.Website = request.Website;
            entity.Industry = request.Industry;

            _companyRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SetActiveAsync(long companyId, bool isActive)
        {
            var entity = await _companyRepository.GetByIdAsync(companyId)
                ?? throw new NotFoundException("Company", companyId);

            entity.Status = isActive ? CompanyStatusEnum.Active : CompanyStatusEnum.Inactive;
            _companyRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<CompanyResponse> GetByIdAsync(long companyId)
        {
            var entity = await _companyRepository.GetByIdAsync(companyId)
                ?? throw new NotFoundException("Company", companyId);

            var (jobPostingCount, userAccountCount) = await _companyRepository.GetStatsAsync(companyId);
            return entity.ToResponse(jobPostingCount, userAccountCount);
        }

        public async Task<List<CompanyResponse>> GetAllAsync()
        {
            var entities = await _companyRepository.GetAllOrderedAsync();
            var responses = new List<CompanyResponse>(entities.Count);

            foreach (var entity in entities)
            {
                var (jobPostingCount, userAccountCount) = await _companyRepository.GetStatsAsync(entity.CompanyId);
                responses.Add(entity.ToResponse(jobPostingCount, userAccountCount));
            }

            return responses;
        }

        public async Task<string> UploadLogoAsync(long companyId, IFormFile file)
        {
            var entity = await _companyRepository.GetByIdAsync(companyId)
                ?? throw new NotFoundException("Company", companyId);

            var oldFilePath = entity.LogoFilePath;

            await using var stream = file.OpenReadStream();
            var (storedFileName, filePath) = await _fileStorageService.SaveAsync(stream, file.FileName, LogoStorageSubFolder);

            entity.LogoFileName = file.FileName;
            entity.LogoStoredFileName = storedFileName;
            entity.LogoFilePath = filePath;
            entity.LogoContentType = file.ContentType;

            _companyRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            if (!string.IsNullOrEmpty(oldFilePath))
                await _fileStorageService.DeleteAsync(oldFilePath);

            return filePath;
        }
    }
}
