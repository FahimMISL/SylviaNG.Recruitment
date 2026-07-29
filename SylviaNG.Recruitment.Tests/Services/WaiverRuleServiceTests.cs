using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.WaiverRules.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class WaiverRuleServiceTests
{
    private readonly Mock<IWaiverRuleRepository> _waiverRuleRepositoryMock;
    private readonly Mock<ISpecialCategoryRepository> _specialCategoryRepositoryMock;
    private readonly Mock<IReferralSourceRepository> _referralSourceRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly WaiverRuleService _service;

    public WaiverRuleServiceTests()
    {
        _waiverRuleRepositoryMock = new Mock<IWaiverRuleRepository>();
        _specialCategoryRepositoryMock = new Mock<ISpecialCategoryRepository>();
        _referralSourceRepositoryMock = new Mock<IReferralSourceRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new WaiverRuleService(
            _waiverRuleRepositoryMock.Object,
            _specialCategoryRepositoryMock.Object,
            _referralSourceRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    // ── CreateAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_WithDuplicateName_ShouldThrowDuplicateException()
    {
        _waiverRuleRepositoryMock.Setup(r => r.ExistsByNameAsync("Internal Staff", null)).ReturnsAsync(true);

        var act = () => _service.CreateAsync(new WaiverRuleCreateRequest { Name = "Internal Staff" });

        await act.Should().ThrowAsync<DuplicateException>();
        _waiverRuleRepositoryMock.Verify(r => r.AddAsync(It.IsAny<WaiverRule>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithNonExistentSpecialCategory_ShouldThrowNotFoundException()
    {
        _waiverRuleRepositoryMock.Setup(r => r.ExistsByNameAsync("Rule A", null)).ReturnsAsync(false);
        _specialCategoryRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((SpecialCategory?)null);

        var act = () => _service.CreateAsync(new WaiverRuleCreateRequest { Name = "Rule A", SpecialCategoryId = 99 });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldPersistAndReturnId()
    {
        _waiverRuleRepositoryMock.Setup(r => r.ExistsByNameAsync("Internal Staff", null)).ReturnsAsync(false);
        _waiverRuleRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<WaiverRule>()))
            .Callback<WaiverRule>(w => w.WaiverRuleId = 5)
            .Returns(Task.CompletedTask);

        var id = await _service.CreateAsync(new WaiverRuleCreateRequest
        {
            Name = "Internal Staff",
            CandidateTypeFilter = WaiverCandidateTypeEnum.Internal,
            Priority = 1,
            IsActive = true
        });

        id.Should().Be(5);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    // ── UpdateAsync / DeleteAsync ───────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_WhenRuleDoesNotExist_ShouldThrowNotFoundException()
    {
        _waiverRuleRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((WaiverRule?)null);

        var act = () => _service.UpdateAsync(999, new WaiverRuleUpdateRequest { Name = "X" });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteWithoutUsageGuard()
    {
        var rule = new WaiverRule { WaiverRuleId = 1, Name = "Rule A" };
        _waiverRuleRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(rule);

        await _service.DeleteAsync(1);

        _waiverRuleRepositoryMock.Verify(r => r.Delete(rule), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    // ── TryMatchAsync ───────────────────────────────────────────────────────

    private static WaiverRule Rule(long id, int priority, WaiverCandidateTypeEnum? candidateType = null, long? specialCategoryId = null, long? referralSourceId = null, bool isActive = true) =>
        new()
        {
            WaiverRuleId = id,
            Name = $"Rule {id}",
            Priority = priority,
            IsActive = isActive,
            CandidateTypeFilter = candidateType,
            SpecialCategoryId = specialCategoryId,
            ReferralSourceId = referralSourceId
        };

    [Fact]
    public async Task TryMatchAsync_WithMatchingCandidateTypeRule_ShouldReturnRule()
    {
        _waiverRuleRepositoryMock.Setup(r => r.GetActiveOrderedByPriorityAsync())
            .ReturnsAsync(new List<WaiverRule> { Rule(1, priority: 1, candidateType: WaiverCandidateTypeEnum.Internal) });

        var result = await _service.TryMatchAsync(candidateIsInternal: true, specialCategoryId: null, referralSourceId: null);

        result.Should().NotBeNull();
        result!.WaiverRuleId.Should().Be(1);
    }

    [Fact]
    public async Task TryMatchAsync_WithNonMatchingCandidateType_ShouldReturnNull()
    {
        _waiverRuleRepositoryMock.Setup(r => r.GetActiveOrderedByPriorityAsync())
            .ReturnsAsync(new List<WaiverRule> { Rule(1, priority: 1, candidateType: WaiverCandidateTypeEnum.Internal) });

        var result = await _service.TryMatchAsync(candidateIsInternal: false, specialCategoryId: null, referralSourceId: null);

        result.Should().BeNull();
    }

    [Fact]
    public async Task TryMatchAsync_WithWildcardRule_ShouldMatchAnyCandidate()
    {
        _waiverRuleRepositoryMock.Setup(r => r.GetActiveOrderedByPriorityAsync())
            .ReturnsAsync(new List<WaiverRule> { Rule(1, priority: 1) });

        var result = await _service.TryMatchAsync(candidateIsInternal: false, specialCategoryId: 42, referralSourceId: 7);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task TryMatchAsync_ShouldRespectPriorityOrderingFromRepository()
    {
        // Repository already returns Priority-ascending order; service picks the FIRST match -
        // a lower-priority (evaluated-first) rule that matches should win over a later one.
        _waiverRuleRepositoryMock.Setup(r => r.GetActiveOrderedByPriorityAsync())
            .ReturnsAsync(new List<WaiverRule>
            {
                Rule(1, priority: 1, specialCategoryId: 10),
                Rule(2, priority: 2) // wildcard, would also match
            });

        var result = await _service.TryMatchAsync(candidateIsInternal: false, specialCategoryId: 10, referralSourceId: null);

        result!.WaiverRuleId.Should().Be(1);
    }

    [Fact]
    public async Task TryMatchAsync_WithSpecialCategoryMismatch_ShouldSkipToNextRule()
    {
        _waiverRuleRepositoryMock.Setup(r => r.GetActiveOrderedByPriorityAsync())
            .ReturnsAsync(new List<WaiverRule>
            {
                Rule(1, priority: 1, specialCategoryId: 10),
                Rule(2, priority: 2) // wildcard fallback
            });

        var result = await _service.TryMatchAsync(candidateIsInternal: false, specialCategoryId: 99, referralSourceId: null);

        result!.WaiverRuleId.Should().Be(2);
    }
}
