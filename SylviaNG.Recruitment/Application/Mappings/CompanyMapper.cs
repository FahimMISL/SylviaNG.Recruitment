using SylviaNG.Recruitment.Application.Common.Helpers;
using SylviaNG.Recruitment.Application.Features.Companies.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class CompanyMapper
    {
        public static Company ToEntity(this CompanyCreateRequest request)
        {
            return new Company
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                Website = request.Website,
                Industry = request.Industry,
                TradeLicenseNumber = request.TradeLicenseNumber,
                BinNumber = request.BinNumber,
            };
        }

        public static CompanyResponse ToResponse(this Company entity, int jobPostingCount = 0, int userAccountCount = 0)
        {
            return new CompanyResponse
            {
                CompanyId = entity.CompanyId,
                Name = entity.Name,
                LogoUrl = FileUrlBuilder.BuildDownloadUrl(entity.LogoFilePath),
                Email = entity.Email,
                Phone = entity.Phone,
                Address = entity.Address,
                Website = entity.Website,
                Industry = entity.Industry,
                TradeLicenseNumber = entity.TradeLicenseNumber,
                BinNumber = entity.BinNumber,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt,
                JobPostingCount = jobPostingCount,
                UserAccountCount = userAccountCount,
            };
        }
    }
}
