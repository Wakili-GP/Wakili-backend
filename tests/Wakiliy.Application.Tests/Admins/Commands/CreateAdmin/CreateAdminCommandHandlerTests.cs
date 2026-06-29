//using FluentAssertions;
//using Microsoft.AspNetCore.Identity;
//using Moq;
//using Wakiliy.Application.Features.Admins.Commands.CreateAdmin;
//using Wakiliy.Application.Tests.Common;
//using Wakiliy.Domain.Constants;
//using Wakiliy.Domain.Entities;
//using Wakiliy.Domain.Errors;

//namespace Wakiliy.Application.Tests.Admins.Commands.CreateAdmin;

//public class CreateAdminCommandHandlerTests
//{
//    private readonly Mock<UserManager<AppUser>> _userManagerMock;
//    private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
//    private readonly CreateAdminCommandHandler _handler;

//    public CreateAdminCommandHandlerTests()
//    {
//        _userManagerMock = MockUserManagerHelper.CreateMockUserManager();
        
//        var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
//        _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
//            roleStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

//        _handler = new CreateAdminCommandHandler(_userManagerMock.Object, _roleManagerMock.Object);
//    }

//    [Fact]
//    public async Task Handle_WhenEmailAlreadyExists_ReturnsFailureWithDuplicatedEmailError()
//    {
//        // Arrange
//        var command = new CreateAdminCommand { Email = "admin@example.com" };
//        var users = new List<AppUser> { new AppUser { Email = command.Email } }.BuildAsyncQueryable();
//        _userManagerMock.Setup(m => m.Users).Returns(users);

//        // Act
//        var result = await _handler.Handle(command, CancellationToken.None);

//        // Assert
//        result.IsFailure.Should().BeTrue();
//        result.Error.Should().Be(UserErrors.DuplicatedEmail);
//    }

//    [Fact]
//    public async Task Handle_WhenRoleDoesNotExist_ReturnsFailureWithInvalidRolesError()
//    {
//        // Arrange
//        var command = new CreateAdminCommand { Email = "admin@example.com" };
//        var emptyUsers = new List<AppUser>().BuildAsyncQueryable();
//        _userManagerMock.Setup(m => m.Users).Returns(emptyUsers);

//        _roleManagerMock.Setup(m => m.RoleExistsAsync(DefaultRoles.Admin)).ReturnsAsync(false);

//        // Act
//        var result = await _handler.Handle(command, CancellationToken.None);

//        // Assert
//        result.IsFailure.Should().BeTrue();
//        result.Error.Should().Be(UserErrors.InvalidRoles);
//    }

//    [Fact]
//    public async Task Handle_WhenCreateAsyncFails_ReturnsFailureWithCreateFailedError()
//    {
//        // Arrange
//        var command = new CreateAdminCommand { Email = "admin@example.com", Password = "weak" };
//        var emptyUsers = new List<AppUser>().BuildAsyncQueryable();
//        _userManagerMock.Setup(m => m.Users).Returns(emptyUsers);
//        _roleManagerMock.Setup(m => m.RoleExistsAsync(DefaultRoles.Admin)).ReturnsAsync(true);

//        _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<AppUser>(), command.Password))
//            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Weak password" }));

//        // Act
//        var result = await _handler.Handle(command, CancellationToken.None);

//        // Assert
//        result.IsFailure.Should().BeTrue();
//        result.Error.code.Should().Be("User.CreateFailed");
//    }

//    [Fact]
//    public async Task Handle_WithValidCommand_CreatesAdminAndReturnsSuccess()
//    {
//        // Arrange
//        var command = new CreateAdminCommand 
//        { 
//            Email = "admin@example.com", 
//            Password = "Password123!",
//            FirstName = "Super",
//            LastName = "Admin"
//        };
//        var emptyUsers = new List<AppUser>().BuildAsyncQueryable();
//        _userManagerMock.Setup(m => m.Users).Returns(emptyUsers);
//        _roleManagerMock.Setup(m => m.RoleExistsAsync(DefaultRoles.Admin)).ReturnsAsync(true);

//        _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<AppUser>(), command.Password))
//            .ReturnsAsync(IdentityResult.Success);
//        _userManagerMock.Setup(m => m.AddToRoleAsync(It.IsAny<AppUser>(), DefaultRoles.Admin))
//            .ReturnsAsync(IdentityResult.Success);

//        // Act
//        var result = await _handler.Handle(command, CancellationToken.None);

//        // Assert
//        result.IsSuccess.Should().BeTrue();
//        result.Value.Should().NotBeNull();
//        result.Value.Email.Should().Be(command.Email);
//        result.Value.Role.Should().Be(DefaultRoles.Admin);
        
//        _userManagerMock.Verify(m => m.CreateAsync(It.Is<AppUser>(u => u.Email == command.Email), command.Password), Times.Once);
//        _userManagerMock.Verify(m => m.AddToRoleAsync(It.IsAny<AppUser>(), DefaultRoles.Admin), Times.Once);
//    }
//}
