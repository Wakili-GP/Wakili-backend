using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Wakiliy.Application.Features.Admins.Commands.UpdateAdmin;
using Wakiliy.Application.Tests.Common;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;

namespace Wakiliy.Application.Tests.Admins.Commands.UpdateAdmin;

public class UpdateAdminCommandHandlerTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly UpdateAdminCommandHandler _handler;

    public UpdateAdminCommandHandlerTests()
    {
        _userManagerMock = MockUserManagerHelper.CreateMockUserManager();
        _handler = new UpdateAdminCommandHandler(_userManagerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsFailureWithUserNotFoundError()
    {
        // Arrange
        var command = new UpdateAdminCommand { Id = "admin-id" };
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
        var command = new UpdateAdminCommand { Id = "admin-id" };
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
    public async Task Handle_WhenUpdateFails_ReturnsFailureWithUpdateFailedError()
    {
        // Arrange
        var command = new UpdateAdminCommand { Id = "admin-id", FirstName = "NewName" };
        var user = new AppUser { Id = command.Id };
        
        _userManagerMock.Setup(m => m.FindByIdAsync(command.Id)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { DefaultRoles.Admin });
        _userManagerMock.Setup(m => m.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Update failed" }));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.code.Should().Be("Admin.UpdateFailed");
    }

    [Fact]
    public async Task Handle_WithValidCommand_UpdatesAdminAndReturnsSuccess()
    {
        // Arrange
        var command = new UpdateAdminCommand 
        { 
            Id = "admin-id", 
            FirstName = "UpdatedFirstName",
            LastName = "UpdatedLastName",
            Status = "Inactive"
        };
        var user = new AppUser 
        { 
            Id = command.Id,
            FirstName = "OldName",
            LastName = "OldLastName",
            Status = UserStatus.Active,
            Email = "admin@example.com"
        };
        
        _userManagerMock.Setup(m => m.FindByIdAsync(command.Id)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { DefaultRoles.Admin });
        _userManagerMock.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.FirstName.Should().Be(command.FirstName);
        result.Value.LastName.Should().Be(command.LastName);
        result.Value.Status.Should().Be("Inactive");
        
        user.FirstName.Should().Be(command.FirstName);
        user.LastName.Should().Be(command.LastName);
        user.Status.Should().Be(UserStatus.Inactive);

        _userManagerMock.Verify(m => m.UpdateAsync(user), Times.Once);
    }
}
