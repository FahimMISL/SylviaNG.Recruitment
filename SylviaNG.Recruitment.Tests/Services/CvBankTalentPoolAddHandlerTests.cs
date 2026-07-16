using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Features.CvBank.Commands.CvBankTalentPoolAdd;
using SylviaNG.Recruitment.Application.Features.CvBank.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class CvBankTalentPoolAddHandlerTests
{
    private readonly Mock<ICandidateTalentPoolRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CvBankTalentPoolAddHandler _handler;

    public CvBankTalentPoolAddHandlerTests()
    {
        _repositoryMock = new Mock<ICandidateTalentPoolRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CvBankTalentPoolAddHandler(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_SkipsCandidatesAlreadyInPool()
    {
        _repositoryMock.Setup(r => r.GetExistingCandidateProfileIdsAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(new HashSet<long> { 2 });

        var response = await _handler.Handle(
            new CvBankTalentPoolAddCommand(new CvBankTalentPoolAddRequest { CandidateProfileIds = new List<long> { 1, 2, 3 } }),
            CancellationToken.None);

        response.AddedCount.Should().Be(2);
        response.AlreadyInPoolCount.Should().Be(1);
        _repositoryMock.Verify(r => r.AddAsync(It.Is<CandidateTalentPool>(t => t.CandidateProfileId == 1)), Times.Once);
        _repositoryMock.Verify(r => r.AddAsync(It.Is<CandidateTalentPool>(t => t.CandidateProfileId == 3)), Times.Once);
        _repositoryMock.Verify(r => r.AddAsync(It.Is<CandidateTalentPool>(t => t.CandidateProfileId == 2)), Times.Never);
    }

    [Fact]
    public async Task Handle_DeduplicatesRepeatedIdsInRequest()
    {
        _repositoryMock.Setup(r => r.GetExistingCandidateProfileIdsAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(new HashSet<long>());

        var response = await _handler.Handle(
            new CvBankTalentPoolAddCommand(new CvBankTalentPoolAddRequest { CandidateProfileIds = new List<long> { 1, 1, 1 } }),
            CancellationToken.None);

        response.AddedCount.Should().Be(1);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<CandidateTalentPool>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNothingNewToAdd_DoesNotSaveChanges()
    {
        _repositoryMock.Setup(r => r.GetExistingCandidateProfileIdsAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(new HashSet<long> { 1 });

        await _handler.Handle(
            new CvBankTalentPoolAddCommand(new CvBankTalentPoolAddRequest { CandidateProfileIds = new List<long> { 1 } }),
            CancellationToken.None);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
}
