using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class NotificationTemplateService : INotificationTemplateService
    {
        private readonly INotificationTemplateRepository _notificationTemplateRepository;
        private readonly IUnitOfWork _unitOfWork;

        public NotificationTemplateService(INotificationTemplateRepository notificationTemplateRepository, IUnitOfWork unitOfWork)
        {
            _notificationTemplateRepository = notificationTemplateRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(NotificationTemplateCreateRequest request)
        {
            var exists = await _notificationTemplateRepository.ExistsByCodeAsync(request.Code);
            if (exists)
                throw new DuplicateException("NotificationTemplate", "Code", request.Code);

            var entity = request.ToEntity();
            await _notificationTemplateRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            await _notificationTemplateRepository.AddVersionAsync(new NotificationTemplateVersion
            {
                NotificationTemplateId = entity.NotificationTemplateId,
                VersionNumber = 1,
                Subject = entity.Subject,
                Body = entity.Body,
            });
            await _unitOfWork.SaveChangesAsync();

            return entity.NotificationTemplateId;
        }

        public async Task UpdateAsync(long notificationTemplateId, NotificationTemplateUpdateRequest request)
        {
            var entity = await _notificationTemplateRepository.GetByIdAsync(notificationTemplateId)
                ?? throw new NotFoundException("NotificationTemplate", notificationTemplateId);

            entity.Name = request.Name;
            entity.Subject = request.Subject;
            entity.Body = request.Body;
            entity.IsActive = request.IsActive;
            entity.CurrentVersionNumber += 1;
            _notificationTemplateRepository.Update(entity);

            await _notificationTemplateRepository.AddVersionAsync(new NotificationTemplateVersion
            {
                NotificationTemplateId = entity.NotificationTemplateId,
                VersionNumber = entity.CurrentVersionNumber,
                Subject = entity.Subject,
                Body = entity.Body,
            });

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long notificationTemplateId)
        {
            var entity = await _notificationTemplateRepository.GetByIdAsync(notificationTemplateId)
                ?? throw new NotFoundException("NotificationTemplate", notificationTemplateId);

            var usageCount = await _notificationTemplateRepository.CountMappingUsageAsync(notificationTemplateId);
            if (usageCount > 0)
                throw new ResourceInUseException("NotificationTemplate", notificationTemplateId, usageCount);

            _notificationTemplateRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<NotificationTemplateResponse>> GetAllAsync()
        {
            var entities = await _notificationTemplateRepository.GetAllOrderedAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<NotificationTemplateResponse> GetByIdAsync(long notificationTemplateId)
        {
            var entity = await _notificationTemplateRepository.GetByIdAsync(notificationTemplateId)
                ?? throw new NotFoundException("NotificationTemplate", notificationTemplateId);

            return entity.ToResponse();
        }

        public async Task<List<NotificationTemplateVersionResponse>> GetVersionsAsync(long notificationTemplateId)
        {
            var exists = await _notificationTemplateRepository.GetByIdAsync(notificationTemplateId)
                ?? throw new NotFoundException("NotificationTemplate", notificationTemplateId);

            var versions = await _notificationTemplateRepository.GetVersionsOrderedAsync(notificationTemplateId);
            return versions.Select(v => v.ToResponse()).ToList();
        }
    }
}
