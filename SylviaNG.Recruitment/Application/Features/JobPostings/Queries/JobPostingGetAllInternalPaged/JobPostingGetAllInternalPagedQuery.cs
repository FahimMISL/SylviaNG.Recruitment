using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Queries.JobPostingGetAllInternalPaged
{
    /// <summary>
    /// Internal job board browse (any authenticated user): paginated Open postings with
    /// CircularType InternalOnly or Both.
    /// </summary>
    public class JobPostingGetAllInternalPagedQuery : IRequest<PagedResult<JobPostingResponse>>
    {
        public PagedRequest Request { get; set; }
        public string? Location { get; set; }
        public long? DepartmentId { get; set; }
        public EmploymentTypeEnum? EmploymentType { get; set; }
        public int? MaxExperienceYears { get; set; }

        public JobPostingGetAllInternalPagedQuery(PagedRequest request, string? location, long? departmentId, EmploymentTypeEnum? employmentType, int? maxExperienceYears)
        {
            Request = request;
            Location = location;
            DepartmentId = departmentId;
            EmploymentType = employmentType;
            MaxExperienceYears = maxExperienceYears;
        }
    }
}
