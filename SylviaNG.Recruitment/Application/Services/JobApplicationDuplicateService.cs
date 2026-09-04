using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Common.Utilities;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class JobApplicationDuplicateService : IJobApplicationDuplicateService
    {
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJobApplicationStatusService _statusService;

        public JobApplicationDuplicateService(
            IJobApplicationRepository jobApplicationRepository,
            IUnitOfWork unitOfWork,
            IJobApplicationStatusService statusService)
        {
            _jobApplicationRepository = jobApplicationRepository;
            _unitOfWork = unitOfWork;
            _statusService = statusService;
        }

        public async Task<List<JobApplicationDuplicateGroupResponse>> GetDuplicatesAsync(long jobPostingId)
        {
            var applications = await _jobApplicationRepository.GetAllByJobPostingIdAsync(jobPostingId);
            return BuildDuplicateGroups(applications);
        }

        public async Task ResolveDuplicatesAsync(JobApplicationDuplicateResolveRequest request)
        {
            if (request.DuplicateJobApplicationIds.Count == 0)
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(request.DuplicateJobApplicationIds),
                        "At least one duplicate application id is required.")
                });
            }

            if (request.DuplicateJobApplicationIds.Contains(request.PrimaryJobApplicationId))
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(request.PrimaryJobApplicationId),
                        "PrimaryJobApplicationId must not also appear in DuplicateJobApplicationIds.")
                });
            }

            // Re-derive the duplicate group server-side rather than trusting the client's grouping,
            // so HR can't dismiss an application that isn't actually part of a detected duplicate set.
            var groups = await GetDuplicatesAsync(request.JobPostingId);
            var group = groups.FirstOrDefault(g => g.Applications.Any(a => a.JobApplicationId == request.PrimaryJobApplicationId));

            if (group == null || request.DuplicateJobApplicationIds.Any(id => !group.Applications.Any(a => a.JobApplicationId == id)))
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(request.DuplicateJobApplicationIds),
                        "The supplied applications are not a detected duplicate group for this vacancy.")
                });
            }

            foreach (var duplicateId in request.DuplicateJobApplicationIds)
            {
                var entity = await _jobApplicationRepository.GetByIdAsync(duplicateId)
                    ?? throw new NotFoundException("JobApplication", duplicateId);

                await _statusService.ApplyStatusChangeAsync(entity, new JobApplicationStatusUpdateRequest
                {
                    ToStatus = ApplicationStatusEnum.DuplicateDismissed,
                    Note = $"Duplicate of application #{request.PrimaryJobApplicationId}, dismissed by HR."
                });
            }

            await _unitOfWork.SaveChangesAsync();
        }

        private static List<JobApplicationDuplicateGroupResponse> BuildDuplicateGroups(List<JobApplication> applications)
        {
            var candidates = applications
                .Where(a => a.ApplicationStatus != ApplicationStatusEnum.DuplicateDismissed)
                .ToList();

            var parent = Enumerable.Range(0, candidates.Count).ToArray();

            int Find(int i) => parent[i] == i ? i : (parent[i] = Find(parent[i]));
            void Union(int a, int b)
            {
                var rootA = Find(a);
                var rootB = Find(b);
                if (rootA != rootB) parent[rootA] = rootB;
            }

            for (var i = 0; i < candidates.Count; i++)
            {
                for (var j = i + 1; j < candidates.Count; j++)
                {
                    if (AreDuplicates(candidates[i], candidates[j]))
                        Union(i, j);
                }
            }

            return candidates
                .Select((app, index) => (app, root: Find(index)))
                .GroupBy(x => x.root)
                .Where(g => g.Count() > 1)
                .Select(g =>
                {
                    var members = g.Select(x => x.app).ToList();
                    return new JobApplicationDuplicateGroupResponse
                    {
                        Applications = members.OrderBy(m => m.AppliedDate).Select(m => m.ToDuplicateItemResponse()).ToList(),
                        MatchedOn = DetermineMatchedOn(members)
                    };
                })
                .ToList();
        }

        private static bool AreDuplicates(JobApplication a, JobApplication b)
        {
            if (!string.IsNullOrEmpty(a.CandidateEmail) && !string.IsNullOrEmpty(b.CandidateEmail)
                && string.Equals(a.CandidateEmail, b.CandidateEmail, StringComparison.OrdinalIgnoreCase))
                return true;

            var phoneA = PhoneNormalizer.Normalize(a.CandidatePhone);
            var phoneB = PhoneNormalizer.Normalize(b.CandidatePhone);
            if (phoneA != null && phoneB != null && phoneA == phoneB)
                return true;

            if (!string.IsNullOrEmpty(a.CandidateNationalId) && !string.IsNullOrEmpty(b.CandidateNationalId)
                && string.Equals(a.CandidateNationalId, b.CandidateNationalId, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        private static List<string> DetermineMatchedOn(List<JobApplication> members)
        {
            var pairs = members
                .SelectMany((a, i) => members.Skip(i + 1).Select(b => (a, b)))
                .ToList();

            var reasons = new List<string>();

            if (pairs.Any(p => !string.IsNullOrEmpty(p.a.CandidateEmail) && !string.IsNullOrEmpty(p.b.CandidateEmail)
                && string.Equals(p.a.CandidateEmail, p.b.CandidateEmail, StringComparison.OrdinalIgnoreCase)))
                reasons.Add("Email");

            if (pairs.Any(p =>
            {
                var phoneA = PhoneNormalizer.Normalize(p.a.CandidatePhone);
                var phoneB = PhoneNormalizer.Normalize(p.b.CandidatePhone);
                return phoneA != null && phoneB != null && phoneA == phoneB;
            }))
                reasons.Add("Phone");

            if (pairs.Any(p => !string.IsNullOrEmpty(p.a.CandidateNationalId) && !string.IsNullOrEmpty(p.b.CandidateNationalId)
                && string.Equals(p.a.CandidateNationalId, p.b.CandidateNationalId, StringComparison.OrdinalIgnoreCase)))
                reasons.Add("NationalId");

            return reasons;
        }
    }
}
