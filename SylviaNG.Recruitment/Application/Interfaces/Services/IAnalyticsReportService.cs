using SylviaNG.Recruitment.Application.Features.Analytics.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>EP-14 US-106/US-107: recruitment funnel + time-to-hire analytics reports.</summary>
    public interface IAnalyticsReportService
    {
        Task<RecruitmentFunnelResponse> GetRecruitmentFunnelAsync(RecruitmentFunnelRequest request);
        Task<byte[]> ExportRecruitmentFunnelCsvAsync(RecruitmentFunnelRequest request);
        Task<TimeToHireResponse> GetTimeToHireAsync(TimeToHireRequest request);
        Task<byte[]> ExportTimeToHireCsvAsync(TimeToHireRequest request);
    }
}
