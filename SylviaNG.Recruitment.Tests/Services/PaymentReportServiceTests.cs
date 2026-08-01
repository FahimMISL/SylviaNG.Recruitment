using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Features.PaymentReports.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Tests.Services;

public class PaymentReportServiceTests
{
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
    private readonly Mock<IJobApplicationRepository> _jobApplicationRepositoryMock;
    private readonly Mock<IPaymentReportPdfGeneratorService> _pdfGeneratorMock;
    private readonly PaymentReportService _service;

    public PaymentReportServiceTests()
    {
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _jobApplicationRepositoryMock = new Mock<IJobApplicationRepository>();
        _pdfGeneratorMock = new Mock<IPaymentReportPdfGeneratorService>();

        _service = new PaymentReportService(_paymentRepositoryMock.Object, _jobApplicationRepositoryMock.Object, _pdfGeneratorMock.Object);
    }

    private static Payment Payment(long jobApplicationId, PaymentStatusEnum status, decimal amount = 500m) =>
        new() { JobApplicationId = jobApplicationId, Amount = amount, Currency = "BDT", PaymentStatus = status };

    // ── GetReconciliationAsync ──────────────────────────────────────────────

    [Fact]
    public async Task GetReconciliationAsync_ShouldSumPaidCountFailedCountAndWaivedCountSeparately()
    {
        var request = new ReconciliationRequest { DateFrom = new DateTime(2026, 1, 1), DateTo = new DateTime(2026, 1, 31) };

        _paymentRepositoryMock
            .Setup(r => r.GetLatestPaymentsInScopeAsync(request.DateFrom, request.DateTo, null, null))
            .ReturnsAsync(new List<Payment>
            {
                Payment(1, PaymentStatusEnum.Success, 500m),
                Payment(2, PaymentStatusEnum.Success, 300m),
                Payment(3, PaymentStatusEnum.Failed),
                Payment(4, PaymentStatusEnum.Cancelled)
            });

        _jobApplicationRepositoryMock
            .Setup(r => r.CountWaivedInPeriodAsync(request.DateFrom, request.DateTo, null, null))
            .ReturnsAsync(2);

        var result = await _service.GetReconciliationAsync(request);

        result.PaidCount.Should().Be(2);
        result.PaidAmount.Should().Be(800m);
        result.FailedCount.Should().Be(2);
        result.WaivedCount.Should().Be(2);
        result.NetAmount.Should().Be(800m);
    }

    [Fact]
    public async Task GetReconciliationAsync_WithNoTransactions_ShouldReturnAllZeros()
    {
        var request = new ReconciliationRequest { DateFrom = new DateTime(2026, 1, 1), DateTo = new DateTime(2026, 1, 31) };

        _paymentRepositoryMock
            .Setup(r => r.GetLatestPaymentsInScopeAsync(request.DateFrom, request.DateTo, null, null))
            .ReturnsAsync(new List<Payment>());
        _jobApplicationRepositoryMock
            .Setup(r => r.CountWaivedInPeriodAsync(request.DateFrom, request.DateTo, null, null))
            .ReturnsAsync(0);

        var result = await _service.GetReconciliationAsync(request);

        result.PaidCount.Should().Be(0);
        result.PaidAmount.Should().Be(0m);
        result.NetAmount.Should().Be(0m);
    }

    // ── GetTransactionsAsync ────────────────────────────────────────────────

    [Fact]
    public async Task GetTransactionsAsync_ShouldMapPagedPaymentsAndIncludeTotalAmount()
    {
        var jobPosting = new JobPosting { JobPostingId = 1, Title = "Software Engineer" };
        var jobApplication = new JobApplication { JobApplicationId = 1, CandidateName = "Jane Doe", CandidateEmail = "jane@example.com", JobPosting = jobPosting };
        var payment = new Payment { PaymentId = 1, JobApplicationId = 1, Amount = 500m, Currency = "BDT", TransactionId = "TRAN1", PaymentStatus = PaymentStatusEnum.Success, JobApplication = jobApplication };

        var paging = new PagedRequest { Page = 1, PageSize = 10 };
        var filter = new PaymentTransactionFilterRequest();

        _paymentRepositoryMock.Setup(r => r.GetPagedTransactionsAsync(filter, paging))
            .ReturnsAsync(new PagedResult<Payment> { Data = new List<Payment> { payment }, TotalCount = 1, PageNumber = 1, PageSize = 10 });
        _paymentRepositoryMock.Setup(r => r.GetFilteredTotalAmountAsync(filter)).ReturnsAsync(500m);

        var result = await _service.GetTransactionsAsync(paging, filter);

        result.Data.Should().ContainSingle();
        result.Data[0].CandidateName.Should().Be("Jane Doe");
        result.Data[0].VacancyTitle.Should().Be("Software Engineer");
        result.TotalAmount.Should().Be(500m);
    }

    // ── ExportReconciliationAsync ───────────────────────────────────────────

    [Fact]
    public async Task ExportReconciliationAsync_WithPdfFormat_ShouldUsePdfGenerator()
    {
        var request = new ReconciliationRequest { DateFrom = new DateTime(2026, 1, 1), DateTo = new DateTime(2026, 1, 31) };
        _paymentRepositoryMock.Setup(r => r.GetLatestPaymentsInScopeAsync(request.DateFrom, request.DateTo, null, null))
            .ReturnsAsync(new List<Payment>());
        _jobApplicationRepositoryMock.Setup(r => r.CountWaivedInPeriodAsync(request.DateFrom, request.DateTo, null, null))
            .ReturnsAsync(0);
        _pdfGeneratorMock.Setup(g => g.GenerateReconciliationPdfAsync(request, It.IsAny<ReconciliationResponse>()))
            .ReturnsAsync(new byte[] { 1, 2, 3 });

        var result = await _service.ExportReconciliationAsync(request, "pdf");

        result.ContentType.Should().Be("application/pdf");
        result.FileName.Should().EndWith(".pdf");
        result.Content.Should().Equal(new byte[] { 1, 2, 3 });
    }

    [Fact]
    public async Task ExportReconciliationAsync_WithXlsxFormat_ShouldBuildExcelWorkbookWithoutCallingPdfGenerator()
    {
        var request = new ReconciliationRequest { DateFrom = new DateTime(2026, 1, 1), DateTo = new DateTime(2026, 1, 31) };
        _paymentRepositoryMock.Setup(r => r.GetLatestPaymentsInScopeAsync(request.DateFrom, request.DateTo, null, null))
            .ReturnsAsync(new List<Payment>());
        _jobApplicationRepositoryMock.Setup(r => r.CountWaivedInPeriodAsync(request.DateFrom, request.DateTo, null, null))
            .ReturnsAsync(0);

        var result = await _service.ExportReconciliationAsync(request, "xlsx");

        result.ContentType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        result.FileName.Should().EndWith(".xlsx");
        result.Content.Should().NotBeEmpty();
        _pdfGeneratorMock.Verify(g => g.GenerateReconciliationPdfAsync(It.IsAny<ReconciliationRequest>(), It.IsAny<ReconciliationResponse>()), Times.Never);
    }
}
