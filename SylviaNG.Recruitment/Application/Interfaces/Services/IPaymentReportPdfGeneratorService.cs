using SylviaNG.Recruitment.Application.Features.PaymentReports.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IPaymentReportPdfGeneratorService
    {
        Task<byte[]> GenerateReconciliationPdfAsync(ReconciliationRequest request, ReconciliationResponse summary);
    }
}
