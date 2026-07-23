using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.ExamTaking.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    /// <summary>US-058: candidate-facing online exam attempt - list/start/submit. Auto-scores
    /// McqSingle/McqMultiple/TrueFalse (exact-set-match, all-or-nothing per question); Subjective
    /// answers are stored but left ungraded (IsCorrect/MarksAwarded null), flagged for HR to
    /// finalize via the US-059 manual-score-upload path, which overwrites Score/IsPassed.</summary>
    public class ExamTakingService : IExamTakingService
    {
        private readonly IExamEnrollmentRepository _examEnrollmentRepository;
        private readonly IExamQuestionRepository _examQuestionRepository;
        private readonly IExamAnswerRepository _examAnswerRepository;
        private readonly ICurrentCandidateService _currentCandidateService;
        private readonly IUnitOfWork _unitOfWork;

        public ExamTakingService(
            IExamEnrollmentRepository examEnrollmentRepository,
            IExamQuestionRepository examQuestionRepository,
            IExamAnswerRepository examAnswerRepository,
            ICurrentCandidateService currentCandidateService,
            IUnitOfWork unitOfWork)
        {
            _examEnrollmentRepository = examEnrollmentRepository;
            _examQuestionRepository = examQuestionRepository;
            _examAnswerRepository = examAnswerRepository;
            _currentCandidateService = currentCandidateService;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<MyExamEnrollmentResponse>> GetMyEnrollmentsAsync()
        {
            var candidateProfileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            var enrollments = await _examEnrollmentRepository.GetByCandidateProfileIdAsync(candidateProfileId);

            return enrollments.Select(e =>
            {
                var attemptStatus = e.SubmittedAt != null
                    ? ExamAttemptStatusEnum.Submitted
                    : e.StartedAt != null
                        ? ExamAttemptStatusEnum.InProgress
                        : ExamAttemptStatusEnum.NotStarted;

                var resultsVisible = e.Exam.ShowResultsToCandidate && e.SubmittedAt != null;

                return new MyExamEnrollmentResponse
                {
                    ExamEnrollmentId = e.ExamEnrollmentId,
                    JobApplicationId = e.JobApplicationId,
                    JobPostingId = e.Exam.JobPostingId,
                    JobPostingTitle = e.Exam.JobPosting?.Title ?? string.Empty,
                    ExamId = e.ExamId,
                    ExamTitle = e.Exam.Title,
                    ExamType = e.Exam.ExamType,
                    ScheduledStartAt = e.Exam.ScheduledStartAt,
                    DurationMinutes = e.Exam.DurationMinutes,
                    TotalMarks = e.Exam.TotalMarks,
                    PassMarks = e.Exam.PassMarks,
                    AttemptStatus = attemptStatus,
                    Score = resultsVisible ? e.Score : null,
                    IsPassed = resultsVisible ? e.IsPassed : null,
                    ResultsVisible = resultsVisible,
                };
            }).ToList();
        }

        public async Task<ExamPaperResponse> StartExamAsync(long examEnrollmentId)
        {
            var enrollment = await GetOwnedEnrollmentAsync(examEnrollmentId);

            if (enrollment.Exam.ExamType != ExamTypeEnum.Online)
                throw new InvalidStatusTransitionException("Only online exams can be started here.");

            if (enrollment.SubmittedAt != null)
                throw new InvalidStatusTransitionException("This exam has already been submitted.");

            if (DateTime.UtcNow < enrollment.Exam.ScheduledStartAt)
                throw new InvalidStatusTransitionException("This exam has not started yet.");

            if (enrollment.StartedAt == null)
            {
                enrollment.StartedAt = DateTime.UtcNow;
                _examEnrollmentRepository.Update(enrollment);
                await _unitOfWork.SaveChangesAsync();
            }

            var questions = await _examQuestionRepository.GetActiveByQuestionGroupIdAsync(enrollment.Exam.QuestionGroupId!.Value);

            return new ExamPaperResponse
            {
                ExamEnrollmentId = enrollment.ExamEnrollmentId,
                ExamTitle = enrollment.Exam.Title,
                DurationMinutes = enrollment.Exam.DurationMinutes,
                StartedAt = enrollment.StartedAt.Value,
                DeadlineAt = enrollment.StartedAt.Value.AddMinutes(enrollment.Exam.DurationMinutes),
                Questions = questions.Select(q => new ExamPaperQuestionResponse
                {
                    ExamQuestionId = q.ExamQuestionId,
                    QuestionText = q.QuestionText,
                    QuestionType = q.QuestionType,
                    Marks = q.Marks,
                    Options = q.Options
                        .OrderBy(o => o.DisplayOrder)
                        .Select(o => new ExamPaperOptionResponse
                        {
                            ExamQuestionOptionId = o.ExamQuestionOptionId,
                            OptionText = o.OptionText,
                        }).ToList(),
                }).ToList(),
            };
        }

        public async Task<ExamSubmitResultResponse> SubmitExamAsync(long examEnrollmentId, ExamSubmitRequest request)
        {
            var enrollment = await GetOwnedEnrollmentAsync(examEnrollmentId);

            if (enrollment.StartedAt == null)
                throw new InvalidStatusTransitionException("This exam has not been started.");

            if (enrollment.SubmittedAt != null)
                throw new InvalidStatusTransitionException("This exam has already been submitted.");

            var questions = await _examQuestionRepository.GetActiveByQuestionGroupIdAsync(enrollment.Exam.QuestionGroupId!.Value);
            var answersByQuestionId = request.Answers
                .GroupBy(a => a.ExamQuestionId)
                .ToDictionary(g => g.Key, g => g.First());

            var examAnswers = new List<ExamAnswer>();
            decimal totalScore = 0;

            foreach (var question in questions)
            {
                answersByQuestionId.TryGetValue(question.ExamQuestionId, out var answer);

                if (question.QuestionType == QuestionTypeEnum.Subjective)
                {
                    examAnswers.Add(new ExamAnswer
                    {
                        ExamEnrollmentId = enrollment.ExamEnrollmentId,
                        ExamQuestionId = question.ExamQuestionId,
                        AnswerText = answer?.AnswerText,
                        IsCorrect = null,
                        MarksAwarded = null,
                    });
                    continue;
                }

                var selectedIds = (answer?.SelectedOptionIds ?? new List<long>()).Distinct().ToHashSet();
                var correctIds = question.Options.Where(o => o.IsCorrect).Select(o => o.ExamQuestionOptionId).ToHashSet();
                var isCorrect = selectedIds.Count > 0 && selectedIds.SetEquals(correctIds);
                var marksAwarded = isCorrect ? question.Marks : 0m;
                totalScore += marksAwarded;

                examAnswers.Add(new ExamAnswer
                {
                    ExamEnrollmentId = enrollment.ExamEnrollmentId,
                    ExamQuestionId = question.ExamQuestionId,
                    SelectedOptionIds = selectedIds.Count > 0 ? string.Join(",", selectedIds) : null,
                    IsCorrect = isCorrect,
                    MarksAwarded = marksAwarded,
                });
            }

            await _examAnswerRepository.AddRangeAsync(examAnswers);

            enrollment.SubmittedAt = DateTime.UtcNow;
            enrollment.Score = totalScore;
            enrollment.IsPassed = totalScore >= enrollment.Exam.PassMarks;
            enrollment.ScoreSource = ScoreSourceEnum.AutoScored;
            enrollment.ScoredAt = DateTime.UtcNow;
            _examEnrollmentRepository.Update(enrollment);

            await _unitOfWork.SaveChangesAsync();

            var resultsVisible = enrollment.Exam.ShowResultsToCandidate;

            return new ExamSubmitResultResponse
            {
                ReferenceNumber = $"EXM-{enrollment.ExamId}-{enrollment.ExamEnrollmentId}",
                SubmittedAt = enrollment.SubmittedAt.Value,
                ResultsVisible = resultsVisible,
                Score = resultsVisible ? enrollment.Score : null,
                IsPassed = resultsVisible ? enrollment.IsPassed : null,
            };
        }

        private async Task<ExamEnrollment> GetOwnedEnrollmentAsync(long examEnrollmentId)
        {
            var enrollment = await _examEnrollmentRepository.GetByIdWithExamAndQuestionsAsync(examEnrollmentId)
                ?? throw new NotFoundException("ExamEnrollment", examEnrollmentId);

            var candidateProfileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            if (enrollment.JobApplication.CandidateProfileId != candidateProfileId)
                throw new ForbiddenException("This exam enrollment does not belong to you.");

            return enrollment;
        }
    }
}
