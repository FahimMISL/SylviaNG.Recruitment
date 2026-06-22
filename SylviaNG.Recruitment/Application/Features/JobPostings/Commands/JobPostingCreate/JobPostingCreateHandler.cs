using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.Infrastructure.Kafka;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Commands.JobPostingCreate
{
    public class JobPostingCreateHandler : IRequestHandler<JobPostingCreateCommand, long>
    {
        private readonly IJobPostingService _jobPostingService;
        private readonly IRecruitmentEventProducer _eventProducer;
        private readonly IUserNotificationService _notificationService;

        public JobPostingCreateHandler(IJobPostingService jobPostingService, IRecruitmentEventProducer eventProducer, IUserNotificationService notificationService)
        {
            _jobPostingService = jobPostingService;
            _eventProducer = eventProducer;
            _notificationService = notificationService;
        }

        public async Task<long> Handle(JobPostingCreateCommand command, CancellationToken cancellationToken)
        {
            var id = await _jobPostingService.CreateAsync(command.Request);

            await _eventProducer.PublishAsync(NotificationEventConsumer.TOPIC_JOB_POSTING, new { action = "CREATED", jobPostingId = id, title = command.Request.Title });
            await _notificationService.NotifyRoleAsync("Admin", "New Job Posting Created", $"\"{command.Request.Title}\" has been created.", UserNotificationTypeEnum.Info, "/job-postings");

            return id;
        }
    }
}
