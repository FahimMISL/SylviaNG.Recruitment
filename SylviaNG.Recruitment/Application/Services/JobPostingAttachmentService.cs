using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.JobPostingAttachments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class JobPostingAttachmentService : IJobPostingAttachmentService
    {
        private readonly IJobPostingAttachmentRepository _attachmentRepository;
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IUnitOfWork _unitOfWork;

        public JobPostingAttachmentService(
            IJobPostingAttachmentRepository attachmentRepository,
            IJobPostingRepository jobPostingRepository,
            IFileStorageService fileStorageService,
            IUnitOfWork unitOfWork)
        {
            _attachmentRepository = attachmentRepository;
            _jobPostingRepository = jobPostingRepository;
            _fileStorageService = fileStorageService;
            _unitOfWork = unitOfWork;
        }

        public async Task<JobPostingAttachmentResponse> UploadAsync(long jobPostingId, IFormFile file)
        {
            _ = await _jobPostingRepository.GetByIdAsync(jobPostingId)
                ?? throw new NotFoundException("JobPosting", jobPostingId);

            await using var stream = file.OpenReadStream();
            var (storedFileName, filePath) = await _fileStorageService.SaveAsync(
                stream, file.FileName, jobPostingId.ToString());

            var entity = new JobPostingAttachment
            {
                JobPostingId = jobPostingId,
                FileName = file.FileName,
                StoredFileName = storedFileName,
                FilePath = filePath,
                ContentType = file.ContentType,
                FileSizeBytes = file.Length,
                IsActive = true
            };

            await _attachmentRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.ToResponse();
        }

        public async Task DeleteAsync(long jobPostingId, long attachmentId)
        {
            var entity = await _attachmentRepository.GetByIdAsync(attachmentId);
            if (entity == null || entity.JobPostingId != jobPostingId)
                throw new NotFoundException("JobPostingAttachment", attachmentId);

            _attachmentRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();

            await _fileStorageService.DeleteAsync(entity.FilePath);
        }

        public async Task<List<JobPostingAttachmentResponse>> GetAllByJobPostingIdAsync(long jobPostingId)
        {
            _ = await _jobPostingRepository.GetByIdAsync(jobPostingId)
                ?? throw new NotFoundException("JobPosting", jobPostingId);

            var entities = await _attachmentRepository.GetAllByJobPostingIdAsync(jobPostingId);
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
