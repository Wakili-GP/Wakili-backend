using FluentAssertions;
using Microsoft.AspNetCore.Identity.UI.Services;
using Moq;
using Wakiliy.Application.Features.Lawyers.Commands.Verification.ApproveVerification;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;

namespace Wakiliy.Application.Tests.Lawyers.Commands.Verification;

public class ApproveVerificationCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILawyerRepository> _lawyerRepoMock;
    private readonly Mock<IEmailSender> _emailSenderMock;
    private readonly ApproveVerificationCommandHandler _handler;

    public ApproveVerificationCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _lawyerRepoMock = new Mock<ILawyerRepository>();
        _emailSenderMock = new Mock<IEmailSender>();

        _unitOfWorkMock.Setup(u => u.Lawyers).Returns(_lawyerRepoMock.Object);

        _handler = new ApproveVerificationCommandHandler(
            _unitOfWorkMock.Object,
            _emailSenderMock.Object);
    }

    [Fact]
    public async Task Handle_WhenLawyerNotFound_ReturnsFailure()
    {
        // Arrange
        var command = new ApproveVerificationCommand { LawyerId = "lawyer-1", AdminId = "admin-1" };
        _lawyerRepoMock.Setup(r => r.GetByIdAsync(command.LawyerId, default)).ReturnsAsync((Lawyer)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OnboardingErrors.LawyerNotFound);
    }

    [Fact]
    public async Task Handle_WhenAlreadyApproved_ReturnsFailure()
    {
        // Arrange
        var lawyer = new Lawyer { Id = "lawyer-1", VerificationStatus = VerificationStatus.Approved };
        var command = new ApproveVerificationCommand { LawyerId = lawyer.Id, AdminId = "admin-1" };
        
        _lawyerRepoMock.Setup(r => r.GetByIdAsync(command.LawyerId, default)).ReturnsAsync(lawyer);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OnboardingErrors.AlreadyApproved);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ApprovesVerificationAndReturnsSuccess()
    {
        // Arrange
        var lawyer = new Lawyer 
        { 
            Id = "lawyer-1", 
            VerificationStatus = VerificationStatus.Pending,
            FirstName = "Ali",
            LastName = "Ahmad",
            Email = "lawyer@example.com"
        };
        var command = new ApproveVerificationCommand { LawyerId = lawyer.Id, AdminId = "admin-1" };
        
        _lawyerRepoMock.Setup(r => r.GetByIdAsync(command.LawyerId, default)).ReturnsAsync(lawyer);
        _lawyerRepoMock.Setup(r => r.UpdateAsync(lawyer, default)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);
        _emailSenderMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        lawyer.VerificationStatus.Should().Be(VerificationStatus.Approved);
        lawyer.ApprovedById.Should().Be(command.AdminId);
        lawyer.ApprovedAt.Should().NotBeNull();
        lawyer.RejectedAt.Should().BeNull();
        lawyer.RejectedById.Should().BeNull();
        
        _lawyerRepoMock.Verify(r => r.UpdateAsync(lawyer, default), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        _emailSenderMock.Verify(e => e.SendEmailAsync(lawyer.Email, "تمت الموافقة على طلب التحقق الخاص بك", It.IsAny<string>()), Times.Once);
    }
}
