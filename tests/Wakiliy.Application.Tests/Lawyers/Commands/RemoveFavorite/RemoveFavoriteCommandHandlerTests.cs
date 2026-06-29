using FluentAssertions;
using Moq;
using Wakiliy.Application.Features.Lawyers.Commands.RemoveFavorite;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;

namespace Wakiliy.Application.Tests.Lawyers.Commands.RemoveFavorite;

public class RemoveFavoriteCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IFavoriteLawyerRepository> _favoriteRepoMock;
    private readonly RemoveFavoriteLawyerCommandHandler _handler;

    public RemoveFavoriteCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _favoriteRepoMock = new Mock<IFavoriteLawyerRepository>();

        _unitOfWorkMock.Setup(u => u.FavoriteLawyers).Returns(_favoriteRepoMock.Object);

        _handler = new RemoveFavoriteLawyerCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenNotFavorited_ReturnsFailure()
    {
        // Arrange
        var command = new RemoveFavoriteLawyerCommand { UserId = "user-1", LawyerId = "lawyer-1" };
        _favoriteRepoMock.Setup(r => r.ExistsAsync(command.UserId, command.LawyerId, default)).ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FavoriteErrors.NotInFavorites);
    }

    [Fact]
    public async Task Handle_WithValidCommand_RemovesFavoriteAndReturnsSuccess()
    {
        // Arrange
        var command = new RemoveFavoriteLawyerCommand { UserId = "user-1", LawyerId = "lawyer-1" };
        _favoriteRepoMock.Setup(r => r.ExistsAsync(command.UserId, command.LawyerId, default)).ReturnsAsync(true);
        _favoriteRepoMock.Setup(r => r.RemoveAsync(command.UserId, command.LawyerId, default)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _favoriteRepoMock.Verify(r => r.RemoveAsync(command.UserId, command.LawyerId, default), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }
}
