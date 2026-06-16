using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Wakiliy.Application.Features.Auth.Commands.Login;
using Wakiliy.Application.Interfaces.Services;
using Wakiliy.Application.Tests.Common;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;

namespace Wakiliy.Application.Tests.Auth.Commands.Login;

public class LoginCommandHandlerTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly Mock<SignInManager<AppUser>> _signInManagerMock;
    private readonly Mock<IJwtProvider> _jwtProviderMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userManagerMock = MockUserManagerHelper.CreateMockUserManager();
        _signInManagerMock = MockUserManagerHelper.CreateMockSignInManager(_userManagerMock);
        _jwtProviderMock = new Mock<IJwtProvider>();

        _handler = new LoginCommandHandler(
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _jwtProviderMock.Object);
    }

    // ─────────────────────────────────────────────
    //  User not found
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsInvalidCredentialsFailure()
    {
        // Arrange
        var command = BuildCommand("notfound@example.com", "pass");
        SetupUserStore(new List<AppUser>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidCredentials);
    }

    // ─────────────────────────────────────────────
    //  Inactive user
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenUserIsInactive_ReturnsInactiveUserFailure()
    {
        // Arrange
        var user = CreateUser("user@example.com", status: UserStatus.Inactive);
        var command = BuildCommand(user.Email!);
        SetupUserStore(new List<AppUser> { user });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InactiveUser);
    }

    // ─────────────────────────────────────────────
    //  Admin trying client login endpoint
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenUserIsAdmin_ReturnsUnauthorizedFailure()
    {
        // Arrange
        var user = CreateUser("admin@example.com");
        var command = BuildCommand(user.Email!);
        SetupUserStore(new List<AppUser> { user });

        _userManagerMock
            .Setup(m => m.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { DefaultRoles.Admin });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Unauthorized);
    }

    // ─────────────────────────────────────────────
    //  Locked out
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenUserIsLockedOut_ReturnsLockedUserFailure()
    {
        // Arrange
        var user = CreateUser("locked@example.com");
        var command = BuildCommand(user.Email!, "wrongpass");
        SetupUserStore(new List<AppUser> { user });

        _userManagerMock
            .Setup(m => m.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { DefaultRoles.Client });

        _signInManagerMock
            .Setup(m => m.PasswordSignInAsync(user, command.Password, false, true))
            .ReturnsAsync(SignInResult.LockedOut);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.LockedUser);
    }

    // ─────────────────────────────────────────────
    //  Email not confirmed
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenEmailNotConfirmed_ReturnsEmailNotConfirmedFailure()
    {
        // Arrange
        var user = CreateUser("unconfirmed@example.com");
        var command = BuildCommand(user.Email!);
        SetupUserStore(new List<AppUser> { user });

        _userManagerMock
            .Setup(m => m.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { DefaultRoles.Client });

        _signInManagerMock
            .Setup(m => m.PasswordSignInAsync(user, command.Password, false, true))
            .ReturnsAsync(SignInResult.NotAllowed);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.EmailNotConfirmed);
    }

    // ─────────────────────────────────────────────
    //  Wrong password (generic failure)
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenPasswordIsWrong_ReturnsInvalidCredentialsFailure()
    {
        // Arrange
        var user = CreateUser("user@example.com");
        var command = BuildCommand(user.Email!, "WrongPassword");
        SetupUserStore(new List<AppUser> { user });

        _userManagerMock
            .Setup(m => m.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { DefaultRoles.Client });

        _signInManagerMock
            .Setup(m => m.PasswordSignInAsync(user, command.Password, false, true))
            .ReturnsAsync(SignInResult.Failed);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidCredentials);
    }

    // ─────────────────────────────────────────────
    //  Success — Client
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenCredentialsAreValid_ReturnsSuccessWithLoginResponse()
    {
        // Arrange
        var user = CreateUser("client@example.com");
        var command = BuildCommand(user.Email!);
        SetupUserStore(new List<AppUser> { user });

        _userManagerMock
            .Setup(m => m.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { DefaultRoles.Client });

        _signInManagerMock
            .Setup(m => m.PasswordSignInAsync(user, command.Password, false, true))
            .ReturnsAsync(SignInResult.Success);

        _jwtProviderMock
            .Setup(j => j.GenerateToken(user, It.IsAny<IEnumerable<string>>()))
            .Returns(("jwt-token-string", 3600));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be("jwt-token-string");
        result.Value.ExpiresIn.Should().Be(3600);
        result.Value.User.Should().NotBeNull();
        result.Value.User.UserType.Should().Be(DefaultRoles.Client);

        _jwtProviderMock.Verify(j =>
            j.GenerateToken(user, It.IsAny<IEnumerable<string>>()),
            Times.Once);
    }

    // ─────────────────────────────────────────────
    //  Success — Lawyer: SubmittedAndApproved
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenLawyerIsApproved_ReturnsSubmittedAndApprovedStatus()
    {
        // Arrange
        var lawyer = new Lawyer
        {
            Id = "lawyer-id",
            Email = "lawyer@example.com",
            UserName = "lawyer@example.com",
            Status = UserStatus.Active,
            CurrentOnboardingStep = -1,
            VerificationStatus = VerificationStatus.Approved
        };
        var command = BuildCommand(lawyer.Email);
        SetupUserStore(new List<AppUser> { lawyer });

        _userManagerMock
            .Setup(m => m.GetRolesAsync(lawyer))
            .ReturnsAsync(new List<string> { DefaultRoles.Lawyer });

        _signInManagerMock
            .Setup(m => m.PasswordSignInAsync(lawyer, command.Password, false, true))
            .ReturnsAsync(SignInResult.Success);

        _jwtProviderMock
            .Setup(j => j.GenerateToken(lawyer, It.IsAny<IEnumerable<string>>()))
            .Returns(("token", 3600));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.User.Status.Should().Be(LawyerOnboardingStatus.SubmittedAndApproved.ToString());
    }

    // ─────────────────────────────────────────────
    //  Success — Lawyer: SubmittedAndNotApproved
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenLawyerIsRejected_ReturnsSubmittedAndNotApprovedStatus()
    {
        // Arrange
        var lawyer = new Lawyer
        {
            Id = "lawyer-id",
            Email = "lawyer@example.com",
            UserName = "lawyer@example.com",
            Status = UserStatus.Active,
            CurrentOnboardingStep = -1,
            VerificationStatus = VerificationStatus.Rejected
        };
        var command = BuildCommand(lawyer.Email);
        SetupUserStore(new List<AppUser> { lawyer });

        _userManagerMock
            .Setup(m => m.GetRolesAsync(lawyer))
            .ReturnsAsync(new List<string> { DefaultRoles.Lawyer });

        _signInManagerMock
            .Setup(m => m.PasswordSignInAsync(lawyer, command.Password, false, true))
            .ReturnsAsync(SignInResult.Success);

        _jwtProviderMock
            .Setup(j => j.GenerateToken(lawyer, It.IsAny<IEnumerable<string>>()))
            .Returns(("token", 3600));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.User.Status.Should().Be(LawyerOnboardingStatus.SubmittedAndNotApproved.ToString());
    }

    // ─────────────────────────────────────────────
    //  Success — Lawyer: Unfinished onboarding
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenLawyerOnboardingUnfinished_ReturnsUnfinishedStatus()
    {
        // Arrange
        var lawyer = new Lawyer
        {
            Id = "lawyer-id",
            Email = "lawyer@example.com",
            UserName = "lawyer@example.com",
            Status = UserStatus.Active,
            CurrentOnboardingStep = 2,  // Not yet -1
            VerificationStatus = VerificationStatus.Pending
        };
        var command = BuildCommand(lawyer.Email);
        SetupUserStore(new List<AppUser> { lawyer });

        _userManagerMock
            .Setup(m => m.GetRolesAsync(lawyer))
            .ReturnsAsync(new List<string> { DefaultRoles.Lawyer });

        _signInManagerMock
            .Setup(m => m.PasswordSignInAsync(lawyer, command.Password, false, true))
            .ReturnsAsync(SignInResult.Success);

        _jwtProviderMock
            .Setup(j => j.GenerateToken(lawyer, It.IsAny<IEnumerable<string>>()))
            .Returns(("token", 3600));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.User.Status.Should().Be(LawyerOnboardingStatus.Unfinished.ToString());
    }

    // ─────────────────────────────────────────────
    //  Helpers
    // ─────────────────────────────────────────────

    private static LoginCommand BuildCommand(string email, string password = "Password@123") =>
        new() { Email = email, Password = password };

    private static AppUser CreateUser(string email, UserStatus status = UserStatus.Active) =>
        new()
        {
            Id = Guid.NewGuid().ToString(),
            Email = email,
            UserName = email,
            Status = status
        };

    private void SetupUserStore(List<AppUser> users)
    {
        var mockUsers = users.BuildAsyncQueryable();
        _userManagerMock.Setup(m => m.Users).Returns(mockUsers);
    }
}
