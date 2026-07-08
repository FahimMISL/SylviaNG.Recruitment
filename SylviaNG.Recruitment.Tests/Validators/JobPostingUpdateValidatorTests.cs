using FluentAssertions;
using SylviaNG.Recruitment.Application.Features.JobPostings.Commands.JobPostingUpdate;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;

namespace SylviaNG.Recruitment.Tests.Validators;

public class JobPostingUpdateValidatorTests
{
    private readonly JobPostingUpdateValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new JobPostingUpdateCommand(1, new JobPostingUpdateRequest
        {
            Title = "Software Engineer",
            MinAge = 20,
            MaxAge = 35,
            MinExperienceYears = 2,
            ApplicationFeeAmount = 500,
            ApplicationFeeCurrency = "BDT",
            Location = "Dhaka",
            RequiredDistrict = "Dhaka"
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithZeroJobPostingId_ShouldHaveError()
    {
        // Arrange
        var command = new JobPostingUpdateCommand(0, new JobPostingUpdateRequest());

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "JobPostingId");
    }

    [Fact]
    public void Validate_WithMinSalaryGreaterThanMaxSalary_ShouldHaveError()
    {
        // Arrange
        var command = new JobPostingUpdateCommand(1, new JobPostingUpdateRequest
        {
            MinSalary = 100000,
            MaxSalary = 50000
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.MinSalary");
    }

    [Fact]
    public void Validate_WithClosingDateBeforePostingDate_ShouldHaveError()
    {
        // Arrange
        var command = new JobPostingUpdateCommand(1, new JobPostingUpdateRequest
        {
            PostingDate = new DateTime(2026, 6, 30),
            ClosingDate = new DateTime(2026, 1, 1)
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.ClosingDate");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Validate_WithInvalidMinAge_ShouldHaveError(int minAge)
    {
        // Arrange
        var command = new JobPostingUpdateCommand(1, new JobPostingUpdateRequest
        {
            MinAge = minAge
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.MinAge");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithInvalidMaxAge_ShouldHaveError(int maxAge)
    {
        // Arrange
        var command = new JobPostingUpdateCommand(1, new JobPostingUpdateRequest
        {
            MaxAge = maxAge
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.MaxAge");
    }

    [Fact]
    public void Validate_WithMinAgeGreaterThanMaxAge_ShouldHaveError()
    {
        // Arrange
        var command = new JobPostingUpdateCommand(1, new JobPostingUpdateRequest
        {
            MinAge = 40,
            MaxAge = 30
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.MinAge");
    }

    [Fact]
    public void Validate_WithNegativeMinExperienceYears_ShouldHaveError()
    {
        // Arrange
        var command = new JobPostingUpdateCommand(1, new JobPostingUpdateRequest
        {
            MinExperienceYears = -1
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.MinExperienceYears");
    }

    [Fact]
    public void Validate_WithZeroExperienceYears_ShouldBeValid()
    {
        // Arrange
        var command = new JobPostingUpdateCommand(1, new JobPostingUpdateRequest
        {
            MinExperienceYears = 0
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithNegativeApplicationFeeAmount_ShouldHaveError()
    {
        // Arrange
        var command = new JobPostingUpdateCommand(1, new JobPostingUpdateRequest
        {
            ApplicationFeeAmount = -10,
            ApplicationFeeCurrency = "BDT"
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.ApplicationFeeAmount");
    }

    [Fact]
    public void Validate_WithZeroApplicationFeeAmountAndCurrency_ShouldBeValid()
    {
        // Arrange
        var command = new JobPostingUpdateCommand(1, new JobPostingUpdateRequest
        {
            ApplicationFeeAmount = 0,
            ApplicationFeeCurrency = "BDT"
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithApplicationFeeAmountButNoCurrency_ShouldHaveError()
    {
        // Arrange
        var command = new JobPostingUpdateCommand(1, new JobPostingUpdateRequest
        {
            ApplicationFeeAmount = 100
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.ApplicationFeeCurrency");
    }

    [Fact]
    public void Validate_WithLocationExceedingMaxLength_ShouldHaveError()
    {
        // Arrange
        var command = new JobPostingUpdateCommand(1, new JobPostingUpdateRequest
        {
            Location = new string('A', 201)
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.Location");
    }

    [Fact]
    public void Validate_WithRequiredDistrictExceedingMaxLength_ShouldHaveError()
    {
        // Arrange
        var command = new JobPostingUpdateCommand(1, new JobPostingUpdateRequest
        {
            RequiredDistrict = new string('B', 101)
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.RequiredDistrict");
    }

    [Fact]
    public void Validate_WithTitleExceedingMaxLength_ShouldHaveError()
    {
        // Arrange
        var command = new JobPostingUpdateCommand(1, new JobPostingUpdateRequest
        {
            Title = new string('C', 201)
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.Title");
    }
}
