using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Features.TalentPools.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class TalentPoolService : ITalentPoolService
    {
        private readonly ITalentPoolRepository _talentPoolRepository;
        private readonly ITalentPoolCandidateRepository _talentPoolCandidateRepository;
        private readonly ICandidateProfileRepository _candidateProfileRepository;
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly IJobApplicationService _jobApplicationService;
        private readonly IUnitOfWork _unitOfWork;

        public TalentPoolService(
            ITalentPoolRepository talentPoolRepository,
            ITalentPoolCandidateRepository talentPoolCandidateRepository,
            ICandidateProfileRepository candidateProfileRepository,
            IJobPostingRepository jobPostingRepository,
            IJobApplicationRepository jobApplicationRepository,
            IJobApplicationService jobApplicationService,
            IUnitOfWork unitOfWork)
        {
            _talentPoolRepository = talentPoolRepository;
            _talentPoolCandidateRepository = talentPoolCandidateRepository;
            _candidateProfileRepository = candidateProfileRepository;
            _jobPostingRepository = jobPostingRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _jobApplicationService = jobApplicationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(TalentPoolCreateRequest request)
        {
            var name = request.Name.Trim();

            var exists = await _talentPoolRepository.ExistsByNameAsync(name);
            if (exists)
                throw new DuplicateException("TalentPool", "Name", name);

            var entity = new TalentPool { Name = name };

            await _talentPoolRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.TalentPoolId;
        }

        public async Task DeleteAsync(long talentPoolId)
        {
            var entity = await _talentPoolRepository.GetByIdAsync(talentPoolId)
                ?? throw new NotFoundException("TalentPool", talentPoolId);

            _talentPoolRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<TalentPoolResponse>> GetAllAsync()
        {
            var entities = await _talentPoolRepository.GetAllWithCandidateCountAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<List<TalentPoolLookupResponse>> GetLookupAsync()
        {
            var entities = await _talentPoolRepository.GetAllAsync();
            return entities.Select(e => e.ToLookupResponse()).ToList();
        }

        public async Task<TalentPoolDetailResponse> GetByIdAsync(long talentPoolId)
        {
            var entity = await _talentPoolRepository.GetByIdWithCandidatesAsync(talentPoolId)
                ?? throw new NotFoundException("TalentPool", talentPoolId);

            return entity.ToDetailResponse();
        }

        public async Task<TalentPoolCandidateAddResponse> AddCandidatesAsync(long talentPoolId, TalentPoolCandidateAddRequest request)
        {
            var pool = await _talentPoolRepository.GetByIdAsync(talentPoolId)
                ?? throw new NotFoundException("TalentPool", talentPoolId);

            var candidateIds = request.CandidateProfileIds.Distinct().ToList();
            var existingIds = await _talentPoolCandidateRepository.GetExistingCandidateIdsAsync(talentPoolId, candidateIds);

            var response = new TalentPoolCandidateAddResponse();

            foreach (var candidateProfileId in candidateIds)
            {
                if (existingIds.Contains(candidateProfileId))
                {
                    response.AlreadyInPoolCount++;
                    continue;
                }

                var candidate = await _candidateProfileRepository.GetByIdAsync(candidateProfileId);
                if (candidate == null)
                {
                    response.NotFoundCount++;
                    continue;
                }

                await _talentPoolCandidateRepository.AddAsync(new TalentPoolCandidate
                {
                    TalentPoolId = talentPoolId,
                    CandidateProfileId = candidateProfileId,
                    AddedDate = DateTime.UtcNow
                });

                response.AddedCount++;
            }

            await _unitOfWork.SaveChangesAsync();
            return response;
        }

        public async Task RemoveCandidateAsync(long talentPoolId, long candidateProfileId)
        {
            var entity = await _talentPoolCandidateRepository.GetByPoolAndCandidateAsync(talentPoolId, candidateProfileId)
                ?? throw new NotFoundException("TalentPoolCandidate", candidateProfileId);

            _talentPoolCandidateRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<TalentPoolFastTrackResponse> FastTrackAsync(TalentPoolFastTrackRequest request)
        {
            _ = await _jobPostingRepository.GetByIdAsync(request.JobPostingId)
                ?? throw new NotFoundException("JobPosting", request.JobPostingId);

            var response = new TalentPoolFastTrackResponse();

            foreach (var candidateProfileId in request.CandidateProfileIds.Distinct())
            {
                response.ProcessedCount++;

                var candidate = await _candidateProfileRepository.GetByIdAsync(candidateProfileId);
                if (candidate == null)
                {
                    response.SkippedCount++;
                    continue;
                }

                var alreadyApplied = await _jobApplicationRepository.GetByEmailAndJobPostingIdAsync(candidate.Email, request.JobPostingId);
                if (alreadyApplied != null)
                {
                    response.AlreadyAppliedCount++;
                    continue;
                }

                // JobApplication has no FK to CandidateProfile - reuse the resume from the
                // candidate's most recent prior application (GetByCandidateEmailAsync orders
                // desc by AppliedDate) instead of requiring HR to re-upload a CV.
                var priorApplications = await _jobApplicationRepository.GetByCandidateEmailAsync(candidate.Email);
                var latestApplication = priorApplications.FirstOrDefault();

                if (latestApplication == null)
                {
                    response.SkippedCount++;
                    continue;
                }

                var jobApplicationId = await _jobApplicationService.CreateAsync(new JobApplicationCreateRequest
                {
                    JobPostingId = request.JobPostingId,
                    CandidateName = candidate.FullName,
                    CandidateEmail = candidate.Email,
                    CandidatePhone = candidate.Phone,
                    ResumeUrl = latestApplication.ResumeUrl,
                    Source = ApplicationSourceEnum.Admin
                });

                await _jobApplicationService.UpdateStatusAsync(jobApplicationId, new JobApplicationStatusUpdateRequest
                {
                    ToStatus = ApplicationStatusEnum.Shortlisted
                });

                response.FastTrackedCount++;
            }

            return response;
        }
    }
}
