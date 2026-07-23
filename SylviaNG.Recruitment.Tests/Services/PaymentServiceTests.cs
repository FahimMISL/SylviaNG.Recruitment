using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Interfaces.Externals;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class PaymentServiceTests
{
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
    private readonly Mock<IJobApplicationRepository> _jobApplicationRepositoryMock;
    private readonly Mock<ISslCommerzPaymentGateway> _gatewayMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly PaymentService _service;

    public PaymentServiceTests()
    {
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _jobApplicationRepositoryMock = new Mock<IJobApplicationRepository>();
        _gatewayMock = new Mock<ISslCommerzPaymentGateway>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new PaymentService(
            _paymentRepositoryMock.Object,
            _jobApplicationRepositoryMock.Object,
            _gatewayMock.Object,
            _unitOfWorkMock.Object,
            NullLogger<PaymentService>.Instance);
    }

    private static JobApplication CreateAwaitingPaymentApplication(long jobApplicationId = 1, decimal feeAmount = 500m)
    {
        return new JobApplication
        {
            JobApplicationId = jobApplicationId,
            CandidateName = "Jane Doe",
            CandidateEmail = "jane@example.com",
            CandidatePhone = "+880123456789",
            ApplicationStatus = ApplicationStatusEnum.AwaitingPayment,
            JobPosting = new JobPosting { JobPostingId = 1, Title = "Software Engineer", ApplicationFeeAmount = feeAmount, ApplicationFeeCurrency = "BDT" }
        };
    }

    // ── InitiateAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task InitiateAsync_WithAwaitingPaymentApplication_ShouldOpenSessionAndPersistPayment()
    {
        var jobApplication = CreateAwaitingPaymentApplication();
        _jobApplicationRepositoryMock
            .Setup(r => r.GetByIdWithIncludeAsync(It.IsAny<System.Linq.Expressions.Expression<Func<JobApplication, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<JobApplication, object>>[]>()))
            .ReturnsAsync(jobApplication);
        _paymentRepositoryMock.Setup(r => r.HasSuccessfulPaymentAsync(1)).ReturnsAsync(false);

        _gatewayMock.Setup(g => g.InitiateSessionAsync(It.IsAny<SslCommerzSessionRequest>()))
            .ReturnsAsync(new SslCommerzSessionResult(true, "https://sandbox.sslcommerz.com/pay/abc", "session-key-1", null));

        var result = await _service.InitiateAsync(1);

        result.Success.Should().BeTrue();
        result.GatewayRedirectUrl.Should().Be("https://sandbox.sslcommerz.com/pay/abc");
        _paymentRepositoryMock.Verify(r => r.AddAsync(It.Is<Payment>(p =>
            p.JobApplicationId == 1 && p.Amount == 500m && p.Currency == "BDT" && p.PaymentStatus == PaymentStatusEnum.Initiated)), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task InitiateAsync_WhenApplicationAlreadyApplied_ShouldReturnFailureWithoutCallingGateway()
    {
        var jobApplication = CreateAwaitingPaymentApplication();
        jobApplication.ApplicationStatus = ApplicationStatusEnum.Applied;
        _jobApplicationRepositoryMock
            .Setup(r => r.GetByIdWithIncludeAsync(It.IsAny<System.Linq.Expressions.Expression<Func<JobApplication, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<JobApplication, object>>[]>()))
            .ReturnsAsync(jobApplication);

        var result = await _service.InitiateAsync(1);

        result.Success.Should().BeFalse();
        _gatewayMock.Verify(g => g.InitiateSessionAsync(It.IsAny<SslCommerzSessionRequest>()), Times.Never);
    }

    [Fact]
    public async Task InitiateAsync_WhenApplicationDoesNotExist_ShouldThrowNotFoundException()
    {
        _jobApplicationRepositoryMock
            .Setup(r => r.GetByIdWithIncludeAsync(It.IsAny<System.Linq.Expressions.Expression<Func<JobApplication, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<JobApplication, object>>[]>()))
            .ReturnsAsync((JobApplication?)null);

        var act = () => _service.InitiateAsync(999);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task InitiateAsync_WhenGatewayRejectsSession_ShouldReturnFailureAndNotPersistPayment()
    {
        var jobApplication = CreateAwaitingPaymentApplication();
        _jobApplicationRepositoryMock
            .Setup(r => r.GetByIdWithIncludeAsync(It.IsAny<System.Linq.Expressions.Expression<Func<JobApplication, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<JobApplication, object>>[]>()))
            .ReturnsAsync(jobApplication);
        _paymentRepositoryMock.Setup(r => r.HasSuccessfulPaymentAsync(1)).ReturnsAsync(false);

        _gatewayMock.Setup(g => g.InitiateSessionAsync(It.IsAny<SslCommerzSessionRequest>()))
            .ReturnsAsync(new SslCommerzSessionResult(false, null, null, "Invalid store credentials."));

        var result = await _service.InitiateAsync(1);

        result.Success.Should().BeFalse();
        result.FailureReason.Should().Be("Invalid store credentials.");
        _paymentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Payment>()), Times.Never);
    }

    // ── HandleIpnAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task HandleIpnAsync_WithValidMatchingTransaction_ShouldMarkPaymentSuccessAndApplicationApplied()
    {
        var payment = new Payment { PaymentId = 1, JobApplicationId = 1, Amount = 500m, Currency = "BDT", TransactionId = "TRAN123", PaymentStatus = PaymentStatusEnum.Initiated };
        _paymentRepositoryMock.Setup(r => r.GetByTransactionIdAsync("TRAN123")).ReturnsAsync(payment);
        _gatewayMock.Setup(g => g.ValidateTransactionAsync("VAL123"))
            .ReturnsAsync(new SslCommerzValidationResult(true, "VALID", 500m, "BDT", "TRAN123"));

        var jobApplication = new JobApplication { JobApplicationId = 1, ApplicationStatus = ApplicationStatusEnum.AwaitingPayment };
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(jobApplication);

        await _service.HandleIpnAsync("TRAN123", "VAL123", "tran_id=TRAN123&val_id=VAL123");

        payment.PaymentStatus.Should().Be(PaymentStatusEnum.Success);
        payment.ValidationId.Should().Be("VAL123");
        payment.PaidAt.Should().NotBeNull();
        jobApplication.ApplicationStatus.Should().Be(ApplicationStatusEnum.Applied);
        jobApplication.StatusHistory.Should().ContainSingle(h => h.ChangedByUserName == "system:sslcommerz-ipn" && h.ToStatus == ApplicationStatusEnum.Applied);
    }

    [Fact]
    public async Task HandleIpnAsync_WithAmountMismatch_ShouldMarkPaymentFailedAndLeaveApplicationAwaitingPayment()
    {
        var payment = new Payment { PaymentId = 1, JobApplicationId = 1, Amount = 500m, Currency = "BDT", TransactionId = "TRAN123", PaymentStatus = PaymentStatusEnum.Initiated };
        _paymentRepositoryMock.Setup(r => r.GetByTransactionIdAsync("TRAN123")).ReturnsAsync(payment);
        // Attacker/mismatch scenario: validation reports a different (lower) amount than what was recorded.
        _gatewayMock.Setup(g => g.ValidateTransactionAsync("VAL123"))
            .ReturnsAsync(new SslCommerzValidationResult(true, "VALID", 1m, "BDT", "TRAN123"));

        var jobApplication = new JobApplication { JobApplicationId = 1, ApplicationStatus = ApplicationStatusEnum.AwaitingPayment };
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(jobApplication);

        await _service.HandleIpnAsync("TRAN123", "VAL123", "tran_id=TRAN123&val_id=VAL123");

        payment.PaymentStatus.Should().Be(PaymentStatusEnum.Failed);
        jobApplication.ApplicationStatus.Should().Be(ApplicationStatusEnum.AwaitingPayment);
    }

    [Fact]
    public async Task HandleIpnAsync_WithNoValId_ShouldMarkPaymentFailedWithoutCallingValidationApi()
    {
        var payment = new Payment { PaymentId = 1, JobApplicationId = 1, Amount = 500m, Currency = "BDT", TransactionId = "TRAN123", PaymentStatus = PaymentStatusEnum.Initiated };
        _paymentRepositoryMock.Setup(r => r.GetByTransactionIdAsync("TRAN123")).ReturnsAsync(payment);

        await _service.HandleIpnAsync("TRAN123", null, "tran_id=TRAN123&status=FAILED");

        payment.PaymentStatus.Should().Be(PaymentStatusEnum.Failed);
        _gatewayMock.Verify(g => g.ValidateTransactionAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task HandleIpnAsync_WithUnknownTransactionId_ShouldNoOp()
    {
        _paymentRepositoryMock.Setup(r => r.GetByTransactionIdAsync("UNKNOWN")).ReturnsAsync((Payment?)null);

        await _service.HandleIpnAsync("UNKNOWN", "VAL123", "tran_id=UNKNOWN");

        _gatewayMock.Verify(g => g.ValidateTransactionAsync(It.IsAny<string>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task HandleIpnAsync_RedeliveredAfterAlreadySuccess_ShouldBeIdempotentAndNotDuplicateStatusHistory()
    {
        // A duplicate IPN delivery for a tran_id already marked Success must not re-flip the
        // application's status a second time (JobApplication is no longer AwaitingPayment).
        var payment = new Payment { PaymentId = 1, JobApplicationId = 1, Amount = 500m, Currency = "BDT", TransactionId = "TRAN123", PaymentStatus = PaymentStatusEnum.Success, ValidationId = "VAL123" };
        _paymentRepositoryMock.Setup(r => r.GetByTransactionIdAsync("TRAN123")).ReturnsAsync(payment);
        _gatewayMock.Setup(g => g.ValidateTransactionAsync("VAL123"))
            .ReturnsAsync(new SslCommerzValidationResult(true, "VALID", 500m, "BDT", "TRAN123"));

        var jobApplication = new JobApplication { JobApplicationId = 1, ApplicationStatus = ApplicationStatusEnum.Applied };
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(jobApplication);

        await _service.HandleIpnAsync("TRAN123", "VAL123", "tran_id=TRAN123&val_id=VAL123");

        jobApplication.ApplicationStatus.Should().Be(ApplicationStatusEnum.Applied);
        jobApplication.StatusHistory.Should().BeEmpty();
        payment.PaymentStatus.Should().Be(PaymentStatusEnum.Success);
    }

    // ── GetStatusAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task GetStatusAsync_ShouldReturnLatestPaymentAndApplicationStatus()
    {
        var jobApplication = new JobApplication { JobApplicationId = 1, ApplicationStatus = ApplicationStatusEnum.Applied };
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(jobApplication);

        var payment = new Payment { PaymentId = 2, JobApplicationId = 1, Amount = 500m, Currency = "BDT", PaymentStatus = PaymentStatusEnum.Success, PaidAt = DateTime.UtcNow };
        _paymentRepositoryMock.Setup(r => r.GetLatestByJobApplicationIdAsync(1)).ReturnsAsync(payment);

        var result = await _service.GetStatusAsync(1);

        result.ApplicationStatus.Should().Be("Applied");
        result.PaymentStatus.Should().Be("Success");
        result.Amount.Should().Be(500m);
    }

    [Fact]
    public async Task GetStatusAsync_WhenApplicationDoesNotExist_ShouldThrowNotFoundException()
    {
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((JobApplication?)null);

        var act = () => _service.GetStatusAsync(999);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
