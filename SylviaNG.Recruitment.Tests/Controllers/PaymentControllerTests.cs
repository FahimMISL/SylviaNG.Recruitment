using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Features.Payments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Controllers;

namespace SylviaNG.Recruitment.Tests.Controllers;

public class PaymentControllerTests
{
    private readonly Mock<IPaymentService> _paymentServiceMock;
    private readonly PaymentController _controller;

    public PaymentControllerTests()
    {
        _paymentServiceMock = new Mock<IPaymentService>();
        var settings = Options.Create(new SslCommerzSettings { FrontendReturnBaseUrl = "http://localhost:4600" });
        _controller = new PaymentController(_paymentServiceMock.Object, settings, NullLogger<PaymentController>.Instance);
    }

    private static IFormCollection CreateForm(Dictionary<string, string> fields)
    {
        var dict = fields.ToDictionary(kv => kv.Key, kv => new Microsoft.Extensions.Primitives.StringValues(kv.Value));
        return new FormCollection(dict);
    }

    [Fact]
    public async Task Initiate_ShouldReturnOkWithServiceResult()
    {
        var expected = new PaymentInitiateResponse { Success = true, GatewayRedirectUrl = "https://sandbox.sslcommerz.com/pay/abc" };
        _paymentServiceMock.Setup(s => s.InitiateAsync(1)).ReturnsAsync(expected);

        var result = await _controller.Initiate(1);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetStatus_ShouldReturnOkWithServiceResult()
    {
        var expected = new PaymentStatusResponse { JobApplicationId = 1, ApplicationStatus = "Applied", PaymentStatus = "Success" };
        _paymentServiceMock.Setup(s => s.GetStatusAsync(1)).ReturnsAsync(expected);

        var result = await _controller.GetStatus(1);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Ipn_WithTranIdAndValId_ShouldDelegateToServiceAndReturnOk()
    {
        var form = CreateForm(new Dictionary<string, string> { ["tran_id"] = "TRAN123", ["val_id"] = "VAL123", ["status"] = "VALID" });

        var result = await _controller.Ipn(form);

        result.Should().BeOfType<OkResult>();
        _paymentServiceMock.Verify(s => s.HandleIpnAsync("TRAN123", "VAL123", It.Is<string>(p => p.Contains("tran_id=TRAN123"))), Times.Once);
    }

    [Fact]
    public async Task Ipn_WithoutTranId_ShouldReturnOkWithoutCallingService()
    {
        var form = CreateForm(new Dictionary<string, string> { ["status"] = "FAILED" });

        var result = await _controller.Ipn(form);

        result.Should().BeOfType<OkResult>();
        _paymentServiceMock.Verify(s => s.HandleIpnAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Ipn_WithMissingValId_ShouldPassNullValidationIdToService()
    {
        var form = CreateForm(new Dictionary<string, string> { ["tran_id"] = "TRAN123", ["status"] = "FAILED" });

        await _controller.Ipn(form);

        _paymentServiceMock.Verify(s => s.HandleIpnAsync("TRAN123", null, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task CallbackSuccess_WithKnownTransaction_ShouldRedirectToFrontendResultPageWithJobApplicationId()
    {
        var form = CreateForm(new Dictionary<string, string> { ["tran_id"] = "TRAN123" });
        _paymentServiceMock.Setup(s => s.GetJobApplicationIdByTransactionIdAsync("TRAN123")).ReturnsAsync(42L);

        var result = await _controller.CallbackSuccess(form);

        var redirect = result.Should().BeOfType<RedirectResult>().Subject;
        redirect.Url.Should().Be("http://localhost:4600/careers/payment-result?jobApplicationId=42&status=success");
    }

    [Fact]
    public async Task CallbackSuccess_WithValId_ShouldTriggerTheSameValidatedConfirmationAsIpn()
    {
        // The browser-return callback carries the same tran_id/val_id SSLCommerz's IPN would -
        // it must drive the SAME validated HandleIpnAsync path (Validation API cross-check),
        // not just redirect based on the request's own "status" field. This is what lets a
        // candidate see a confirmed result even if the async IPN never arrives.
        var form = CreateForm(new Dictionary<string, string> { ["tran_id"] = "TRAN123", ["val_id"] = "VAL123", ["status"] = "VALID" });
        _paymentServiceMock.Setup(s => s.GetJobApplicationIdByTransactionIdAsync("TRAN123")).ReturnsAsync(42L);

        await _controller.CallbackSuccess(form);

        _paymentServiceMock.Verify(s => s.HandleIpnAsync("TRAN123", "VAL123", It.Is<string>(p => p.Contains("tran_id=TRAN123"))), Times.Once);
    }

    [Fact]
    public async Task CallbackFail_WithUnknownTransaction_ShouldRedirectWithoutJobApplicationId()
    {
        var form = CreateForm(new Dictionary<string, string> { ["tran_id"] = "UNKNOWN" });
        _paymentServiceMock.Setup(s => s.GetJobApplicationIdByTransactionIdAsync("UNKNOWN")).ReturnsAsync((long?)null);

        var result = await _controller.CallbackFail(form);

        var redirect = result.Should().BeOfType<RedirectResult>().Subject;
        redirect.Url.Should().Be("http://localhost:4600/careers/payment-result?status=fail");
    }

    [Fact]
    public async Task CallbackCancel_ShouldRedirectWithCancelStatus()
    {
        var form = CreateForm(new Dictionary<string, string> { ["tran_id"] = "TRAN123" });
        _paymentServiceMock.Setup(s => s.GetJobApplicationIdByTransactionIdAsync("TRAN123")).ReturnsAsync(42L);

        var result = await _controller.CallbackCancel(form);

        var redirect = result.Should().BeOfType<RedirectResult>().Subject;
        redirect.Url.Should().Be("http://localhost:4600/careers/payment-result?jobApplicationId=42&status=cancel");
    }

    [Fact]
    public async Task CallbackWithNoTranId_ShouldNotCallHandleIpnAsync()
    {
        var form = CreateForm(new Dictionary<string, string> { ["status"] = "cancelled by user" });

        var result = await _controller.CallbackCancel(form);

        _paymentServiceMock.Verify(s => s.HandleIpnAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string>()), Times.Never);
        var redirect = result.Should().BeOfType<RedirectResult>().Subject;
        redirect.Url.Should().Be("http://localhost:4600/careers/payment-result?status=cancel");
    }
}
