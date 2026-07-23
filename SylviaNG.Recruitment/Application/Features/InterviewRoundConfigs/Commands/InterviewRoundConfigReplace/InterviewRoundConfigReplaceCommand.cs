using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewRoundConfigs.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewRoundConfigs.Commands.InterviewRoundConfigReplace
{
    public class InterviewRoundConfigReplaceCommand : IRequest
    {
        public long JobPostingId { get; set; }
        public InterviewRoundConfigReplaceRequest Request { get; set; }

        public InterviewRoundConfigReplaceCommand(long jobPostingId, InterviewRoundConfigReplaceRequest request)
        {
            JobPostingId = jobPostingId;
            Request = request;
        }
    }
}
