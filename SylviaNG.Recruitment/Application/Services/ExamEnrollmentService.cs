using Microsoft.Extensions.Logging;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class ExamEnrollmentService : IExamEnrollmentService
    {
        private readonly IExamRepository _examRepository;
        private readonly IExamEnrollmentRepository _examEnrollmentRepository;
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly IExamRoomRepository _examRoomRepository;
        private readonly IExamNotificationService _examNotificationService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ExamEnrollmentService> _logger;

        public ExamEnrollmentService(
            IExamRepository examRepository,
            IExamEnrollmentRepository examEnrollmentRepository,
            IJobApplicationRepository jobApplicationRepository,
            IExamRoomRepository examRoomRepository,
            IExamNotificationService examNotificationService,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork,
            ILogger<ExamEnrollmentService> logger)
        {
            _examRepository = examRepository;
            _examEnrollmentRepository = examEnrollmentRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _examRoomRepository = examRoomRepository;
            _examNotificationService = examNotificationService;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<long>> EnrollAsync(long examId, List<long> jobApplicationIds)
        {
            var exam = await _examRepository.GetByIdWithDetailsAsync(examId)
                ?? throw new NotFoundException("Exam", examId);

            var newEnrollments = new List<(ExamEnrollment Enrollment, JobApplication JobApplication)>();

            foreach (var jobApplicationId in jobApplicationIds.Distinct())
            {
                var jobApplication = await _jobApplicationRepository.GetByIdAsync(jobApplicationId)
                    ?? throw new NotFoundException("JobApplication", jobApplicationId);

                if (jobApplication.JobPostingId != exam.JobPostingId)
                    throw new InvalidStatusTransitionException(
                        $"JobApplication {jobApplicationId} does not belong to the job posting this exam ({examId}) is for.");

                if (jobApplication.ApplicationStatus != ApplicationStatusEnum.Shortlisted)
                    throw new InvalidStatusTransitionException(
                        $"JobApplication {jobApplicationId} must be Shortlisted to be enrolled in an exam (current status: {jobApplication.ApplicationStatus}).");

                var alreadyEnrolled = await _examEnrollmentRepository.ExistsByExamAndJobApplicationAsync(examId, jobApplicationId);
                if (alreadyEnrolled)
                    throw new DuplicateException("ExamEnrollment", "JobApplicationId", jobApplicationId.ToString());

                var enrollment = new ExamEnrollment
                {
                    ExamId = examId,
                    JobApplicationId = jobApplicationId,
                    EnrolledAt = DateTime.UtcNow,
                };

                await _examEnrollmentRepository.AddAsync(enrollment);
                newEnrollments.Add((enrollment, jobApplication));
            }

            await _unitOfWork.SaveChangesAsync();

            var enrollmentIds = new List<long>();

            foreach (var (enrollment, jobApplication) in newEnrollments)
            {
                enrollmentIds.Add(enrollment.ExamEnrollmentId);

                // Double-safety belt-and-suspenders: ExamNotificationService already never throws
                // internally, but this is the one place a whole HR bulk action must not fail
                // because of a mail server, so we wrap it again here regardless.
                try
                {
                    await _examNotificationService.NotifyEnrollmentAsync(enrollment, exam, jobApplication);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Notification failed for ExamEnrollmentId {ExamEnrollmentId} - continuing with remaining enrollments.", enrollment.ExamEnrollmentId);
                }
            }

            return enrollmentIds;
        }

        public async Task<List<ExamEnrollmentResponse>> GetByExamIdAsync(long examId)
        {
            var entities = await _examEnrollmentRepository.GetByExamIdAsync(examId);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task ReassignSeatAsync(long examEnrollmentId, long examRoomId, string seatNumber)
        {
            var enrollment = await _examEnrollmentRepository.GetByIdWithDetailsAsync(examEnrollmentId)
                ?? throw new NotFoundException("ExamEnrollment", examEnrollmentId);

            var room = await _examRoomRepository.GetByIdAsync(examRoomId)
                ?? throw new NotFoundException("ExamRoom", examRoomId);

            if (room.ExamVenueId != enrollment.Exam.ExamVenueId)
                throw new InvalidStatusTransitionException(
                    $"ExamRoom {examRoomId} does not belong to the venue for exam {enrollment.ExamId}.");

            var seatTaken = await _examEnrollmentRepository.ExistsBySeatNumberAsync(enrollment.ExamId, seatNumber, examEnrollmentId);
            if (seatTaken)
                throw new DuplicateException("ExamEnrollment", "SeatNumber", seatNumber);

            var currentOccupants = await _examEnrollmentRepository.GetCountByRoomIdAsync(examRoomId);
            var isMovingIntoRoom = enrollment.ExamRoomId != examRoomId;
            if (isMovingIntoRoom && currentOccupants >= room.Capacity)
                throw new InvalidStatusTransitionException(
                    $"ExamRoom {examRoomId} ({room.RoomName}) is at capacity ({room.Capacity}).");

            enrollment.ExamRoomId = examRoomId;
            enrollment.SeatNumber = seatNumber;

            _examEnrollmentRepository.Update(enrollment);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UploadScoreAsync(long examEnrollmentId, decimal score)
        {
            var enrollment = await _examEnrollmentRepository.GetByIdWithDetailsAsync(examEnrollmentId)
                ?? throw new NotFoundException("ExamEnrollment", examEnrollmentId);

            if (score < 0 || score > enrollment.Exam.TotalMarks)
                throw new InvalidStatusTransitionException(
                    $"Score must be between 0 and the exam's total marks ({enrollment.Exam.TotalMarks}).");

            enrollment.Score = score;
            enrollment.IsPassed = score >= enrollment.Exam.PassMarks;
            enrollment.ScoreSource = ScoreSourceEnum.ManualUpload;
            enrollment.ScoredAt = DateTime.UtcNow;
            enrollment.ScoredByUserName = _currentUserService.GetCurrentUserName();

            _examEnrollmentRepository.Update(enrollment);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
