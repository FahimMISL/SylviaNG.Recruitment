using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class NotificationLogService : INotificationLogService
    {
        private readonly INotificationLogRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public NotificationLogService(INotificationLogRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(NotificationLogCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.NotificationLogId;
        }

        public async Task UpdateAsync(long notificationLogId, NotificationLogUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(notificationLogId)
                ?? throw new KeyNotFoundException($"NotificationLog with ID {notificationLogId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long notificationLogId)
        {
            var entity = await _repository.GetByIdAsync(notificationLogId)
                ?? throw new KeyNotFoundException($"NotificationLog with ID {notificationLogId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<NotificationLogResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<NotificationLogResponse> GetByIdAsync(long notificationLogId)
        {
            var entity = await _repository.GetByIdAsync(notificationLogId)
                ?? throw new KeyNotFoundException($"NotificationLog with ID {notificationLogId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<NotificationLogResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<NotificationLogResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
