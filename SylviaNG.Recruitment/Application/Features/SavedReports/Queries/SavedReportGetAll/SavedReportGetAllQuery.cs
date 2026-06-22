using MediatR;
using SylviaNG.Recruitment.Application.Features.SavedReports.Models;

namespace SylviaNG.Recruitment.Application.Features.SavedReports.Queries.SavedReportGetAll
{
    public class SavedReportGetAllQuery : IRequest<List<SavedReportResponse>>
    {
    }
}
