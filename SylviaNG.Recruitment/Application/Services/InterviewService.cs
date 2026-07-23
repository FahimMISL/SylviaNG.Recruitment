using Microsoft.Extensions.Logging;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.Interviews.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Services
{
    public class InterviewService : IInterviewService
    {
        private readonly IInterviewRepository _interviewRepository;
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly IInterviewVenueRepository _interviewVenueRepository;
        private readonly IInterviewRoomRepository _interviewRoomRepository;
        private readonly IInterviewRoundConfigRepository _interviewRoundConfigRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IInterviewNotificationService _interviewNotificationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InterviewService> _logger;

        public InterviewService(
            IInterviewRepository interviewRepository,
            IJobApplicationRepository jobApplicationRepository,
            IInterviewVenueRepository interviewVenueRepository,
            IInterviewRoomRepository interviewRoomRepository,
            IInterviewRoundConfigRepository interviewRoundConfigRepository,
            IEmployeeRepository employeeRepository,
            IInterviewNotificationService interviewNotificationService,
            IUnitOfWork unitOfWork,
            ILogger<InterviewService> logger)
        {
            _interviewRepository = interviewRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _interviewVenueRepository = interviewVenueRepository;
            _interviewRoomRepository = interviewRoomRepository;
            _interviewRoundConfigRepository = interviewRoundConfigRepository;
            _employeeRepository = employeeRepository;
            _interviewNotificationService = interviewNotificationService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<long> ScheduleAsync(InterviewScheduleRequest request)
        {
            var jobApplication = await _jobApplicationRepository.GetByIdAsync(request.JobApplicationId)
                ?? throw new NotFoundException("JobApplication", request.JobApplicationId);

            var entity = request.ToEntity();
            var panelistIds = request.PanelistEmployeeIds.Distinct().ToList();

            if (request.InterviewRoundConfigId.HasValue)
                await ApplyRoundConfigAsync(entity, jobApplication.JobPostingId, request.InterviewRoundConfigId.Value, request.JobApplicationId);

            await ResolveLocationAsync(entity, request.InterviewType, request.InterviewRoomId, request.MeetingLink);
            await ValidatePanelistsAsync(panelistIds);
            await CheckConflictsAsync(entity, panelistIds, excludeInterviewId: null);

            entity.PanelMembers = panelistIds.Select(id => new InterviewPanelMember { EmployeeId = id }).ToList();

            await _interviewRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            entity.JobApplication = jobApplication;
            if (entity.InterviewVenueId.HasValue)
                entity.InterviewVenue = await _interviewVenueRepository.GetByIdAsync(entity.InterviewVenueId.Value);
            if (entity.InterviewRoomId.HasValue)
                entity.InterviewRoom = await _interviewRoomRepository.GetByIdAsync(entity.InterviewRoomId.Value);

            await NotifySafelyAsync(entity, () => _interviewNotificationService.NotifyScheduledAsync(entity));

            return entity.InterviewId;
        }

        public async Task<List<long>> BulkScheduleAsync(InterviewBulkScheduleRequest request)
        {
            var slotLength = TimeSpan.FromMinutes(request.DurationMinutes);
            var gap = TimeSpan.FromMinutes(request.GapMinutes);
            var panelistIds = request.PanelistEmployeeIds.Distinct().ToList();
            await ValidatePanelistsAsync(panelistIds);

            var created = new List<Interview>();
            var slotStart = request.StartAt;

            foreach (var jobApplicationId in request.JobApplicationIds.Distinct())
            {
                var jobApplication = await _jobApplicationRepository.GetByIdAsync(jobApplicationId)
                    ?? throw new NotFoundException("JobApplication", jobApplicationId);

                var entity = new Interview
                {
                    JobApplicationId = jobApplicationId,
                    PipelineStageId = request.PipelineStageId,
                    InterviewType = request.InterviewType,
                    ScheduledStartAt = slotStart,
                    ScheduledEndAt = slotStart + slotLength,
                    Round = request.Round,
                    Notes = request.Notes,
                };

                if (request.InterviewRoundConfigId.HasValue)
                    await ApplyRoundConfigAsync(entity, jobApplication.JobPostingId, request.InterviewRoundConfigId.Value, jobApplicationId);

                await ResolveLocationAsync(entity, request.InterviewType, request.InterviewRoomId, request.MeetingLink);
                await CheckConflictsAsync(entity, panelistIds, excludeInterviewId: null);

                entity.PanelMembers = panelistIds.Select(id => new InterviewPanelMember { EmployeeId = id }).ToList();

                await _interviewRepository.AddAsync(entity);
                entity.JobApplication = jobApplication;
                created.Add(entity);

                slotStart = entity.ScheduledEndAt + gap;
            }

            await _unitOfWork.SaveChangesAsync();

            InterviewVenue? venue = null;
            InterviewRoom? room = null;
            if (request.InterviewType == InterviewTypeEnum.InPerson && request.InterviewRoomId.HasValue)
            {
                room = await _interviewRoomRepository.GetByIdAsync(request.InterviewRoomId.Value);
                if (room != null)
                    venue = await _interviewVenueRepository.GetByIdAsync(room.InterviewVenueId);
            }

            var interviewIds = new List<long>();
            foreach (var entity in created)
            {
                interviewIds.Add(entity.InterviewId);
                entity.InterviewVenue = venue;
                entity.InterviewRoom = room;

                await NotifySafelyAsync(entity, () => _interviewNotificationService.NotifyScheduledAsync(entity));
            }

            return interviewIds;
        }

        public async Task RescheduleAsync(long interviewId, InterviewRescheduleRequest request)
        {
            var interview = await _interviewRepository.GetByIdWithDetailsAsync(interviewId)
                ?? throw new NotFoundException("Interview", interviewId);

            if (interview.Status == InterviewStatusEnum.Cancelled)
                throw new InvalidStatusTransitionException("Interview", interview.Status, InterviewStatusEnum.Rescheduled);

            interview.ScheduledStartAt = request.ScheduledStartAt;
            interview.ScheduledEndAt = request.ScheduledEndAt;

            if (interview.InterviewType == InterviewTypeEnum.InPerson && request.InterviewRoomId.HasValue)
            {
                var room = await _interviewRoomRepository.GetByIdAsync(request.InterviewRoomId.Value)
                    ?? throw new NotFoundException("InterviewRoom", request.InterviewRoomId.Value);
                interview.InterviewRoomId = room.InterviewRoomId;
                interview.InterviewVenueId = room.InterviewVenueId;
                interview.InterviewRoom = room;
                interview.InterviewVenue = await _interviewVenueRepository.GetByIdAsync(room.InterviewVenueId);
            }
            else if (interview.InterviewType == InterviewTypeEnum.Virtual && !string.IsNullOrWhiteSpace(request.MeetingLink))
            {
                interview.MeetingLink = request.MeetingLink;
            }

            var panelistIds = interview.PanelMembers.Select(p => p.EmployeeId).ToList();
            await CheckConflictsAsync(interview, panelistIds, excludeInterviewId: interviewId);

            interview.Status = InterviewStatusEnum.Rescheduled;

            _interviewRepository.Update(interview);
            await _unitOfWork.SaveChangesAsync();

            await NotifySafelyAsync(interview, () => _interviewNotificationService.NotifyRescheduledAsync(interview));
        }

        public async Task BulkRescheduleAsync(InterviewBulkRescheduleRequest request)
        {
            var interviews = await _interviewRepository.GetByIdsWithDetailsAsync(request.InterviewIds.Distinct().ToList());
            var gap = TimeSpan.FromMinutes(request.GapMinutes);
            var slotStart = request.StartAt;

            foreach (var interview in interviews)
            {
                if (interview.Status == InterviewStatusEnum.Cancelled)
                    throw new InvalidStatusTransitionException("Interview", interview.Status, InterviewStatusEnum.Rescheduled);

                var duration = interview.ScheduledEndAt - interview.ScheduledStartAt;
                interview.ScheduledStartAt = slotStart;
                interview.ScheduledEndAt = slotStart + duration;
                interview.Status = InterviewStatusEnum.Rescheduled;

                var panelistIds = interview.PanelMembers.Select(p => p.EmployeeId).ToList();
                await CheckConflictsAsync(interview, panelistIds, excludeInterviewId: interview.InterviewId);

                _interviewRepository.Update(interview);
                slotStart = interview.ScheduledEndAt + gap;
            }

            await _unitOfWork.SaveChangesAsync();

            foreach (var interview in interviews)
                await NotifySafelyAsync(interview, () => _interviewNotificationService.NotifyRescheduledAsync(interview));
        }

        public async Task CancelAsync(long interviewId, InterviewCancelRequest request)
        {
            var interview = await _interviewRepository.GetByIdWithDetailsAsync(interviewId)
                ?? throw new NotFoundException("Interview", interviewId);

            if (interview.Status == InterviewStatusEnum.Cancelled)
                throw new InvalidStatusTransitionException("Interview", interview.Status, InterviewStatusEnum.Cancelled);

            interview.Status = InterviewStatusEnum.Cancelled;
            interview.CancellationReason = request.CancellationReason;

            _interviewRepository.Update(interview);
            await _unitOfWork.SaveChangesAsync();

            await NotifySafelyAsync(interview, () => _interviewNotificationService.NotifyCancelledAsync(interview));
        }

        public async Task BulkCancelAsync(InterviewBulkCancelRequest request)
        {
            var interviews = await _interviewRepository.GetByIdsWithDetailsAsync(request.InterviewIds.Distinct().ToList());

            foreach (var interview in interviews)
            {
                if (interview.Status == InterviewStatusEnum.Cancelled)
                    continue;

                interview.Status = InterviewStatusEnum.Cancelled;
                interview.CancellationReason = request.CancellationReason;
                _interviewRepository.Update(interview);
            }

            await _unitOfWork.SaveChangesAsync();

            foreach (var interview in interviews)
                await NotifySafelyAsync(interview, () => _interviewNotificationService.NotifyCancelledAsync(interview));
        }

        public async Task MarkResultAsync(long interviewId, InterviewMarkResultRequest request)
        {
            var interview = await _interviewRepository.GetByIdWithDetailsAsync(interviewId)
                ?? throw new NotFoundException("Interview", interviewId);

            if (interview.Status == InterviewStatusEnum.Cancelled)
                throw new InvalidStatusTransitionException("Interview", interview.Status, InterviewStatusEnum.Completed);

            interview.Result = request.Result;
            interview.Status = InterviewStatusEnum.Completed;

            _interviewRepository.Update(interview);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PagedResult<InterviewResponse>> GetPagedAsync(
            PagedRequest request,
            long? jobPostingId,
            InterviewStatusEnum? status,
            DateTime? dateFrom,
            DateTime? dateTo)
        {
            var paged = await _interviewRepository.GetPagedAsync(request, jobPostingId, status, dateFrom, dateTo);

            return new PagedResult<InterviewResponse>
            {
                Data = paged.Data.Select(i => i.ToResponse()).ToList(),
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount
            };
        }

        public async Task<InterviewResponse> GetByIdAsync(long interviewId)
        {
            var entity = await _interviewRepository.GetByIdWithDetailsAsync(interviewId)
                ?? throw new NotFoundException("Interview", interviewId);

            return entity.ToResponse();
        }

        public async Task<List<InterviewResponse>> GetByJobApplicationIdAsync(long jobApplicationId)
        {
            var entities = await _interviewRepository.GetByJobApplicationIdAsync(jobApplicationId);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        private async Task ResolveLocationAsync(Interview entity, InterviewTypeEnum type, long? interviewRoomId, string? meetingLink)
        {
            if (type == InterviewTypeEnum.InPerson)
            {
                if (!interviewRoomId.HasValue)
                    throw new InvalidStatusTransitionException("InterviewRoomId is required for an in-person interview.");

                var room = await _interviewRoomRepository.GetByIdAsync(interviewRoomId.Value)
                    ?? throw new NotFoundException("InterviewRoom", interviewRoomId.Value);

                // Venue is derived from the room, not taken from the caller, so the two can never
                // disagree - same "trust the child, ignore a possibly-stale parent id" precedent
                // as ExamEnrollmentService.ReassignSeatAsync's venue-consistency check.
                entity.InterviewRoomId = room.InterviewRoomId;
                entity.InterviewVenueId = room.InterviewVenueId;
                entity.MeetingLink = null;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(meetingLink))
                    throw new InvalidStatusTransitionException("MeetingLink is required for a virtual interview.");

                entity.InterviewRoomId = null;
                entity.InterviewVenueId = null;
                entity.MeetingLink = meetingLink;
            }
        }

        private async Task ApplyRoundConfigAsync(Interview entity, long jobPostingId, long roundConfigId, long jobApplicationId)
        {
            var config = await _interviewRoundConfigRepository.GetByIdWithDetailsAsync(roundConfigId)
                ?? throw new NotFoundException("InterviewRoundConfig", roundConfigId);

            if (config.JobPostingId != jobPostingId)
                throw new InvalidStatusTransitionException($"InterviewRoundConfig {roundConfigId} does not belong to this job posting.");

            if (config.Sequence > 1)
            {
                var priorConfig = await _interviewRoundConfigRepository.GetByJobPostingAndSequenceAsync(jobPostingId, config.Sequence - 1)
                    ?? throw new InvalidStatusTransitionException($"Round '{config.Name}' has no prior round configured to gate on.");

                var priorPassed = await _interviewRepository.ExistsPassedForRoundConfigAsync(jobApplicationId, priorConfig.InterviewRoundConfigId);
                if (!priorPassed)
                    throw new InvalidStatusTransitionException($"Candidate must pass round '{priorConfig.Name}' before scheduling '{config.Name}'.");
            }

            entity.Round = config.Sequence;
            entity.InterviewRoundConfigId = config.InterviewRoundConfigId;
        }

        private async Task ValidatePanelistsAsync(List<long> employeeIds)
        {
            foreach (var employeeId in employeeIds)
            {
                _ = await _employeeRepository.GetByIdAsync(employeeId)
                    ?? throw new NotFoundException("Employee", employeeId);
            }
        }

        private async Task CheckConflictsAsync(Interview entity, List<long> panelistEmployeeIds, long? excludeInterviewId)
        {
            if (entity.InterviewType == InterviewTypeEnum.InPerson && entity.InterviewRoomId.HasValue)
            {
                var room = await _interviewRoomRepository.GetByIdAsync(entity.InterviewRoomId.Value)
                    ?? throw new NotFoundException("InterviewRoom", entity.InterviewRoomId.Value);

                var overlapCount = await _interviewRepository.GetOverlappingCountByRoomIdAsync(
                    entity.InterviewRoomId.Value, entity.ScheduledStartAt, entity.ScheduledEndAt, excludeInterviewId);

                if (overlapCount >= room.Capacity)
                    throw new InvalidStatusTransitionException(
                        $"InterviewRoom {room.RoomName} is at capacity ({room.Capacity}) for {entity.ScheduledStartAt:dd MMM yyyy HH:mm}-{entity.ScheduledEndAt:HH:mm}.");
            }

            if (panelistEmployeeIds.Count > 0)
            {
                var conflicting = await _interviewRepository.GetConflictingPanelistEmployeeIdsAsync(
                    panelistEmployeeIds, entity.ScheduledStartAt, entity.ScheduledEndAt, excludeInterviewId);

                if (conflicting.Count > 0)
                    throw new InvalidStatusTransitionException(
                        $"Employee(s) {string.Join(", ", conflicting)} already have an overlapping interview at this time.");
            }
        }

        private async Task NotifySafelyAsync(Interview interview, Func<Task> notify)
        {
            // Double-safety belt-and-suspenders: InterviewNotificationService already never throws
            // internally, but this is the one place a bulk action must not fail because of a mail
            // server, so we wrap it again here regardless - same precedent as
            // ExamEnrollmentService.EnrollAsync.
            try
            {
                await notify();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Notification failed for InterviewId {InterviewId} - continuing.", interview.InterviewId);
            }
        }
    }
}
