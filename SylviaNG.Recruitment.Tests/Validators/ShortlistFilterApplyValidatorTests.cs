using FluentAssertions;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Commands.ShortlistFilterApply;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models;

namespace SylviaNG.Recruitment.Tests.Validators;

public class ShortlistFilterApplyValidatorTests
{
    private readonly ShortlistFilterApplyValidator _validator = new();

    [Fact]
    public void Validate_WithFilterIdAndJobPostingId_ShouldHaveNoErrors()
    {
        var command = new ShortlistFilterApplyCommand(new ShortlistFilterApplyRequest { ShortlistFilterId = 1, JobPostingId = 1 });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithNoShortlistFilterId_ShouldHaveError()
    {
        var command = new ShortlistFilterApplyCommand(new ShortlistFilterApplyRequest { ShortlistFilterId = 0, JobPostingId = 1 });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithNoJobPostingId_ShouldHaveError()
    {
        var command = new ShortlistFilterApplyCommand(new ShortlistFilterApplyRequest { ShortlistFilterId = 1, JobPostingId = 0 });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}
