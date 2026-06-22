using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class NotificationTemplateService : INotificationTemplateService
    {
        private readonly INotificationTemplateRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public NotificationTemplateService(INotificationTemplateRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(NotificationTemplateCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.NotificationTemplateId;
        }

        public async Task UpdateAsync(long notificationTemplateId, NotificationTemplateUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(notificationTemplateId)
                ?? throw new KeyNotFoundException($"NotificationTemplate with ID {notificationTemplateId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long notificationTemplateId)
        {
            var entity = await _repository.GetByIdAsync(notificationTemplateId)
                ?? throw new KeyNotFoundException($"NotificationTemplate with ID {notificationTemplateId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<NotificationTemplateResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<NotificationTemplateResponse> GetByIdAsync(long notificationTemplateId)
        {
            var entity = await _repository.GetByIdAsync(notificationTemplateId)
                ?? throw new KeyNotFoundException($"NotificationTemplate with ID {notificationTemplateId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<NotificationTemplateResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<NotificationTemplateResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
