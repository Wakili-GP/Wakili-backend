using FluentAssertions;
using Moq;
using Wakiliy.Application.Features.Lawyers.Commands.Update;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;

namespace Wakiliy.Application.Tests.Lawyers.Commands.Update;

public class UpdateLawyerCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILawyerRepository> _lawyerRepoMock;
    private readonly Mock<ISpecializationRepository> _specializationRepoMock;
    private readonly UpdateLawyerCommandHandler _handler;

    public UpdateLawyerCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _lawyerRepoMock = new Mock<ILawyerRepository>();
        _specializationRepoMock = new Mock<ISpecializationRepository>();

        _unitOfWorkMock.Setup(u => u.Lawyers).Returns(_lawyerRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Specializations).Returns(_specializationRepoMock.Object);

        _handler = new UpdateLawyerCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenLawyerNotFound_ReturnsFailure()
    {
        // Arrange
        var command = new UpdateLawyerCommand { Id = "lawyer-1" };
        _lawyerRepoMock.Setup(r => r.GetByIdAsync(command.Id, default)).ReturnsAsync((Lawyer)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.code.Should().Be("Lawyer.NotFound");
    }

    [Fact]
    public async Task Handle_WhenSpecializationsNotFound_ReturnsFailure()
    {
        // Arrange
        var command = new UpdateLawyerCommand { Id = "lawyer-1", SpecializationIds = new List<int> { 1 } };
        var lawyer = new Lawyer { Id = command.Id };
        
        _lawyerRepoMock.Setup(r => r.GetByIdAsync(command.Id, default)).ReturnsAsync(lawyer);
        _specializationRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<List<int>>(), default)).ReturnsAsync(new List<Specialization>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(SpecializationErrors.InvalidSelection);
    }

    [Fact]
    public async Task Handle_WithValidCommand_UpdatesLawyerAndReturnsSuccess()
    {
        // Arrange
        var specId = 1;
        var command = new UpdateLawyerCommand { Id = "lawyer-1", FullName = "NewName", SpecializationIds = new List<int> { specId } };
        var lawyer = new Lawyer { Id = command.Id, FirstName = "OldName", Specializations = new List<Specialization>() };
        var spec = new Specialization { Id = specId };
        
        _lawyerRepoMock.Setup(r => r.GetByIdAsync(command.Id, default)).ReturnsAsync(lawyer);
        _specializationRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<List<int>>(), default)).ReturnsAsync(new List<Specialization> { spec });
        _lawyerRepoMock.Setup(r => r.UpdateAsync(lawyer, default)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        //result.Value.FirstName.Should().Be("NewName");
        
        lawyer.Specializations.Should().ContainSingle(s => s.Id == specId);
        
        _lawyerRepoMock.Verify(r => r.UpdateAsync(lawyer, default), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }
}
