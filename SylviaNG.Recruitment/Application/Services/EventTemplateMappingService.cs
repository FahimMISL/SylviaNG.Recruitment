using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.EventTemplateMappings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class EventTemplateMappingService : IEventTemplateMappingService
    {
        private readonly IEventTemplateMappingRepository _eventTemplateMappingRepository;
        private readonly INotificationTemplateRepository _notificationTemplateRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EventTemplateMappingService(
            IEventTemplateMappingRepository eventTemplateMappingRepository,
            INotificationTemplateRepository notificationTemplateRepository,
            IUnitOfWork unitOfWork)
        {
            _eventTemplateMappingRepository = eventTemplateMappingRepository;
            _notificationTemplateRepository = notificationTemplateRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(EventTemplateMappingCreateRequest request)
        {
            await EnsureTemplateChannelMatchesAsync(request.NotificationTemplateId, request.Channel);

            var exists = await _eventTemplateMappingRepository.ExistsAsync(request.RecruitmentEvent, request.Channel, request.RecipientType);
            if (exists)
            {
                throw new DuplicateException(
                    "EventTemplateMapping",
                    "Event+Channel+RecipientType",
                    $"{request.RecruitmentEvent}/{request.Channel}/{request.RecipientType}");
            }

            var entity = request.ToEntity();
            await _eventTemplateMappingRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.EventTemplateMappingId;
        }

        public async Task UpdateAsync(long eventTemplateMappingId, EventTemplateMappingUpdateRequest request)
        {
            var entity = await _eventTemplateMappingRepository.GetByIdAsync(eventTemplateMappingId)
                ?? throw new NotFoundException("EventTemplateMapping", eventTemplateMappingId);

            await EnsureTemplateChannelMatchesAsync(request.NotificationTemplateId, entity.Channel);

            entity.NotificationTemplateId = request.NotificationTemplateId;
            entity.IsActive = request.IsActive;
            _eventTemplateMappingRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long eventTemplateMappingId)
        {
            var entity = await _eventTemplateMappingRepository.GetByIdAsync(eventTemplateMappingId)
                ?? throw new NotFoundException("EventTemplateMapping", eventTemplateMappingId);

            _eventTemplateMappingRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<EventTemplateMappingResponse>> GetAllAsync()
        {
            var entities = await _eventTemplateMappingRepository.GetAllWithTemplateAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        private async Task EnsureTemplateChannelMatchesAsync(long notificationTemplateId, Domain.Enums.NotificationChannelEnum channel)
        {
            var template = await _notificationTemplateRepository.GetByIdAsync(notificationTemplateId)
                ?? throw new NotFoundException("NotificationTemplate", notificationTemplateId);

            if (template.Channel != channel)
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(EventTemplateMappingCreateRequest.NotificationTemplateId),
                        $"Template \"{template.Name}\" is a {template.Channel} template and cannot be mapped to a {channel} event.")
                });
            }
        }
    }
}
