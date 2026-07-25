using SylviaNG.Recruitment.Application.Features.InterviewRoundConfigs.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class InterviewRoundConfigMapper
    {
        public static InterviewRoundConfig ToEntity(this InterviewRoundConfigRequest request)
        {
            return new InterviewRoundConfig
            {
                Name = request.Name,
                Sequence = request.Sequence,
                ScorecardId = request.ScorecardId,
                PanelMembers = request.PanelistEmployeeIds.Distinct()
                    .Select(id => new InterviewRoundConfigPanelMember { EmployeeId = id })
                    .ToList(),
            };
        }

        public static InterviewRoundConfigResponse ToResponse(this InterviewRoundConfig entity)
        {
            return new InterviewRoundConfigResponse
            {
                InterviewRoundConfigId = entity.InterviewRoundConfigId,
                JobPostingId = entity.JobPostingId,
                Name = entity.Name,
                Sequence = entity.Sequence,
                ScorecardId = entity.ScorecardId,
                ScorecardName = entity.Scorecard?.Name,
                PanelistEmployeeIds = entity.PanelMembers.Select(p => p.EmployeeId).ToList(),
            };
        }
    }
}
