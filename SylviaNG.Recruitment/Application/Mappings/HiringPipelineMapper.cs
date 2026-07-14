using SylviaNG.Recruitment.Application.Features.HiringPipelines.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    /// <summary>
    /// Manual mapping methods for HiringPipeline and PipelineStage entities.
    /// </summary>
    public static class HiringPipelineMapper
    {
        public static PipelineStage ToEntity(this PipelineStageRequest request)
        {
            return new PipelineStage
            {
                Name = request.Name,
                StageType = request.StageType,
                DisplayOrder = request.DisplayOrder,
                Description = request.Description,
                PassingCriteria = request.PassingCriteria,
                IsActive = true,
                IsMandatory = request.IsMandatory,
                DepartmentId = request.DepartmentId,
                EstimatedDurationMinutes = request.EstimatedDurationMinutes,
                SlaDays = request.SlaDays,
                ColorBadge = request.ColorBadge,
                EmailTemplate = request.EmailTemplate,
                NotifyCandidateOnEnter = request.NotifyCandidateOnEnter,
                NotifyInterviewersOnAssign = request.NotifyInterviewersOnAssign,
                RequiredDocuments = request.RequiredDocuments,
                AllowCandidateReschedule = request.AllowCandidateReschedule,
                AutoProgressionRule = request.AutoProgressionRule,
                ManualApprovalRequired = request.ManualApprovalRequired,
                Interviewers = request.InterviewerEmployeeIds
                    .Distinct()
                    .Select(employeeId => new PipelineStageInterviewer { EmployeeId = employeeId })
                    .ToList()
            };
        }

        public static PipelineStageResponse ToResponse(this PipelineStage entity)
        {
            return new PipelineStageResponse
            {
                PipelineStageId = entity.PipelineStageId,
                Name = entity.Name,
                StageType = entity.StageType,
                DisplayOrder = entity.DisplayOrder,
                Description = entity.Description,
                PassingCriteria = entity.PassingCriteria,
                IsActive = entity.IsActive,
                IsMandatory = entity.IsMandatory,
                DepartmentId = entity.DepartmentId,
                EstimatedDurationMinutes = entity.EstimatedDurationMinutes,
                SlaDays = entity.SlaDays,
                ColorBadge = entity.ColorBadge,
                EmailTemplate = entity.EmailTemplate,
                NotifyCandidateOnEnter = entity.NotifyCandidateOnEnter,
                NotifyInterviewersOnAssign = entity.NotifyInterviewersOnAssign,
                RequiredDocuments = entity.RequiredDocuments,
                AllowCandidateReschedule = entity.AllowCandidateReschedule,
                AutoProgressionRule = entity.AutoProgressionRule,
                ManualApprovalRequired = entity.ManualApprovalRequired,
                InterviewerEmployeeIds = entity.Interviewers?.Select(i => i.EmployeeId).ToList() ?? new List<long>()
            };
        }

        public static HiringPipeline ToEntity(this HiringPipelineCreateRequest request)
        {
            return new HiringPipeline
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = true,
                Stages = request.Stages.OrderBy(s => s.DisplayOrder).Select(s => s.ToEntity()).ToList()
            };
        }

        public static HiringPipelineResponse ToResponse(this HiringPipeline entity)
        {
            return new HiringPipelineResponse
            {
                HiringPipelineId = entity.HiringPipelineId,
                Name = entity.Name,
                Description = entity.Description,
                IsActive = entity.IsActive,
                JobPostingCount = entity.JobPostings?.Count ?? 0,
                Stages = entity.Stages?
                    .OrderBy(s => s.DisplayOrder)
                    .Select(s => s.ToResponse())
                    .ToList() ?? new List<PipelineStageResponse>()
            };
        }

        public static HiringPipelineLookupResponse ToLookupResponse(this HiringPipeline entity)
        {
            return new HiringPipelineLookupResponse
            {
                HiringPipelineId = entity.HiringPipelineId,
                Name = entity.Name
            };
        }
    }
}
