using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Commands.JobApplicationSubmit
{
    /// <summary>
    /// Anonymous/authenticated candidate apply flow, shared by the career portal (Source=External)
    /// and internal job board (Source=Internal) controllers.
    /// </summary>
    public class JobApplicationSubmitCommand : IRequest<JobApplicationResponse>
    {
        public JobApplicationSubmitRequest Request { get; set; }
        public ApplicationSourceEnum Source { get; set; }

        public JobApplicationSubmitCommand(JobApplicationSubmitRequest request, ApplicationSourceEnum source)
        {
            Request = request;
            Source = source;
        }
    }
}
