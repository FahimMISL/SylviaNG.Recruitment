using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Queries.JobPostingGetAllPublicPaged
{
    /// <summary>
    /// Anonymous career-portal browse: paginated Open postings with CircularType ExternalOnly or Both.
    /// </summary>
    public class JobPostingGetAllPublicPagedQuery : IRequest<PagedResult<JobPostingResponse>>
    {
        public PagedRequest Request { get; set; }
        public string? Location { get; set; }
        public long? DepartmentId { get; set; }
        public EmploymentTypeEnum? EmploymentType { get; set; }
        public int? MaxExperienceYears { get; set; }

        public JobPostingGetAllPublicPagedQuery(PagedRequest request, string? location, long? departmentId, EmploymentTypeEnum? employmentType, int? maxExperienceYears)
        {
            Request = request;
            Location = location;
            DepartmentId = departmentId;
            EmploymentType = employmentType;
            MaxExperienceYears = maxExperienceYears;
        }
    }
}
