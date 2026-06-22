using SylviaNG.Recruitment.Application.Features.Candidates.Models;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class CandidateMapper
    {
        public static Candidate ToEntity(this CandidateCreateRequest request)
        {
            return new Candidate
            {
                KeycloakUserId = request.KeycloakUserId,
                CandidateType = request.CandidateType,
                EmployeeId = request.EmployeeId,
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                DateOfBirth = request.DateOfBirth,
                NationalIdNumber = request.NationalIdNumber,
                Gender = request.Gender,
                Address = request.Address,
                City = request.City,
                Country = request.Country,
                ProfilePhotoUrl = request.ProfilePhotoUrl,
                CurrentDesignation = request.CurrentDesignation,
                CurrentOrganization = request.CurrentOrganization,
                TotalExperienceYears = request.TotalExperienceYears,
                ExpectedSalary = request.ExpectedSalary,
                ProfileCompletenessPercent = request.ProfileCompletenessPercent,
                IsEmailVerified = request.IsEmailVerified,
                IsPhoneVerified = request.IsPhoneVerified,
                PresentAddress = request.PresentAddress,
                PermanentAddress = request.PermanentAddress,
                LinkedInUrl = request.LinkedInUrl,
                GitHubUrl = request.GitHubUrl,
                PortfolioUrl = request.PortfolioUrl,
                FatherName = request.FatherName,
                MotherName = request.MotherName,
                MaritalStatus = request.MaritalStatus,
                Religion = request.Religion,
                BloodGroup = request.BloodGroup,
            };
        }

        public static void ApplyUpdate(this Candidate entity, CandidateUpdateRequest request)
        {
            if (request.KeycloakUserId is not null) entity.KeycloakUserId = request.KeycloakUserId;
            if (request.CandidateType.HasValue) entity.CandidateType = request.CandidateType.Value;
            if (request.EmployeeId.HasValue) entity.EmployeeId = request.EmployeeId.Value;
            if (request.FullName is not null) entity.FullName = request.FullName;
            if (request.Email is not null) entity.Email = request.Email;
            if (request.Phone is not null) entity.Phone = request.Phone;
            if (request.DateOfBirth.HasValue) entity.DateOfBirth = request.DateOfBirth;
            if (request.NationalIdNumber is not null) entity.NationalIdNumber = request.NationalIdNumber;
            if (request.Gender is not null) entity.Gender = request.Gender;
            if (request.Address is not null) entity.Address = request.Address;
            if (request.City is not null) entity.City = request.City;
            if (request.Country is not null) entity.Country = request.Country;
            if (request.ProfilePhotoUrl is not null) entity.ProfilePhotoUrl = request.ProfilePhotoUrl;
            if (request.CurrentDesignation is not null) entity.CurrentDesignation = request.CurrentDesignation;
            if (request.CurrentOrganization is not null) entity.CurrentOrganization = request.CurrentOrganization;
            if (request.TotalExperienceYears.HasValue) entity.TotalExperienceYears = request.TotalExperienceYears.Value;
            if (request.ExpectedSalary is not null) entity.ExpectedSalary = request.ExpectedSalary;
            if (request.ProfileCompletenessPercent.HasValue) entity.ProfileCompletenessPercent = request.ProfileCompletenessPercent.Value;
            if (request.IsEmailVerified.HasValue) entity.IsEmailVerified = request.IsEmailVerified.Value;
            if (request.IsPhoneVerified.HasValue) entity.IsPhoneVerified = request.IsPhoneVerified.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
            if (request.PresentAddress is not null) entity.PresentAddress = request.PresentAddress;
            if (request.PermanentAddress is not null) entity.PermanentAddress = request.PermanentAddress;
            if (request.LinkedInUrl is not null) entity.LinkedInUrl = request.LinkedInUrl;
            if (request.GitHubUrl is not null) entity.GitHubUrl = request.GitHubUrl;
            if (request.PortfolioUrl is not null) entity.PortfolioUrl = request.PortfolioUrl;
            if (request.FatherName is not null) entity.FatherName = request.FatherName;
            if (request.MotherName is not null) entity.MotherName = request.MotherName;
            if (request.MaritalStatus is not null) entity.MaritalStatus = request.MaritalStatus;
            if (request.Religion is not null) entity.Religion = request.Religion;
            if (request.BloodGroup is not null) entity.BloodGroup = request.BloodGroup;
        }

        public static CandidateResponse ToResponse(this Candidate entity)
        {
            return new CandidateResponse
            {
                CandidateId = entity.CandidateId,
                KeycloakUserId = entity.KeycloakUserId,
                CandidateType = entity.CandidateType,
                EmployeeId = entity.EmployeeId,
                FullName = entity.FullName,
                Email = entity.Email,
                Phone = entity.Phone,
                DateOfBirth = entity.DateOfBirth,
                NationalIdNumber = entity.NationalIdNumber,
                Gender = entity.Gender,
                Address = entity.Address,
                City = entity.City,
                Country = entity.Country,
                ProfilePhotoUrl = entity.ProfilePhotoUrl,
                CurrentDesignation = entity.CurrentDesignation,
                CurrentOrganization = entity.CurrentOrganization,
                TotalExperienceYears = entity.TotalExperienceYears,
                ExpectedSalary = entity.ExpectedSalary,
                ProfileCompletenessPercent = entity.ProfileCompletenessPercent,
                IsEmailVerified = entity.IsEmailVerified,
                IsPhoneVerified = entity.IsPhoneVerified,
                IsActive = entity.IsActive,
                PresentAddress = entity.PresentAddress,
                PermanentAddress = entity.PermanentAddress,
                LinkedInUrl = entity.LinkedInUrl,
                GitHubUrl = entity.GitHubUrl,
                PortfolioUrl = entity.PortfolioUrl,
                FatherName = entity.FatherName,
                MotherName = entity.MotherName,
                MaritalStatus = entity.MaritalStatus,
                Religion = entity.Religion,
                BloodGroup = entity.BloodGroup,
            };
        }
    }
}
