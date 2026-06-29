using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Wakiliy.Application.Features.Lawyers.Commands.ToggleStatus;
using Wakiliy.Application.Tests.Common;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Errors;

namespace Wakiliy.Application.Tests.Lawyers.Commands.ToggleStatus;

public class ToggleLawyerStatusCommandHandlerTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly ToggleLawyerActiveStatusCommandHandler _handler;

    public ToggleLawyerStatusCommandHandlerTests()
    {
        _userManagerMock = MockUserManagerHelper.CreateMockUserManager();
        _handler = new ToggleLawyerActiveStatusCommandHandler(_userManagerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenLawyerNotFound_ReturnsFailure()
    {
        // Arrange
        var command = new ToggleLawyerActiveStatusCommand("lawyer-1");
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
        var command = new ToggleLawyerActiveStatusCommand("user-1");
        var user = new AppUser { Id = command.Id };
        
        _userManagerMock.Setup(m => m.FindByIdAsync(command.Id)).ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.code.Should().Be("Lawyer.NotFound");
    }

    [Fact]
    public async Task Handle_WithValidCommand_TogglesStatusAndReturnsSuccess()
    {
        // Arrange
        var command = new ToggleLawyerActiveStatusCommand("lawyer-1");
        var lawyer = new Lawyer { Id = command.Id, IsActive = false };
        
        _userManagerMock.Setup(m => m.FindByIdAsync(command.Id)).ReturnsAsync(lawyer);
        _userManagerMock.Setup(m => m.UpdateAsync(lawyer)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        lawyer.IsActive.Should().BeTrue();
        
        _userManagerMock.Verify(m => m.UpdateAsync(lawyer), Times.Once);
    }
}
