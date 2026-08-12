using ClosedXML.Excel;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Common.Email;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.SharedKernel.Utils;

namespace SylviaNG.Recruitment.Application.Services
{
    public class NotificationLogService : INotificationLogService
    {
        private readonly INotificationLogRepository _notificationLogRepository;
        private readonly ISmtpEmailService _smtpEmailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentCandidateService _currentCandidateService;

        public NotificationLogService(
            INotificationLogRepository notificationLogRepository,
            ISmtpEmailService smtpEmailService,
            IUnitOfWork unitOfWork,
            ICurrentCandidateService currentCandidateService)
        {
            _notificationLogRepository = notificationLogRepository;
            _smtpEmailService = smtpEmailService;
            _unitOfWork = unitOfWork;
            _currentCandidateService = currentCandidateService;
        }

        public async Task<PagedResult<NotificationLogResponse>> GetFilteredAsync(NotificationLogFilterRequest filter)
        {
            var query = _notificationLogRepository.GetFilteredQueryable(filter.FromDate, filter.ToDate, filter.Channel, filter.RecruitmentEvent, filter.DeliveryStatus);

            var paged = await query.ToPaginatedResultAsync(new PagedRequest { Page = filter.Page, PageSize = filter.PageSize });

            return new PagedResult<NotificationLogResponse>
            {
                Data = paged.Data.Select(l => l.ToResponse()).ToList(),
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount,
            };
        }

        public async Task<NotificationLogFileResponse> ExportExcelAsync(NotificationLogFilterRequest filter)
        {
            var entities = await _notificationLogRepository
                .GetFilteredQueryable(filter.FromDate, filter.ToDate, filter.Channel, filter.RecruitmentEvent, filter.DeliveryStatus)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Notification Log");

            string[] headers = { "Recipient Name", "Recipient Address", "Channel", "Event", "Subject", "Delivery Status", "Sent At", "Failure Reason" };
            for (var column = 0; column < headers.Length; column++)
                sheet.Cell(1, column + 1).Value = headers[column];
            sheet.Row(1).Style.Font.Bold = true;

            var rowIndex = 2;
            foreach (var entity in entities)
            {
                var response = entity.ToResponse();
                sheet.Cell(rowIndex, 1).Value = response.RecipientName;
                sheet.Cell(rowIndex, 2).Value = response.RecipientAddress;
                sheet.Cell(rowIndex, 3).Value = response.Channel.ToString();
                sheet.Cell(rowIndex, 4).Value = response.RecruitmentEvent.ToString();
                sheet.Cell(rowIndex, 5).Value = response.RenderedSubject ?? string.Empty;
                sheet.Cell(rowIndex, 6).Value = response.DeliveryStatus.ToString();
                sheet.Cell(rowIndex, 7).Value = response.SentAt.HasValue ? DateTimeUtility.ConvertUtcToLocal(response.SentAt.Value).ToString("yyyy-MM-dd hh:mm tt") : string.Empty;
                sheet.Cell(rowIndex, 8).Value = response.FailureReason ?? string.Empty;
                rowIndex++;
            }

            sheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return new NotificationLogFileResponse
            {
                Content = stream.ToArray(),
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                FileName = $"Notification-Log-{DateTime.UtcNow:yyyyMMdd-HHmmss}.xlsx",
            };
        }

        public async Task<List<NotificationLogResponse>> GetUnreadAsync()
        {
            var entities = await _notificationLogRepository.GetUnreadForAdminHrAsync(10);
            return entities.Select(l => l.ToResponse()).ToList();
        }

        public async Task<int> GetUnreadCountAsync()
        {
            return await _notificationLogRepository.GetUnreadCountForAdminHrAsync();
        }

        public async Task<NotificationLogResponse> RetryAsync(long notificationLogId)
        {
            var entity = await _notificationLogRepository.GetByIdAsync(notificationLogId)
                ?? throw new NotFoundException("NotificationLog", notificationLogId);

            if (entity.DeliveryStatus != NotificationStatusEnum.Failed)
                throw new ValidationException(new[]
                {
                    new ValidationFailure(nameof(entity.DeliveryStatus), "Only Failed notifications can be retried.")
                });

            var result = await _smtpEmailService.TrySendAsync(new EmailMessage
            {
                To = entity.RecipientAddress,
                Subject = entity.RenderedSubject ?? string.Empty,
                HtmlBody = entity.RenderedBody ?? string.Empty,
            });

            if (result.Success)
            {
                entity.DeliveryStatus = NotificationStatusEnum.Sent;
                entity.SentAt = DateTime.UtcNow;
                entity.FailureReason = null;
            }
            else
            {
                entity.FailureReason = result.ErrorMessage;
            }

            _notificationLogRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.ToResponse();
        }

        public async Task MarkAsReadAsync(long notificationLogId)
        {
            var entity = await _notificationLogRepository.GetByIdAsync(notificationLogId)
                ?? throw new NotFoundException("NotificationLog", notificationLogId);

            if (entity.IsRead)
                return;

            entity.IsRead = true;
            entity.ReadAt = DateTime.UtcNow;
            _notificationLogRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> MarkAllAsReadAsync()
        {
            return await _notificationLogRepository.MarkAllAsReadForAdminHrAsync();
        }

        public async Task<List<NotificationLogResponse>> GetUnreadForCurrentCandidateAsync()
        {
            var candidateProfileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            var entities = await _notificationLogRepository.GetUnreadForCandidateAsync(candidateProfileId, 10);
            return entities.Select(l => l.ToResponse()).ToList();
        }

        public async Task<int> GetUnreadCountForCurrentCandidateAsync()
        {
            var candidateProfileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            return await _notificationLogRepository.GetUnreadCountForCandidateAsync(candidateProfileId);
        }

        public async Task<int> MarkAllAsReadForCurrentCandidateAsync()
        {
            var candidateProfileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            return await _notificationLogRepository.MarkAllAsReadForCandidateAsync(candidateProfileId);
        }

        public async Task MarkAsReadForCurrentCandidateAsync(long notificationLogId)
        {
            var candidateProfileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();

            if (!await _notificationLogRepository.IsOwnedByCandidateAsync(notificationLogId, candidateProfileId))
                throw new NotFoundException("NotificationLog", notificationLogId);

            var entity = await _notificationLogRepository.GetByIdAsync(notificationLogId)
                ?? throw new NotFoundException("NotificationLog", notificationLogId);

            if (entity.IsRead)
                return;

            entity.IsRead = true;
            entity.ReadAt = DateTime.UtcNow;
            _notificationLogRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
