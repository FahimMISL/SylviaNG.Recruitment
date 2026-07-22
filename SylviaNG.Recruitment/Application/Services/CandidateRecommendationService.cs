using FluentValidation;
using FluentValidation.Results;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.CandidateRecommendations.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Utils;

namespace SylviaNG.Recruitment.Application.Services
{
    public class CandidateRecommendationService : ICandidateRecommendationService
    {
        private readonly ICandidateRecommendationRepository _candidateRecommendationRepository;
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public CandidateRecommendationService(
            ICandidateRecommendationRepository candidateRecommendationRepository,
            IJobApplicationRepository jobApplicationRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _candidateRecommendationRepository = candidateRecommendationRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(long jobApplicationId, CandidateRecommendationCreateRequest request)
        {
            _ = await _jobApplicationRepository.GetByIdAsync(jobApplicationId)
                ?? throw new NotFoundException("JobApplication", jobApplicationId);

            if (string.IsNullOrWhiteSpace(request.Justification))
                throw new ValidationException(new[]
                {
                    new ValidationFailure(nameof(request.Justification), "Justification is required.")
                });

            var existingPending = await _candidateRecommendationRepository.GetLatestByJobApplicationIdAsync(jobApplicationId);
            if (existingPending is { Status: RecommendationStatusEnum.Pending })
                throw new ValidationException(new[]
                {
                    new ValidationFailure(nameof(jobApplicationId), "This application already has a pending recommendation awaiting review.")
                });

            var recommendedByUserName = _currentUserService.GetCurrentUserName();
            if (string.IsNullOrWhiteSpace(recommendedByUserName))
                throw new ValidationException(new[]
                {
                    new ValidationFailure(nameof(recommendedByUserName), "Could not resolve the current user - a recommendation must have a definite author.")
                });

            var entity = new CandidateRecommendation
            {
                JobApplicationId = jobApplicationId,
                Justification = request.Justification,
                RecommendedByUserName = recommendedByUserName,
                RecommendedAt = DateTimeUtility.NowUtc(),
                Status = RecommendationStatusEnum.Pending,
            };

            await _candidateRecommendationRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.CandidateRecommendationId;
        }

        public async Task<CandidateRecommendationResponse?> GetLatestByJobApplicationIdAsync(long jobApplicationId)
        {
            var entity = await _candidateRecommendationRepository.GetLatestByJobApplicationIdAsync(jobApplicationId);
            return entity?.ToResponse();
        }

        public async Task<List<CandidateRecommendationPendingListItemResponse>> GetPendingAsync()
        {
            var entities = await _candidateRecommendationRepository.GetPendingWithApplicationAsync();
            return entities.Select(e => e.ToPendingListItemResponse()).ToList();
        }

        public async Task ReviewAsync(long candidateRecommendationId, CandidateRecommendationReviewRequest request)
        {
            var entity = await _candidateRecommendationRepository.GetByIdAsync(candidateRecommendationId)
                ?? throw new NotFoundException("CandidateRecommendation", candidateRecommendationId);

            if (entity.Status != RecommendationStatusEnum.Pending)
                throw new ValidationException(new[]
                {
                    new ValidationFailure(nameof(entity.Status), "This recommendation has already been reviewed.")
                });

            if (request.Status == RecommendationStatusEnum.Pending)
                throw new ValidationException(new[]
                {
                    new ValidationFailure(nameof(request.Status), "Review outcome must be Accepted or Rejected.")
                });

            var reviewedByUserName = _currentUserService.GetCurrentUserName();
            if (string.IsNullOrWhiteSpace(reviewedByUserName))
                throw new ValidationException(new[]
                {
                    new ValidationFailure(nameof(reviewedByUserName), "Could not resolve the current user - a review must have a definite reviewer.")
                });

            entity.Status = request.Status;
            entity.ReviewComments = request.ReviewComments;
            entity.ReviewedByUserName = reviewedByUserName;
            entity.ReviewedAt = DateTimeUtility.NowUtc();

            _candidateRecommendationRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
