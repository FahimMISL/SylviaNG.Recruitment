using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.InterviewRoundConfigs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class InterviewRoundConfigService : IInterviewRoundConfigService
    {
        private readonly IInterviewRoundConfigRepository _interviewRoundConfigRepository;
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly IInterviewRepository _interviewRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public InterviewRoundConfigService(
            IInterviewRoundConfigRepository interviewRoundConfigRepository,
            IJobPostingRepository jobPostingRepository,
            IInterviewRepository interviewRepository,
            IEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork)
        {
            _interviewRoundConfigRepository = interviewRoundConfigRepository;
            _jobPostingRepository = jobPostingRepository;
            _interviewRepository = interviewRepository;
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<InterviewRoundConfigResponse>> GetAllByJobPostingIdAsync(long jobPostingId)
        {
            var entities = await _interviewRoundConfigRepository.GetAllByJobPostingIdAsync(jobPostingId);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task ReplaceForJobPostingAsync(long jobPostingId, InterviewRoundConfigReplaceRequest request)
        {
            _ = await _jobPostingRepository.GetByIdAsync(jobPostingId)
                ?? throw new NotFoundException("JobPosting", jobPostingId);

            await ValidatePanelistsAsync(request.Rounds);

            var existing = await _interviewRoundConfigRepository.GetAllByJobPostingIdAsync(jobPostingId);
            var submittedIds = request.Rounds.Where(r => r.InterviewRoundConfigId.HasValue).Select(r => r.InterviewRoundConfigId!.Value).ToHashSet();

            // Rounds dropped from the submitted list are removed, unless an Interview already
            // references them - keeps AC4's "before the interview process begins" honest.
            var toRemove = existing.Where(e => !submittedIds.Contains(e.InterviewRoundConfigId)).ToList();
            foreach (var round in toRemove)
            {
                var inUse = await _interviewRepository.ExistsForRoundConfigAsync(round.InterviewRoundConfigId);
                if (inUse)
                    throw new ResourceInUseException("InterviewRoundConfig", round.InterviewRoundConfigId, 1);

                _interviewRoundConfigRepository.Delete(round);
            }

            var ordered = request.Rounds.OrderBy(r => r.Sequence).ToList();
            var sequence = 1;
            foreach (var roundRequest in ordered)
            {
                roundRequest.Sequence = sequence++;

                if (roundRequest.InterviewRoundConfigId.HasValue)
                {
                    var entity = existing.First(e => e.InterviewRoundConfigId == roundRequest.InterviewRoundConfigId.Value);
                    entity.Name = roundRequest.Name;
                    entity.Sequence = roundRequest.Sequence;
                    entity.ScorecardId = roundRequest.ScorecardId;
                    entity.PanelMembers.Clear();
                    foreach (var employeeId in roundRequest.PanelistEmployeeIds.Distinct())
                        entity.PanelMembers.Add(new InterviewRoundConfigPanelMember { EmployeeId = employeeId });

                    _interviewRoundConfigRepository.Update(entity);
                }
                else
                {
                    var entity = roundRequest.ToEntity();
                    entity.JobPostingId = jobPostingId;
                    await _interviewRoundConfigRepository.AddAsync(entity);
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task ValidatePanelistsAsync(IEnumerable<InterviewRoundConfigRequest> rounds)
        {
            var requestedIds = rounds.SelectMany(r => r.PanelistEmployeeIds).Distinct().ToList();
            foreach (var employeeId in requestedIds)
            {
                _ = await _employeeRepository.GetByIdAsync(employeeId)
                    ?? throw new NotFoundException("Employee", employeeId);
            }
        }
    }
}
