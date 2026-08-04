using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Common.Helpers;
using SylviaNG.Recruitment.Application.Features.CompanyBranding.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class CompanyBrandingService : ICompanyBrandingService
    {
        private const string LogoStorageSubFolder = "company-branding";

        private readonly ICompanyBrandingRepository _companyBrandingRepository;
        private readonly IBrandingResolverService _brandingResolverService;
        private readonly IBrandingPreviewPdfGeneratorService _brandingPreviewPdfGeneratorService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IUnitOfWork _unitOfWork;

        public CompanyBrandingService(
            ICompanyBrandingRepository companyBrandingRepository,
            IBrandingResolverService brandingResolverService,
            IBrandingPreviewPdfGeneratorService brandingPreviewPdfGeneratorService,
            IFileStorageService fileStorageService,
            IUnitOfWork unitOfWork)
        {
            _companyBrandingRepository = companyBrandingRepository;
            _brandingResolverService = brandingResolverService;
            _brandingPreviewPdfGeneratorService = brandingPreviewPdfGeneratorService;
            _fileStorageService = fileStorageService;
            _unitOfWork = unitOfWork;
        }

        public async Task<CompanyBrandingResponse> GetAsync()
        {
            var entity = await _brandingResolverService.GetActiveBrandingAsync();
            return ToResponse(entity);
        }

        public async Task UpdateAsync(CompanyBrandingUpdateRequest request)
        {
            var (entity, isNew) = await GetOrCreateEntityAsync();

            entity.CompanyName = request.CompanyName;
            entity.AddressLine = request.AddressLine;
            entity.Phone = request.Phone;
            entity.Email = request.Email;
            entity.Website = request.Website;
            entity.PrimaryColor = request.PrimaryColor;
            entity.SecondaryColor = request.SecondaryColor;
            entity.AccentColor = request.AccentColor;
            entity.FontFamily = request.FontFamily;
            entity.BorderStyle = request.BorderStyle;
            entity.BackgroundWatermarkEnabled = request.BackgroundWatermarkEnabled;
            entity.WatermarkOpacity = request.WatermarkOpacity;
            entity.ShowPageNumbers = request.ShowPageNumbers;

            if (isNew)
                await _companyBrandingRepository.AddAsync(entity);
            else
                _companyBrandingRepository.Update(entity);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<string> UploadLogoAsync(IFormFile file)
        {
            var (entity, isNew) = await GetOrCreateEntityAsync();
            var oldFilePath = entity.LogoFilePath;

            await using var stream = file.OpenReadStream();
            var (storedFileName, filePath) = await _fileStorageService.SaveAsync(stream, file.FileName, LogoStorageSubFolder);

            entity.LogoFileName = file.FileName;
            entity.LogoStoredFileName = storedFileName;
            entity.LogoFilePath = filePath;
            entity.LogoContentType = file.ContentType;

            if (isNew)
                await _companyBrandingRepository.AddAsync(entity);
            else
                _companyBrandingRepository.Update(entity);

            await _unitOfWork.SaveChangesAsync();

            if (!string.IsNullOrEmpty(oldFilePath))
                await _fileStorageService.DeleteAsync(oldFilePath);

            return FileUrlBuilder.BuildDownloadUrl(filePath) ?? filePath;
        }

        public async Task<byte[]> GeneratePreviewPdfAsync(CompanyBrandingUpdateRequest request)
        {
            var current = await _brandingResolverService.GetActiveBrandingAsync();

            var draft = new Domain.Entities.CompanyBranding
            {
                TenantId = current.TenantId,
                LogoFilePath = current.LogoFilePath,
                CompanyName = request.CompanyName,
                AddressLine = request.AddressLine,
                Phone = request.Phone,
                Email = request.Email,
                Website = request.Website,
                PrimaryColor = request.PrimaryColor,
                SecondaryColor = request.SecondaryColor,
                AccentColor = request.AccentColor,
                FontFamily = request.FontFamily,
                HeaderLayout = current.HeaderLayout,
                FooterLayout = current.FooterLayout,
                MarginTop = current.MarginTop,
                MarginBottom = current.MarginBottom,
                MarginLeft = current.MarginLeft,
                MarginRight = current.MarginRight,
                BorderStyle = request.BorderStyle,
                CornerRadius = current.CornerRadius,
                BackgroundWatermarkEnabled = request.BackgroundWatermarkEnabled,
                WatermarkOpacity = request.WatermarkOpacity,
                HeaderDividerStyle = current.HeaderDividerStyle,
                FooterDividerStyle = current.FooterDividerStyle,
                QrCodePosition = current.QrCodePosition,
                SignaturePosition = current.SignaturePosition,
                SealPosition = current.SealPosition,
                DocumentReferenceFormat = current.DocumentReferenceFormat,
                ShowPageNumbers = request.ShowPageNumbers,
            };

            return await _brandingPreviewPdfGeneratorService.Generate(draft);
        }

        private async Task<(Domain.Entities.CompanyBranding Entity, bool IsNew)> GetOrCreateEntityAsync()
        {
            var current = await _brandingResolverService.GetActiveBrandingAsync();
            var existing = await _companyBrandingRepository.GetByTenantIdAsync(current.TenantId);
            return existing != null ? (existing, false) : (current, true);
        }

        private static CompanyBrandingResponse ToResponse(Domain.Entities.CompanyBranding entity) => new()
        {
            LogoFilePath = FileUrlBuilder.BuildDownloadUrl(entity.LogoFilePath),
            CompanyName = entity.CompanyName,
            AddressLine = entity.AddressLine,
            Phone = entity.Phone,
            Email = entity.Email,
            Website = entity.Website,
            PrimaryColor = entity.PrimaryColor,
            SecondaryColor = entity.SecondaryColor,
            AccentColor = entity.AccentColor,
            FontFamily = entity.FontFamily,
            BorderStyle = entity.BorderStyle,
            BackgroundWatermarkEnabled = entity.BackgroundWatermarkEnabled,
            WatermarkOpacity = entity.WatermarkOpacity,
            ShowPageNumbers = entity.ShowPageNumbers,
        };
    }
}
