using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Wakiliy.Application.Features.Lawyers.Commands.Delete;
using Wakiliy.Application.Tests.Common;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Errors;

namespace Wakiliy.Application.Tests.Lawyers.Commands.Delete;

public class DeleteLawyerCommandHandlerTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly DeleteLawyerCommandHandler _handler;

    public DeleteLawyerCommandHandlerTests()
    {
        _userManagerMock = MockUserManagerHelper.CreateMockUserManager();
        _handler = new DeleteLawyerCommandHandler(_userManagerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenLawyerNotFound_ReturnsFailure()
    {
        // Arrange
        var command = new DeleteLawyerCommand("lawyer-1");
        _userManagerMock.Setup(m => m.FindByIdAsync(command.Id)).ReturnsAsync((AppUser)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.code.Should().Be("Lawyer.NotFound");
    }

    [Fact]
    public async Task Handle_WhenUserIsNotLawyer_ReturnsFailure()
    {
        // Arrange
        var command = new DeleteLawyerCommand("user-1");
        var user = new AppUser { Id = command.Id };
        
        _userManagerMock.Setup(m => m.FindByIdAsync(command.Id)).ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.code.Should().Be("Lawyer.NotFound");
    }

    [Fact]
    public async Task Handle_WhenDeleteFails_ReturnsFailure()
    {
        // Arrange
        var command = new DeleteLawyerCommand("lawyer-1");
        var lawyer = new Lawyer { Id = command.Id };
        
        _userManagerMock.Setup(m => m.FindByIdAsync(command.Id)).ReturnsAsync(lawyer);
        _userManagerMock.Setup(m => m.DeleteAsync(lawyer))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "DeleteFailed", Description = "Error deleting user" }));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.code.Should().Be("DeleteFailed");
    }

    [Fact]
    public async Task Handle_WithValidCommand_DeletesLawyerAndReturnsSuccess()
    {
        // Arrange
        var command = new DeleteLawyerCommand("lawyer-1");
        var lawyer = new Lawyer { Id = command.Id };
        
        _userManagerMock.Setup(m => m.FindByIdAsync(command.Id)).ReturnsAsync(lawyer);
        _userManagerMock.Setup(m => m.DeleteAsync(lawyer)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _userManagerMock.Verify(m => m.DeleteAsync(lawyer), Times.Once);
    }
}
