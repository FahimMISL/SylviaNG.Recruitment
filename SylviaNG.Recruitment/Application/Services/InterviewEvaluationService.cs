using ClosedXML.Excel;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class InterviewEvaluationService : IInterviewEvaluationService
    {
        private readonly IInterviewEvaluationRepository _interviewEvaluationRepository;
        private readonly IInterviewRepository _interviewRepository;
        private readonly IScorecardRepository _scorecardRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public InterviewEvaluationService(
            IInterviewEvaluationRepository interviewEvaluationRepository,
            IInterviewRepository interviewRepository,
            IScorecardRepository scorecardRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _interviewEvaluationRepository = interviewEvaluationRepository;
            _interviewRepository = interviewRepository;
            _scorecardRepository = scorecardRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> SubmitAsync(long interviewId, InterviewEvaluationSubmitRequest request)
        {
            var interview = await _interviewRepository.GetByIdWithDetailsAsync(interviewId)
                ?? throw new NotFoundException("Interview", interviewId);

            if (interview.Status == InterviewStatusEnum.Cancelled)
                throw new InvalidStatusTransitionException("Cannot evaluate a cancelled interview.");

            if (!interview.PanelMembers.Any(p => p.EmployeeId == request.EmployeeId))
                throw new InvalidStatusTransitionException($"Employee {request.EmployeeId} is not a panel member for this interview.");

            var alreadyEvaluated = await _interviewEvaluationRepository.ExistsByInterviewAndEmployeeAsync(interviewId, request.EmployeeId);
            if (alreadyEvaluated)
                throw new DuplicateException("InterviewEvaluation", "EmployeeId", request.EmployeeId.ToString());

            var scorecard = await _scorecardRepository.GetByIdWithCriteriaAsync(request.ScorecardId)
                ?? throw new NotFoundException("Scorecard", request.ScorecardId);

            if (!scorecard.IsActive)
                throw new InvalidStatusTransitionException($"Scorecard {request.ScorecardId} is not active.");

            ValidateScores(scorecard, request.Scores);

            var entity = new InterviewEvaluation
            {
                InterviewId = interviewId,
                ScorecardId = request.ScorecardId,
                EmployeeId = request.EmployeeId,
                OverallComments = request.OverallComments,
                SubmittedAt = DateTime.UtcNow,
                SubmittedByUserName = _currentUserService.GetCurrentUserName(),
                Scores = request.Scores
                    .Select(s => new InterviewEvaluationScore { ScorecardCriterionId = s.ScorecardCriterionId, Score = s.Score })
                    .ToList(),
            };

            await _interviewEvaluationRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.InterviewEvaluationId;
        }

        public async Task UpdateAsync(long interviewEvaluationId, InterviewEvaluationUpdateRequest request)
        {
            var entity = await _interviewEvaluationRepository.GetByIdWithDetailsAsync(interviewEvaluationId)
                ?? throw new NotFoundException("InterviewEvaluation", interviewEvaluationId);

            ValidateScores(entity.Scorecard, request.Scores);

            entity.Scores.Clear();
            foreach (var score in request.Scores)
            {
                entity.Scores.Add(new InterviewEvaluationScore { ScorecardCriterionId = score.ScorecardCriterionId, Score = score.Score });
            }
            entity.OverallComments = request.OverallComments;

            _interviewEvaluationRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<InterviewEvaluationResponse> GetByIdAsync(long interviewEvaluationId)
        {
            var entity = await _interviewEvaluationRepository.GetByIdWithDetailsAsync(interviewEvaluationId)
                ?? throw new NotFoundException("InterviewEvaluation", interviewEvaluationId);

            return entity.ToResponse();
        }

        public async Task<List<InterviewEvaluationResponse>> GetByInterviewIdAsync(long interviewId)
        {
            var entities = await _interviewEvaluationRepository.GetByInterviewIdAsync(interviewId);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<InterviewEvaluationResultsFileResponse> ExportResultsExcelAsync(long interviewId)
        {
            var interview = await _interviewRepository.GetByIdWithDetailsAsync(interviewId)
                ?? throw new NotFoundException("Interview", interviewId);

            var evaluations = await _interviewEvaluationRepository.GetByInterviewIdAsync(interviewId);
            var responses = evaluations.Select(e => e.ToResponse()).ToList();

            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Interview Results");

            var headers = new[] { "Candidate", "Panelist Employee ID", "Scorecard", "Criteria Scores", "Weighted Score (%)", "Overall Comments", "Submitted At", "Submitted By" };
            for (var column = 0; column < headers.Length; column++)
                sheet.Cell(1, column + 1).Value = headers[column];
            sheet.Row(1).Style.Font.Bold = true;

            var rowIndex = 2;
            foreach (var evaluation in responses)
            {
                var criteriaScores = string.Join("; ", evaluation.Scores.Select(s => $"{s.CriterionName}: {s.Score}/{s.MaxScore}"));

                sheet.Cell(rowIndex, 1).Value = interview.JobApplication?.CandidateName ?? string.Empty;
                sheet.Cell(rowIndex, 2).Value = evaluation.EmployeeId;
                sheet.Cell(rowIndex, 3).Value = evaluation.ScorecardName;
                sheet.Cell(rowIndex, 4).Value = criteriaScores;
                sheet.Cell(rowIndex, 5).Value = evaluation.WeightedScore;
                sheet.Cell(rowIndex, 6).Value = evaluation.OverallComments ?? string.Empty;
                sheet.Cell(rowIndex, 7).Value = evaluation.SubmittedAt.ToString("yyyy-MM-dd HH:mm");
                sheet.Cell(rowIndex, 8).Value = evaluation.SubmittedByUserName ?? string.Empty;
                rowIndex++;
            }

            sheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return new InterviewEvaluationResultsFileResponse
            {
                Content = stream.ToArray(),
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                FileName = $"Interview-{interviewId}-Results-{DateTime.UtcNow:yyyyMMdd-HHmmss}.xlsx",
            };
        }

        private static void ValidateScores(Scorecard scorecard, List<InterviewEvaluationScoreRequest> scores)
        {
            var criteriaById = scorecard.Criteria.ToDictionary(c => c.ScorecardCriterionId);

            if (scores.Count != criteriaById.Count || scores.Select(s => s.ScorecardCriterionId).Distinct().Count() != criteriaById.Count)
                throw new InvalidStatusTransitionException("A score is required for every criterion in this scorecard, exactly once.");

            foreach (var score in scores)
            {
                var criterion = criteriaById.GetValueOrDefault(score.ScorecardCriterionId)
                    ?? throw new NotFoundException("ScorecardCriterion", score.ScorecardCriterionId);

                if (score.Score < 0 || score.Score > criterion.MaxScore)
                    throw new InvalidStatusTransitionException(
                        $"Score for criterion \"{criterion.Name}\" must be between 0 and {criterion.MaxScore}.");
            }
        }
    }
}
