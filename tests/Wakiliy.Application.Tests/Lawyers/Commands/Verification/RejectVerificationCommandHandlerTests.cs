using FluentAssertions;
using Microsoft.AspNetCore.Identity.UI.Services;
using Moq;
using Wakiliy.Application.Features.Lawyers.Commands.Verification.RejectVerification;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;

namespace Wakiliy.Application.Tests.Lawyers.Commands.Verification;

public class RejectVerificationCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILawyerRepository> _lawyerRepoMock;
    private readonly Mock<IEmailSender> _emailSenderMock;
    private readonly RejectVerificationCommandHandler _handler;

    public RejectVerificationCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _lawyerRepoMock = new Mock<ILawyerRepository>();
        _emailSenderMock = new Mock<IEmailSender>();

        _unitOfWorkMock.Setup(u => u.Lawyers).Returns(_lawyerRepoMock.Object);

        _handler = new RejectVerificationCommandHandler(
            _unitOfWorkMock.Object,
            _emailSenderMock.Object);
    }

    [Fact]
    public async Task Handle_WhenLawyerNotFound_ReturnsFailure()
    {
        // Arrange
        var command = new RejectVerificationCommand { LawyerId = "lawyer-1", AdminId = "admin-1" };
        _lawyerRepoMock.Setup(r => r.GetByIdAsync(command.LawyerId, default)).ReturnsAsync((Lawyer)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OnboardingErrors.LawyerNotFound);
    }

    [Fact]
    public async Task Handle_WhenAlreadyRejected_ReturnsFailure()
    {
        // Arrange
        var lawyer = new Lawyer { Id = "lawyer-1", VerificationStatus = VerificationStatus.Rejected };
        var command = new RejectVerificationCommand { LawyerId = lawyer.Id, AdminId = "admin-1" };
        
        _lawyerRepoMock.Setup(r => r.GetByIdAsync(command.LawyerId, default)).ReturnsAsync(lawyer);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OnboardingErrors.AlreadyRejected);
    }

    [Fact]
    public async Task Handle_WithValidCommand_RejectsVerificationAndReturnsSuccess()
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
        var command = new RejectVerificationCommand { LawyerId = lawyer.Id, AdminId = "admin-1", Note = "Incomplete documents" };
        
        _lawyerRepoMock.Setup(r => r.GetByIdAsync(command.LawyerId, default)).ReturnsAsync(lawyer);
        _lawyerRepoMock.Setup(r => r.UpdateAsync(lawyer, default)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);
        _emailSenderMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        lawyer.VerificationStatus.Should().Be(VerificationStatus.Rejected);
        lawyer.RejectedById.Should().Be(command.AdminId);
        lawyer.RejectedAt.Should().NotBeNull();
        lawyer.ApprovedAt.Should().BeNull();
        lawyer.ApprovedById.Should().BeNull();
        
        _lawyerRepoMock.Verify(r => r.UpdateAsync(lawyer, default), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        _emailSenderMock.Verify(e => e.SendEmailAsync(lawyer.Email, "بخصوص طلب التحقق الخاص بك", It.Is<string>(body => body.Contains("Incomplete documents"))), Times.Once);
    }
}
