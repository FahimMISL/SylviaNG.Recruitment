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
        private readonly IJobApplicationStageProgressService _jobApplicationStageProgressService;
        private readonly ICurrentCandidateService _currentCandidateService;
        private readonly IUnitOfWork _unitOfWork;

        public ExamTakingService(
            IExamEnrollmentRepository examEnrollmentRepository,
            IExamQuestionRepository examQuestionRepository,
            IExamAnswerRepository examAnswerRepository,
            IJobApplicationStageProgressService jobApplicationStageProgressService,
            ICurrentCandidateService currentCandidateService,
            IUnitOfWork unitOfWork)
        {
            _examEnrollmentRepository = examEnrollmentRepository;
            _examQuestionRepository = examQuestionRepository;
            _examAnswerRepository = examAnswerRepository;
            _jobApplicationStageProgressService = jobApplicationStageProgressService;
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

            var questions = await _examQuestionRepository.GetActiveByQuestionGroupIdsAsync(enrollment.Exam.QuestionGroupLinks.Select(l => l.QuestionGroupId).ToList());
            var normalizedMarks = NormalizeQuestionMarks(questions, enrollment.Exam.TotalMarks);

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
                    Marks = normalizedMarks[q.ExamQuestionId],
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

        /// <summary>A QuestionGroup's questions carry their own per-question Marks (set
        /// independently when authored in the question bank), which rarely sum to the exam's
        /// configured TotalMarks - HR picks a group without needing its question count or
        /// per-question weights to add up to anything in particular. Scales every question's
        /// Marks proportionally onto the 0-TotalMarks scale so what the candidate sees per
        /// question (and what they're actually scored on) always sums to exactly TotalMarks,
        /// regardless of how many questions the group has or how they're individually weighted.
        /// The last question (stable ExamQuestionId order, same as the repository's own
        /// ordering) absorbs whatever's left after rounding the rest to 2dp, so the total is
        /// always exact rather than off by a few cents.</summary>
        private static Dictionary<long, decimal> NormalizeQuestionMarks(List<ExamQuestion> questions, decimal targetTotalMarks)
        {
            var totalRawMarks = questions.Sum(q => q.Marks);
            if (totalRawMarks <= 0)
                return questions.ToDictionary(q => q.ExamQuestionId, _ => 0m);

            var result = new Dictionary<long, decimal>();
            decimal runningSum = 0;
            for (var i = 0; i < questions.Count; i++)
            {
                var isLast = i == questions.Count - 1;
                var marks = isLast
                    ? targetTotalMarks - runningSum
                    : Math.Round(questions[i].Marks / totalRawMarks * targetTotalMarks, 2);
                result[questions[i].ExamQuestionId] = marks;
                runningSum += marks;
            }
            return result;
        }

        public async Task<ExamSubmitResultResponse> SubmitExamAsync(long examEnrollmentId, ExamSubmitRequest request)
        {
            var enrollment = await GetOwnedEnrollmentAsync(examEnrollmentId);

            if (enrollment.StartedAt == null)
                throw new InvalidStatusTransitionException("This exam has not been started.");

            if (enrollment.SubmittedAt != null)
                throw new InvalidStatusTransitionException("This exam has already been submitted.");

            var questions = await _examQuestionRepository.GetActiveByQuestionGroupIdsAsync(enrollment.Exam.QuestionGroupLinks.Select(l => l.QuestionGroupId).ToList());
            var normalizedMarks = NormalizeQuestionMarks(questions, enrollment.Exam.TotalMarks);
            var answersByQuestionId = request.Answers
                .GroupBy(a => a.ExamQuestionId)
                .ToDictionary(g => g.Key, g => g.First());

            var examAnswers = new List<ExamAnswer>();
            decimal score = 0;

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
                // Scored on the same normalized marks the candidate was shown per question on the
                // paper (StartExamAsync) - not the question bank's raw Marks - so what HR sees per
                // answer afterward matches what the candidate saw while taking it, and the sum
                // always lands on the exam's configured TotalMarks regardless of how many
                // questions the group has or how they're individually weighted.
                var marksAwarded = isCorrect ? normalizedMarks[question.ExamQuestionId] : 0m;
                score += marksAwarded;

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
            enrollment.Score = score;
            enrollment.IsPassed = score >= enrollment.Exam.PassMarks;
            enrollment.ScoreSource = ScoreSourceEnum.AutoScored;
            enrollment.ScoredAt = DateTime.UtcNow;
            _examEnrollmentRepository.Update(enrollment);

            await _unitOfWork.SaveChangesAsync();

            // score only counts auto-gradable questions - if any Subjective question exists, it
            // contributed 0 here and is still awaiting HR grading (via UploadScoreAsync/
            // ExamScoreImportService, which overwrite Score/IsPassed with the true final value).
            // Only safe to drive the pipeline stage from this score when there's nothing left ungraded.
            var hasUngradedSubjective = questions.Any(q => q.QuestionType == QuestionTypeEnum.Subjective);
            if (!hasUngradedSubjective)
                await _jobApplicationStageProgressService.AutoCompleteStageByTypeAsync(
                    enrollment.JobApplicationId, "TechnicalAssessment", score, "system:exam-score");

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
