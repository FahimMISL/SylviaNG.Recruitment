using SylviaNG.Recruitment.Application.Features.ExamHalls.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    /// <summary>
    /// Manual mapping methods for ExamHall. No AutoMapper, matching QuestionGroupMapper.
    /// </summary>
    public static class ExamHallMapper
    {
        public static ExamHall ToEntity(this ExamHallCreateRequest request)
        {
            return new ExamHall
            {
                HallName = request.HallName,
                Location = request.Location,
                TotalCapacity = request.TotalCapacity,
                NotifyInvigilatorsOnAssign = request.NotifyInvigilatorsOnAssign,
                IsActive = true,
                Invigilators = request.InvigilatorEmployeeIds
                    .Distinct()
                    .Select(employeeId => new ExamHallInvigilator { EmployeeId = employeeId })
                    .ToList()
            };
        }

        public static ExamHallResponse ToResponse(this ExamHall entity)
        {
            return new ExamHallResponse
            {
                ExamHallId = entity.ExamHallId,
                HallName = entity.HallName,
                Location = entity.Location,
                TotalCapacity = entity.TotalCapacity,
                NotifyInvigilatorsOnAssign = entity.NotifyInvigilatorsOnAssign,
                IsActive = entity.IsActive,
                InvigilatorEmployeeIds = entity.Invigilators?.Select(i => i.EmployeeId).ToList() ?? new List<long>()
            };
        }

        public static ExamHallLookupResponse ToLookupResponse(this ExamHall entity)
        {
            return new ExamHallLookupResponse
            {
                ExamHallId = entity.ExamHallId,
                HallName = entity.HallName
            };
        }
    }
}
