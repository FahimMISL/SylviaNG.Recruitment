using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.Infrastructure.Services;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Controllers;

[Authorize(Roles = "Admin,HR")]
[ApiController]
[Route("recruitment/pipeline")]
public class PipelineController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJobApplicationService _jobApplicationService;
    private readonly IEmailService _emailService;

    public PipelineController(IUnitOfWork unitOfWork, IJobApplicationService jobApplicationService, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _jobApplicationService = jobApplicationService;
        _emailService = emailService;
    }

    [HttpGet("status/{jobApplicationId}")]
    public async Task<ActionResult> GetPipelineStatus(long jobApplicationId)
    {
        var app = await _unitOfWork.Context.Set<JobApplication>()
            .Include(a => a.JobPosting)
            .FirstOrDefaultAsync(a => a.JobApplicationId == jobApplicationId);
        if (app == null) return NotFound();

        var candidate = app.CandidateId.HasValue
            ? await _unitOfWork.Context.Set<Candidate>().FirstOrDefaultAsync(c => c.CandidateId == app.CandidateId.Value)
            : null;

        var configuredStages = await _unitOfWork.Context.Set<HiringPipelineStage>()
            .Where(s => s.JobPostingId == app.JobPostingId && s.IsActive)
            .OrderBy(s => s.StageOrder)
            .ToListAsync();

        var stageProgress = await _unitOfWork.Context.Set<CandidateStageProgress>()
            .Where(p => p.JobApplicationId == jobApplicationId && p.IsActive)
            .ToListAsync();

        var pipelineStages = configuredStages.Select(s =>
        {
            var progress = stageProgress.FirstOrDefault(p => p.HiringPipelineStageId == s.HiringPipelineStageId);
            return new
            {
                s.HiringPipelineStageId,
                s.StageName,
                s.StageType,
                s.StageOrder,
                s.IsMandatory,
                status = progress?.Status ?? "Pending",
                notes = progress?.Notes,
                meetingLink = progress?.MeetingLink,
                scheduledDate = progress?.ScheduledDate,
                completedAt = progress?.CompletedAt,
                candidateStageProgressId = progress?.CandidateStageProgressId
            };
        });

        return Ok(new
        {
            jobApplicationId,
            candidateId = app.CandidateId,
            candidateName = candidate?.FullName,
            candidateEmail = candidate?.Email,
            jobTitle = app.JobPosting?.Title,
            applicationStatus = app.ApplicationStatus.ToString(),
            pipelineStages
        });
    }

    [HttpPost("stage/start")]
    public async Task<ActionResult> StartStage([FromBody] StartStageRequest request)
    {
        var stage = await _unitOfWork.Context.Set<HiringPipelineStage>()
            .FirstOrDefaultAsync(s => s.HiringPipelineStageId == request.HiringPipelineStageId && s.IsActive);
        if (stage == null) return NotFound(new { message = "Pipeline stage not found." });

        var app = await _unitOfWork.Context.Set<JobApplication>()
            .FirstOrDefaultAsync(a => a.JobApplicationId == request.JobApplicationId && a.IsActive);
        if (app == null) return NotFound(new { message = "Job application not found." });

        var existing = await _unitOfWork.Context.Set<CandidateStageProgress>()
            .FirstOrDefaultAsync(p => p.JobApplicationId == request.JobApplicationId
                                      && p.HiringPipelineStageId == request.HiringPipelineStageId && p.IsActive);
        if (existing != null)
            return BadRequest(new { message = "This stage has already been started." });

        var progress = new CandidateStageProgress
        {
            JobApplicationId = request.JobApplicationId,
            HiringPipelineStageId = request.HiringPipelineStageId,
            Status = "InProgress",
            ScheduledDate = request.ScheduledDate,
            MeetingLink = request.MeetingLink,
            Notes = request.Notes,
            IsActive = true
        };
        await _unitOfWork.Context.Set<CandidateStageProgress>().AddAsync(progress);
        await _unitOfWork.SaveChangesAsync();

        if (stage.StageType == "Interview" && request.ScheduledDate.HasValue)
            await NotifyInterviewScheduledAsync(app, request.ScheduledDate.Value,
                string.IsNullOrEmpty(request.MeetingLink) ? "Office" : "Virtual", request.MeetingLink);
        else
            await NotifyCandidateAsync(app, $"{stage.StageName} Started",
                $"The \"{stage.StageName}\" stage of your application process has begun.");

        return Ok(new { message = $"{stage.StageName} started.", candidateStageProgressId = progress.CandidateStageProgressId });
    }

    [HttpPost("stage/complete")]
    public async Task<ActionResult> CompleteStage([FromBody] CompleteStageRequest request)
    {
        var progress = await _unitOfWork.Context.Set<CandidateStageProgress>()
            .Include(p => p.HiringPipelineStage)
            .FirstOrDefaultAsync(p => p.CandidateStageProgressId == request.CandidateStageProgressId && p.IsActive);
        if (progress == null) return NotFound(new { message = "Stage progress not found." });

        var app = await _unitOfWork.Context.Set<JobApplication>()
            .FirstOrDefaultAsync(a => a.JobApplicationId == progress.JobApplicationId && a.IsActive);
        if (app == null) return NotFound(new { message = "Job application not found." });

        progress.Status = request.Passed ? "Completed" : "Failed";
        progress.Notes = request.Notes ?? progress.Notes;
        progress.CompletedAt = DateTime.UtcNow;
        _unitOfWork.Context.Set<CandidateStageProgress>().Update(progress);
        await _unitOfWork.SaveChangesAsync();

        if (!request.Passed)
        {
            await _jobApplicationService.UpdateAsync(progress.JobApplicationId,
                new JobApplicationUpdateRequest { ApplicationStatus = ApplicationStatusEnum.Rejected });
            await NotifyCandidateAsync(app, "Application Update",
                $"Thank you for your time in the \"{progress.HiringPipelineStage.StageName}\" stage. Unfortunately, we will not be proceeding further.");
        }
        else
        {
            var allStages = await _unitOfWork.Context.Set<HiringPipelineStage>()
                .Where(s => s.JobPostingId == app.JobPostingId && s.IsActive)
                .OrderBy(s => s.StageOrder)
                .ToListAsync();
            var allProgress = await _unitOfWork.Context.Set<CandidateStageProgress>()
                .Where(p => p.JobApplicationId == progress.JobApplicationId && p.IsActive)
                .ToListAsync();

            bool allCompleted = allStages.All(s =>
                allProgress.Any(p => p.HiringPipelineStageId == s.HiringPipelineStageId && p.Status == "Completed"));

            if (allCompleted)
            {
                await _jobApplicationService.UpdateAsync(progress.JobApplicationId,
                    new JobApplicationUpdateRequest { ApplicationStatus = ApplicationStatusEnum.Offered });
                await NotifyCandidateAsync(app, "Offer Extended",
                    "Congratulations! You have successfully completed all stages. We are pleased to extend an offer to you.");
            }
            else
            {
                await NotifyCandidateAsync(app, $"{progress.HiringPipelineStage.StageName} Completed",
                    $"You have successfully completed the \"{progress.HiringPipelineStage.StageName}\" stage. We will proceed with the next steps.");
            }
        }

        return Ok(new { message = $"Stage marked as {progress.Status}." });
    }

    [HttpPost("schedule-interview")]
    public async Task<ActionResult> ScheduleInterview([FromBody] ScheduleInterviewRequest request)
    {
        var app = await _unitOfWork.Context.Set<JobApplication>()
            .FirstOrDefaultAsync(a => a.JobApplicationId == request.JobApplicationId && a.IsActive);
        if (app == null) return NotFound(new { message = "Job application not found." });

        var interview = new Interview
        {
            JobApplicationId = request.JobApplicationId,
            InterviewSessionId = request.InterviewSessionId,
            ScheduledDate = request.ScheduledDate,
            Location = request.Location,
            MeetingLink = request.MeetingLink,
            Round = request.Round ?? "Round 1",
            Result = InterviewResultEnum.Pending.ToString(),
            IsActive = true
        };

        await _unitOfWork.Context.Set<Interview>().AddAsync(interview);
        await _unitOfWork.SaveChangesAsync();

        await _jobApplicationService.UpdateAsync(request.JobApplicationId,
            new JobApplicationUpdateRequest { ApplicationStatus = ApplicationStatusEnum.InterviewScheduled });

        await NotifyInterviewScheduledAsync(app, request.ScheduledDate, request.Location ?? "Office", request.MeetingLink);

        return Ok(new { message = "Interview scheduled. Candidate notified.", interviewId = interview.InterviewId });
    }

    [HttpPost("complete-interview/{interviewId}")]
    public async Task<ActionResult> CompleteInterview(long interviewId, [FromBody] CompleteInterviewRequest request)
    {
        var interview = await _unitOfWork.Context.Set<Interview>()
            .FirstOrDefaultAsync(i => i.InterviewId == interviewId);
        if (interview == null) return NotFound(new { message = "Interview not found." });

        interview.Result = request.Result;
        interview.Feedback = request.Feedback;
        _unitOfWork.Context.Set<Interview>().Update(interview);
        await _unitOfWork.SaveChangesAsync();

        var app = await _unitOfWork.Context.Set<JobApplication>()
            .FirstOrDefaultAsync(a => a.JobApplicationId == interview.JobApplicationId);

        if (request.Result == InterviewResultEnum.Passed.ToString())
        {
            await _jobApplicationService.UpdateAsync(interview.JobApplicationId,
                new JobApplicationUpdateRequest { ApplicationStatus = ApplicationStatusEnum.Interviewed });
            if (app != null)
                await NotifyCandidateAsync(app, "Interview Result — Passed",
                    "Congratulations! You have passed the interview. We will proceed with the next steps.");
        }
        else if (request.Result == InterviewResultEnum.Failed.ToString())
        {
            await _jobApplicationService.UpdateAsync(interview.JobApplicationId,
                new JobApplicationUpdateRequest { ApplicationStatus = ApplicationStatusEnum.Rejected });
            if (app != null)
                await NotifyCandidateAsync(app, "Application Update",
                    "Thank you for your time. Unfortunately, we will not be proceeding with your application at this time.");
        }

        return Ok(new { message = $"Interview marked as {request.Result}. Candidate notified." });
    }

    [HttpPost("start-verification")]
    public async Task<ActionResult> StartVerification([FromBody] StartVerificationRequest request)
    {
        var app = await _unitOfWork.Context.Set<JobApplication>()
            .FirstOrDefaultAsync(a => a.JobApplicationId == request.JobApplicationId && a.IsActive);
        if (app == null) return NotFound(new { message = "Job application not found." });

        var adminUser = await _unitOfWork.Context.Set<User>().FirstOrDefaultAsync(u => u.IsActive);

        var workflow = new VerificationWorkflow
        {
            CandidateId = app.CandidateId ?? 0,
            JobApplicationId = request.JobApplicationId,
            OverallStatus = VerificationStatusEnum.InProgress,
            InitiatedByUserId = adminUser?.UserId ?? 1,
            InitiatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _unitOfWork.Context.Set<VerificationWorkflow>().AddAsync(workflow);
        await _unitOfWork.SaveChangesAsync();

        await NotifyCandidateAsync(app, "Verification In Progress",
            "Your background verification process has been initiated.");

        return Ok(new { message = "Verification started. Candidate notified.", verificationWorkflowId = workflow.VerificationWorkflowId });
    }

    [HttpPost("complete-verification/{verificationWorkflowId}")]
    public async Task<ActionResult> CompleteVerification(long verificationWorkflowId)
    {
        var workflow = await _unitOfWork.Context.Set<VerificationWorkflow>()
            .FirstOrDefaultAsync(v => v.VerificationWorkflowId == verificationWorkflowId);
        if (workflow == null) return NotFound(new { message = "Verification workflow not found." });

        workflow.OverallStatus = VerificationStatusEnum.Verified;
        workflow.CompletedAt = DateTime.UtcNow;
        _unitOfWork.Context.Set<VerificationWorkflow>().Update(workflow);
        await _unitOfWork.SaveChangesAsync();

        await _jobApplicationService.UpdateAsync(workflow.JobApplicationId,
            new JobApplicationUpdateRequest { ApplicationStatus = ApplicationStatusEnum.Offered });

        var verApp = await _unitOfWork.Context.Set<JobApplication>()
            .FirstOrDefaultAsync(a => a.JobApplicationId == workflow.JobApplicationId);
        if (verApp != null)
            await NotifyCandidateAsync(verApp, "Offer Extended",
                "Your verification is complete and we are pleased to extend an offer to you. Please check your dashboard for details.");

        return Ok(new { message = "Verification completed. Offer extended. Candidate notified." });
    }

    [HttpPost("create-preboarding")]
    public async Task<ActionResult> CreatePreBoarding([FromBody] CreatePreBoardingRequest request)
    {
        var app = await _unitOfWork.Context.Set<JobApplication>()
            .FirstOrDefaultAsync(a => a.JobApplicationId == request.JobApplicationId && a.IsActive);
        if (app == null) return NotFound(new { message = "Job application not found." });

        var profile = new PreBoardingProfile
        {
            CandidateId = app.CandidateId ?? 0,
            JobApplicationId = request.JobApplicationId,
            ProfileStatus = PreBoardingStatusEnum.Pending,
            IsLocked = false,
            IsActive = true
        };

        await _unitOfWork.Context.Set<PreBoardingProfile>().AddAsync(profile);
        await _unitOfWork.SaveChangesAsync();

        await NotifyCandidateAsync(app, "Pre-Boarding Profile Created",
            "Your pre-boarding profile has been created. Please complete the required information.");

        return Ok(new { message = "Pre-boarding profile created. Candidate notified.", preBoardingProfileId = profile.PreBoardingProfileId });
    }

    [HttpPost("create-fitment")]
    public async Task<ActionResult> CreateFitment([FromBody] CreateFitmentRequest request)
    {
        var app = await _unitOfWork.Context.Set<JobApplication>()
            .FirstOrDefaultAsync(a => a.JobApplicationId == request.JobApplicationId && a.IsActive);
        if (app == null) return NotFound(new { message = "Job application not found." });

        var fitment = new FitmentData
        {
            CandidateId = app.CandidateId ?? 0,
            JobApplicationId = request.JobApplicationId,
            ProposedRole = request.ProposedRole,
            ProposedGrade = request.ProposedGrade,
            Location = request.Location,
            SalaryStructureJson = request.SalaryStructureJson,
            IsFetchedFromPayroll = false,
            IsManualEntry = true,
            FetchedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _unitOfWork.Context.Set<FitmentData>().AddAsync(fitment);
        await _unitOfWork.SaveChangesAsync();

        await NotifyCandidateAsync(app, "Fitment Details Prepared",
            "Your salary fitment and role details have been prepared.");

        return Ok(new { message = "Fitment data created. Candidate notified.", fitmentDataId = fitment.FitmentDataId });
    }

    [HttpPost("start-onboarding")]
    public async Task<ActionResult> StartOnboarding([FromBody] StartOnboardingRequest request)
    {
        var app = await _unitOfWork.Context.Set<JobApplication>()
            .FirstOrDefaultAsync(a => a.JobApplicationId == request.JobApplicationId && a.IsActive);
        if (app == null) return NotFound(new { message = "Job application not found." });

        var record = new OnboardingRecord
        {
            CandidateId = app.CandidateId ?? 0,
            JobApplicationId = request.JobApplicationId,
            Stage = OnboardingStageEnum.PreHire,
            IsPreHireSuccess = false,
            IsPostHireSuccess = false,
            RetryCount = 0,
            IsActive = true
        };

        await _unitOfWork.Context.Set<OnboardingRecord>().AddAsync(record);
        await _unitOfWork.SaveChangesAsync();

        await _jobApplicationService.UpdateAsync(request.JobApplicationId,
            new JobApplicationUpdateRequest { ApplicationStatus = ApplicationStatusEnum.Hired });

        await NotifyCandidateAsync(app, "Welcome Aboard!",
            "Congratulations! You have been officially hired. Your onboarding process has begun. Please check your dashboard for next steps.");

        return Ok(new { message = "Onboarding started. Candidate notified as Hired!", onboardingRecordId = record.OnboardingRecordId });
    }

    [HttpGet("analytics")]
    public async Task<ActionResult> GetAnalytics()
    {
        var totalCandidates = await _unitOfWork.Context.Set<Candidate>().CountAsync(c => c.IsActive);
        var totalApplications = await _unitOfWork.Context.Set<JobApplication>().CountAsync(a => a.IsActive);
        var totalJobPostings = await _unitOfWork.Context.Set<JobPosting>().CountAsync(j => j.IsActive);
        var totalInterviews = await _unitOfWork.Context.Set<Interview>().CountAsync(i => i.IsActive);
        var totalReferrals = await _unitOfWork.Context.Set<Referral>().CountAsync(r => r.IsActive);
        var totalOnboarded = await _unitOfWork.Context.Set<OnboardingRecord>().CountAsync(o => o.IsActive);

        var statusBreakdown = await _unitOfWork.Context.Set<JobApplication>()
            .Where(a => a.IsActive)
            .GroupBy(a => a.ApplicationStatus)
            .Select(g => new { status = g.Key.ToString(), count = g.Count() })
            .ToListAsync();

        var recentApplications = await _unitOfWork.Context.Set<JobApplication>()
            .Where(a => a.IsActive)
            .OrderByDescending(a => a.CreatedAt)
            .Take(10)
            .Join(_unitOfWork.Context.Set<Candidate>().Where(c => c.IsActive),
                a => a.CandidateId, c => c.CandidateId,
                (a, c) => new { a, c })
            .GroupJoin(_unitOfWork.Context.Set<JobPosting>(),
                ac => ac.a.JobPostingId, j => j.JobPostingId,
                (ac, jobs) => new { ac.a, ac.c, job = jobs.FirstOrDefault() })
            .Select(x => new
            {
                x.a.JobApplicationId,
                candidateName = x.c.FullName ?? "-",
                jobTitle = x.job != null ? x.job.Title : "-",
                status = x.a.ApplicationStatus.ToString(),
                appliedDate = x.a.CreatedAt
            })
            .ToListAsync();

        var verificationStats = new
        {
            total = await _unitOfWork.Context.Set<VerificationWorkflow>().CountAsync(v => v.IsActive),
            verified = await _unitOfWork.Context.Set<VerificationWorkflow>().CountAsync(v => v.IsActive && v.OverallStatus == VerificationStatusEnum.Verified),
            pending = await _unitOfWork.Context.Set<VerificationWorkflow>().CountAsync(v => v.IsActive && v.OverallStatus == VerificationStatusEnum.Pending),
            inProgress = await _unitOfWork.Context.Set<VerificationWorkflow>().CountAsync(v => v.IsActive && v.OverallStatus == VerificationStatusEnum.InProgress),
        };

        return Ok(new
        {
            summary = new { totalCandidates, totalApplications, totalJobPostings, totalInterviews, totalReferrals, totalOnboarded },
            statusBreakdown,
            recentApplications,
            verificationStats,
        });
    }

    [HttpGet("verifications")]
    public async Task<ActionResult> ListVerifications()
    {
        var items = await _unitOfWork.Context.Set<VerificationWorkflow>()
            .Where(v => v.IsActive)
            .Join(_unitOfWork.Context.Set<Candidate>().Where(c => c.IsActive),
                v => v.CandidateId, c => c.CandidateId,
                (v, c) => new { v, c })
            .Join(_unitOfWork.Context.Set<JobApplication>().Where(a => a.IsActive),
                vc => vc.v.JobApplicationId, a => a.JobApplicationId,
                (vc, a) => new { vc.v, vc.c, a })
            .GroupJoin(_unitOfWork.Context.Set<JobPosting>(),
                vca => vca.a.JobPostingId, j => j.JobPostingId,
                (vca, jobs) => new { vca.v, vca.c, vca.a, job = jobs.FirstOrDefault() })
            .Select(x => new
            {
                x.v.VerificationWorkflowId,
                candidateName = x.c.FullName ?? "-",
                jobTitle = x.job != null ? x.job.Title : "-",
                overallStatus = x.v.OverallStatus.ToString(),
                x.v.InitiatedAt,
                x.v.CompletedAt,
                applicationStatus = x.a.ApplicationStatus.ToString(),
                x.v.JobApplicationId
            })
            .OrderByDescending(x => x.VerificationWorkflowId)
            .ToListAsync();

        return Ok(items);
    }

    [HttpGet("pre-boardings")]
    public async Task<ActionResult> ListPreBoardings()
    {
        var items = await _unitOfWork.Context.Set<PreBoardingProfile>()
            .Where(p => p.IsActive)
            .Join(_unitOfWork.Context.Set<Candidate>().Where(c => c.IsActive),
                p => p.CandidateId, c => c.CandidateId,
                (p, c) => new { p, c })
            .Join(_unitOfWork.Context.Set<JobApplication>().Where(a => a.IsActive),
                pc => pc.p.JobApplicationId, a => a.JobApplicationId,
                (pc, a) => new { pc.p, pc.c, a })
            .GroupJoin(_unitOfWork.Context.Set<JobPosting>(),
                pca => pca.a.JobPostingId, j => j.JobPostingId,
                (pca, jobs) => new { pca.p, pca.c, pca.a, job = jobs.FirstOrDefault() })
            .Select(x => new
            {
                x.p.PreBoardingProfileId,
                candidateName = x.c.FullName ?? "-",
                jobTitle = x.job != null ? x.job.Title : "-",
                profileStatus = x.p.ProfileStatus.ToString(),
                x.p.SubmittedAt,
                x.p.CreatedAt,
                x.p.IsLocked,
                applicationStatus = x.a.ApplicationStatus.ToString(),
                x.p.JobApplicationId
            })
            .OrderByDescending(x => x.PreBoardingProfileId)
            .ToListAsync();

        return Ok(items);
    }

    [HttpGet("fitments")]
    public async Task<ActionResult> ListFitments()
    {
        var items = await _unitOfWork.Context.Set<FitmentData>()
            .Where(f => f.IsActive)
            .Join(_unitOfWork.Context.Set<Candidate>().Where(c => c.IsActive),
                f => f.CandidateId, c => c.CandidateId,
                (f, c) => new { f, c })
            .Join(_unitOfWork.Context.Set<JobApplication>().Where(a => a.IsActive),
                fc => fc.f.JobApplicationId, a => a.JobApplicationId,
                (fc, a) => new { fc.f, fc.c, a })
            .GroupJoin(_unitOfWork.Context.Set<JobPosting>(),
                fca => fca.a.JobPostingId, j => j.JobPostingId,
                (fca, jobs) => new { fca.f, fca.c, fca.a, job = jobs.FirstOrDefault() })
            .Select(x => new
            {
                x.f.FitmentDataId,
                candidateName = x.c.FullName ?? "-",
                jobTitle = x.job != null ? x.job.Title : "-",
                x.f.ProposedRole,
                x.f.ProposedGrade,
                x.f.Location,
                applicationStatus = x.a.ApplicationStatus.ToString(),
                x.f.JobApplicationId
            })
            .OrderByDescending(x => x.FitmentDataId)
            .ToListAsync();

        return Ok(items);
    }

    [HttpGet("onboardings")]
    public async Task<ActionResult> ListOnboardings()
    {
        var items = await _unitOfWork.Context.Set<OnboardingRecord>()
            .Where(o => o.IsActive)
            .Join(_unitOfWork.Context.Set<Candidate>().Where(c => c.IsActive),
                o => o.CandidateId, c => c.CandidateId,
                (o, c) => new { o, c })
            .Join(_unitOfWork.Context.Set<JobApplication>().Where(a => a.IsActive),
                oc => oc.o.JobApplicationId, a => a.JobApplicationId,
                (oc, a) => new { oc.o, oc.c, a })
            .GroupJoin(_unitOfWork.Context.Set<JobPosting>(),
                oca => oca.a.JobPostingId, j => j.JobPostingId,
                (oca, jobs) => new { oca.o, oca.c, oca.a, job = jobs.FirstOrDefault() })
            .Select(x => new
            {
                x.o.OnboardingRecordId,
                candidateName = x.c.FullName ?? "-",
                jobTitle = x.job != null ? x.job.Title : "-",
                stage = x.o.Stage.ToString(),
                applicationStatus = x.a.ApplicationStatus.ToString(),
                x.o.CoreHrEmployeeId,
                x.o.CreatedAt,
                x.o.JobApplicationId
            })
            .OrderByDescending(x => x.OnboardingRecordId)
            .ToListAsync();

        return Ok(items);
    }

    private async Task NotifyInterviewScheduledAsync(JobApplication app, DateTime scheduledDate, string location, string? meetingLink)
    {
        if (!app.CandidateId.HasValue) return;

        var candidate = await _unitOfWork.Context.Set<Candidate>()
            .FirstOrDefaultAsync(c => c.CandidateId == app.CandidateId.Value);
        if (candidate == null || string.IsNullOrEmpty(candidate.KeycloakUserId)) return;

        var jobTitle = await _unitOfWork.Context.Set<JobPosting>()
            .Where(j => j.JobPostingId == app.JobPostingId)
            .Select(j => j.Title)
            .FirstOrDefaultAsync() ?? "a position";

        var locationText = string.IsNullOrEmpty(meetingLink) ? location : $"{location} (Virtual)";
        var fullMessage = $"Your interview for \"{jobTitle}\" has been scheduled on {scheduledDate:MMM dd, yyyy 'at' hh:mm tt} UTC. Location: {locationText}";
        if (!string.IsNullOrEmpty(meetingLink))
            fullMessage += $" | Meeting Link: {meetingLink}";

        var notification = new UserNotification
        {
            KeycloakUserId = candidate.KeycloakUserId,
            Title = "Interview Scheduled",
            Message = fullMessage,
            NotificationType = UserNotificationTypeEnum.Info,
            ActionUrl = "/my-applications",
            IsActive = true
        };
        await _unitOfWork.Context.Set<UserNotification>().AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        if (!string.IsNullOrEmpty(candidate.Email))
        {
            var candidateName = candidate.FullName ?? "Candidate";
            var candidateEmail = candidate.Email;
            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendAsync(candidateEmail, candidateName,
                        $"Interview Scheduled — {jobTitle}",
                        EmailTemplates.InterviewScheduled(candidateName, jobTitle, scheduledDate, location, meetingLink));
                }
                catch { }
            });
        }
    }

    private async Task NotifyCandidateAsync(JobApplication app, string title, string message)
    {
        if (!app.CandidateId.HasValue) return;

        var candidate = await _unitOfWork.Context.Set<Candidate>()
            .FirstOrDefaultAsync(c => c.CandidateId == app.CandidateId.Value);
        if (candidate == null || string.IsNullOrEmpty(candidate.KeycloakUserId)) return;

        var jobTitle = await _unitOfWork.Context.Set<JobPosting>()
            .Where(j => j.JobPostingId == app.JobPostingId)
            .Select(j => j.Title)
            .FirstOrDefaultAsync() ?? "a position";

        var fullMessage = $"{message} (Position: \"{jobTitle}\")";

        var notification = new UserNotification
        {
            KeycloakUserId = candidate.KeycloakUserId,
            Title = title,
            Message = fullMessage,
            NotificationType = UserNotificationTypeEnum.Info,
            ActionUrl = "/my-applications",
            IsActive = true
        };
        await _unitOfWork.Context.Set<UserNotification>().AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        if (!string.IsNullOrEmpty(candidate.Email))
        {
            var candidateName = candidate.FullName ?? "Candidate";
            var candidateEmail = candidate.Email;
            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendAsync(candidateEmail, candidateName,
                        $"{title} — {jobTitle}",
                        EmailTemplates.ApplicationStatusUpdate(candidateName, jobTitle, title));
                }
                catch { }
            });
        }
    }
}

public class ScheduleInterviewRequest
{
    public long JobApplicationId { get; set; }
    public long? InterviewSessionId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string? Location { get; set; }
    public string? MeetingLink { get; set; }
    public string? Round { get; set; }
}

public class CompleteInterviewRequest
{
    public string Result { get; set; } = "Passed";
    public string? Feedback { get; set; }
}

public class StartVerificationRequest
{
    public long JobApplicationId { get; set; }
}

public class CreatePreBoardingRequest
{
    public long JobApplicationId { get; set; }
}

public class CreateFitmentRequest
{
    public long JobApplicationId { get; set; }
    public string? ProposedRole { get; set; }
    public string? ProposedGrade { get; set; }
    public string? Location { get; set; }
    public string? SalaryStructureJson { get; set; }
}

public class StartOnboardingRequest
{
    public long JobApplicationId { get; set; }
}

public class StartStageRequest
{
    public long JobApplicationId { get; set; }
    public long HiringPipelineStageId { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public string? MeetingLink { get; set; }
    public string? Notes { get; set; }
}

public class CompleteStageRequest
{
    public long CandidateStageProgressId { get; set; }
    public bool Passed { get; set; } = true;
    public string? Notes { get; set; }
}
