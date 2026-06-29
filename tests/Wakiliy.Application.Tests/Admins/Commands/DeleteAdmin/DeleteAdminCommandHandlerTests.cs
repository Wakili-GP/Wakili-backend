using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Wakiliy.Application.Features.Admins.Commands.DeleteAdmin;
using Wakiliy.Application.Tests.Common;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Errors;

namespace Wakiliy.Application.Tests.Admins.Commands.DeleteAdmin;

public class DeleteAdminCommandHandlerTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly DeleteAdminCommandHandler _handler;

    public DeleteAdminCommandHandlerTests()
    {
        _userManagerMock = MockUserManagerHelper.CreateMockUserManager();
        _handler = new DeleteAdminCommandHandler(_userManagerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsFailureWithUserNotFoundError()
    {
        // Arrange
        var command = new DeleteAdminCommand { Id = "admin-id" };
        _userManagerMock.Setup(m => m.FindByIdAsync(command.Id)).ReturnsAsync((AppUser)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.UserNotFound);
    }

    [Fact]
    public async Task Handle_WhenUserIsNotAdmin_ReturnsFailureWithAdminNotFoundError()
    {
        // Arrange
        var command = new DeleteAdminCommand { Id = "admin-id" };
        var user = new AppUser { Id = command.Id };
        
        _userManagerMock.Setup(m => m.FindByIdAsync(command.Id)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { DefaultRoles.Client });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.code.Should().Be("Admin.NotFound");
    }

    [Fact]
    public async Task Handle_WhenDeleteFails_ReturnsFailureWithDeleteFailedError()
    {
        // Arrange
        var command = new DeleteAdminCommand { Id = "admin-id" };
        var user = new AppUser { Id = command.Id };
        
        _userManagerMock.Setup(m => m.FindByIdAsync(command.Id)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { DefaultRoles.Admin });
        _userManagerMock.Setup(m => m.DeleteAsync(user))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Delete failed" }));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.code.Should().Be("Admin.DeleteFailed");
    }

    [Fact]
    public async Task Handle_WithValidCommand_DeletesAdminAndReturnsSuccess()
    {
        // Arrange
        var command = new DeleteAdminCommand { Id = "admin-id" };
        var user = new AppUser { Id = command.Id };
        
        _userManagerMock.Setup(m => m.FindByIdAsync(command.Id)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { DefaultRoles.Admin });
        _userManagerMock.Setup(m => m.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _userManagerMock.Verify(m => m.DeleteAsync(user), Times.Once);
    }
}
