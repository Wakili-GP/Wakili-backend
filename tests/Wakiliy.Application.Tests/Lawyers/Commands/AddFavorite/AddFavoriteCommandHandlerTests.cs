using FluentAssertions;
using Moq;
using Wakiliy.Application.Features.Lawyers.Commands.AddFavorite;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;

namespace Wakiliy.Application.Tests.Lawyers.Commands.AddFavorite;

public class AddFavoriteCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILawyerRepository> _lawyerRepoMock;
    private readonly Mock<IFavoriteLawyerRepository> _favoriteRepoMock;
    private readonly AddFavoriteLawyerCommandHandler _handler;

    public AddFavoriteCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _lawyerRepoMock = new Mock<ILawyerRepository>();
        _favoriteRepoMock = new Mock<IFavoriteLawyerRepository>();

        _unitOfWorkMock.Setup(u => u.Lawyers).Returns(_lawyerRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.FavoriteLawyers).Returns(_favoriteRepoMock.Object);

        _handler = new AddFavoriteLawyerCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenLawyerNotFound_ReturnsFailure()
    {
        // Arrange
        var command = new AddFavoriteLawyerCommand()
        {
            LawyerId = "lawyer-1",
            UserId = "user-1"
        };
        _lawyerRepoMock.Setup(r => r.GetByIdAsync(command.LawyerId, default)).ReturnsAsync((Lawyer)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FavoriteErrors.LawyerNotFound);
    }

    [Fact]
    public async Task Handle_WhenAlreadyFavorited_ReturnsFailure()
    {
        // Arrange
        var lawyer = new Lawyer { Id = "lawyer-1" };
        var command = new AddFavoriteLawyerCommand()
        {
            LawyerId = "lawyer-1",
            UserId = "user-1"
        };

        _lawyerRepoMock.Setup(r => r.GetByIdAsync(command.LawyerId, default)).ReturnsAsync(lawyer);
        _favoriteRepoMock.Setup(r => r.ExistsAsync(command.UserId, command.LawyerId, default)).ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FavoriteErrors.AlreadyFavorited);
    }

    [Fact]
    public async Task Handle_WithValidCommand_AddsFavoriteAndReturnsSuccess()
    {
        // Arrange
        var lawyer = new Lawyer { Id = "lawyer-1" };
        var command = new AddFavoriteLawyerCommand()
        {
            LawyerId = "lawyer-1",
            UserId = "user-1"
        };

        _lawyerRepoMock.Setup(r => r.GetByIdAsync(command.LawyerId, default)).ReturnsAsync(lawyer);
        _favoriteRepoMock.Setup(r => r.ExistsAsync(command.UserId, command.LawyerId, default)).ReturnsAsync(false);
        _favoriteRepoMock.Setup(r => r.AddAsync(It.IsAny<FavoriteLawyer>(), default)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        _favoriteRepoMock.Verify(r => r.AddAsync(It.Is<FavoriteLawyer>(f => f.UserId == command.UserId && f.LawyerId == command.LawyerId), default), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }
}
