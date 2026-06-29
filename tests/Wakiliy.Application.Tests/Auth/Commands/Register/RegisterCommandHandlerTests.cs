using FluentAssertions;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Wakiliy.Application.Features.Auth.Commands.Register;
using Wakiliy.Application.Tests.Common;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Tests.Auth.Commands.Register;

public class RegisterCommandHandlerTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<RegisterCommandHandler>> _loggerMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<IEmailSender> _emailSenderMock;
    private readonly Mock<IEmailOtpRepository> _emailOtpRepoMock;
    private readonly Mock<IBackgroundJobClient> _backgroundJobClientMock;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _userManagerMock = MockUserManagerHelper.CreateMockUserManager();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<RegisterCommandHandler>>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _emailSenderMock = new Mock<IEmailSender>();
        _emailOtpRepoMock = new Mock<IEmailOtpRepository>();
        _backgroundJobClientMock = new Mock<IBackgroundJobClient>();

        // Default IHttpContextAccessor setup
        var httpContext = new DefaultHttpContext();
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Default EmailOtp repo wired through UnitOfWork
        _unitOfWorkMock.Setup(u => u.EmailOtps).Returns(_emailOtpRepoMock.Object);
        _emailOtpRepoMock.Setup(r => r.AddAsync(It.IsAny<EmailOtp>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Default email sender
        _emailSenderMock
            .Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _handler = new RegisterCommandHandler(
            _userManagerMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object,
            _httpContextAccessorMock.Object,
            _emailSenderMock.Object,
            _backgroundJobClientMock.Object);
    }

    // ─────────────────────────────────────────────
    //  Invalid UserType
    // ─────────────────────────────────────────────

    [Theory]
    [InlineData("Admin")]
    [InlineData("SuperUser")]
    [InlineData("xyz")]
    public async Task Handle_WhenUserTypeIsInvalid_ReturnsFailureWithInvalidUserTypeError(string invalidType)
    {
        // Arrange
        var command = BuildValidClientCommand(c => c.UserType = invalidType);
        SetupEmptyUserStore();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AuthErrors.InvalidUserType);
    }

    // ─────────────────────────────────────────────
    //  Duplicate Email
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ReturnsFailureWithDuplicatedEmailError()
    {
        // Arrange
        var command = BuildValidClientCommand();
        var existingUser = new AppUser { Email = command.Email };
        SetupUserStore(existingUser); // email exists in store

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.DuplicatedEmail);
    }

    // ─────────────────────────────────────────────
    //  CreateAsync fails (e.g. weak password)
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenCreateAsyncFails_ReturnsFailureWithPasswordError()
    {
        // Arrange
        var command = BuildValidClientCommand();
        SetupEmptyUserStore();

        _userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<AppUser>(), command.Password))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError
            {
                Code = "PasswordTooWeak",
                Description = "Password is too weak."
            }));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.code.Should().Be("User.InvalidPassword");
        result.Error.Description.Should().Contain("Password is too weak.");
    }

    // ─────────────────────────────────────────────
    //  Success — Client
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WithValidClientCommand_CreatesClientAndReturnsSuccess()
    {
        // Arrange
        var command = BuildValidClientCommand();
        SetupEmptyUserStore();
        SetupSuccessfulCreate();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _userManagerMock.Verify(m =>
            m.CreateAsync(It.Is<AppUser>(u => u.Email == command.Email), command.Password),
            Times.Once);

        _userManagerMock.Verify(m =>
            m.AddToRoleAsync(It.IsAny<AppUser>(), DefaultRoles.Client),
            Times.Once);

        _emailOtpRepoMock.Verify(r =>
            r.AddAsync(It.Is<EmailOtp>(otp => otp.Email == command.Email)),
            Times.Once);

        _unitOfWorkMock.Verify(u =>
            u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        _backgroundJobClientMock.Verify(x => x.Create(
            It.Is<Hangfire.Common.Job>(job => job.Method.Name == nameof(IEmailSender.SendEmailAsync)),
            It.IsAny<Hangfire.States.EnqueuedState>()),
            Times.Once);
    }

    // ─────────────────────────────────────────────
    //  Success — Lawyer
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WithValidLawyerCommand_CreatesLawyerAndAssignsLawyerRole()
    {
        // Arrange
        var command = BuildValidClientCommand(c => c.UserType = "Lawyer");
        SetupEmptyUserStore();
        SetupSuccessfulCreate();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _userManagerMock.Verify(m =>
            m.AddToRoleAsync(It.IsAny<AppUser>(), DefaultRoles.Lawyer),
            Times.Once);
    }

    // ─────────────────────────────────────────────
    //  OTP stored with hashed code (not plaintext)
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenSuccessful_StoresHashedOtpNotPlaintext()
    {
        // Arrange
        var command = BuildValidClientCommand();
        SetupEmptyUserStore();
        SetupSuccessfulCreate();

        EmailOtp? capturedOtp = null;
        _emailOtpRepoMock
            .Setup(r => r.AddAsync(It.IsAny<EmailOtp>()))
            .Callback<EmailOtp>(otp => capturedOtp = otp)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedOtp.Should().NotBeNull();
        capturedOtp!.Code.Should().NotBeNullOrEmpty();
        // Hashed OTP should be base64 SHA-256, not a plain 6-digit number
        capturedOtp.Code.Should().HaveLength(44); // base64 SHA-256 = 44 chars
        capturedOtp.IsUsed.Should().BeFalse();
        capturedOtp.Purpose.Should().Be(OtpPurpose.EmailVerification);
    }

    // ─────────────────────────────────────────────
    //  Helpers
    // ─────────────────────────────────────────────

    private static RegisterCommand BuildValidClientCommand(Action<RegisterCommand>? configure = null)
    {
        var cmd = new RegisterCommand
        {
            FirstName = "Ahmed",
            LastName = "Ali",
            Email = "ahmed@example.com",
            Password = "Password@123",
            AcceptTerms = true,
            UserType = "Client"
        };
        configure?.Invoke(cmd);
        return cmd;
    }

    /// <summary>Sets up an empty user store so AnyAsync returns false.</summary>
    private void SetupEmptyUserStore()
    {
        var emptyUsers = new List<AppUser>().BuildAsyncQueryable();
        _userManagerMock.Setup(m => m.Users).Returns(emptyUsers);
    }

    /// <summary>Sets up a store containing one user with the given email.</summary>
    private void SetupUserStore(AppUser existingUser)
    {
        var users = new List<AppUser> { existingUser }.BuildAsyncQueryable();
        _userManagerMock.Setup(m => m.Users).Returns(users);
    }

    private void SetupSuccessfulCreate()
    {
        _userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(m => m.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
    }
}
