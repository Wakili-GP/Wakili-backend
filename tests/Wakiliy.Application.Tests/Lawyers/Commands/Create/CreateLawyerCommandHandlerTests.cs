using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Wakiliy.Application.Features.Lawyers.Commands.Create;
using Wakiliy.Application.Tests.Common;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;

namespace Wakiliy.Application.Tests.Lawyers.Commands.Create;

public class CreateLawyerCommandHandlerTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ISpecializationRepository> _specializationRepoMock;
    private readonly CreateLawyerCommandHandler _handler;

    public CreateLawyerCommandHandlerTests()
    {
        _userManagerMock = MockUserManagerHelper.CreateMockUserManager();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _specializationRepoMock = new Mock<ISpecializationRepository>();

        _unitOfWorkMock.Setup(u => u.Specializations).Returns(_specializationRepoMock.Object);

        _handler = new CreateLawyerCommandHandler(
            _userManagerMock.Object,
            _unitOfWorkMock.Object);
    }

    // ─────────────────────────────────────────────
    //  Empty specialization list
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenNoSpecializationIdsProvided_ReturnsInvalidSelectionFailure()
    {
        // Arrange
        var command = BuildValidCommand(specializationIds: new List<int>());
        _specializationRepoMock
            .Setup(r => r.GetByIdsAsync(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Specialization>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(SpecializationErrors.InvalidSelection);

        _userManagerMock.Verify(m =>
            m.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()),
            Times.Never);
    }

    // ─────────────────────────────────────────────
    //  Some specializations not found
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenSomeSpecializationsNotFound_ReturnsInvalidSelectionFailure()
    {
        // Arrange — request 3, only 2 returned from DB
        var command = BuildValidCommand(specializationIds: new List<int> { 1, 2, 3 });
        _specializationRepoMock
            .Setup(r => r.GetByIdsAsync(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Specialization>
            {
                new() { Id = 1, Name = "Criminal Law" },
                new() { Id = 2, Name = "Family Law" }
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(SpecializationErrors.InvalidSelection);
    }

    // ─────────────────────────────────────────────
    //  Identity CreateAsync fails
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenCreateAsyncFails_ReturnsIdentityErrorFailure()
    {
        // Arrange
        var command = BuildValidCommand();
        SetupSpecializationsFound(command.SpecializationIds);

        _userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<AppUser>(), "Temp@123"))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError
            {
                Code = "DuplicateEmail",
                Description = "Email is already taken."
            }));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.code.Should().Be("DuplicateEmail");
        result.Error.Description.Should().Be("Email is already taken.");
    }

    // ─────────────────────────────────────────────
    //  Success
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WithValidRequest_CreatesLawyerWithSpecializationsAndReturnsSuccess()
    {
        // Arrange
        var command = BuildValidCommand(specializationIds: new List<int> { 1, 2 });
        SetupSpecializationsFound(command.SpecializationIds);

        Lawyer? capturedLawyer = null;
        _userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<AppUser>(), "Temp@123"))
            .Callback<AppUser, string>((u, _) => capturedLawyer = u as Lawyer)
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(m => m.AddToRoleAsync(It.IsAny<AppUser>(), DefaultRoles.Lawyer))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        capturedLawyer.Should().NotBeNull();
        capturedLawyer!.Email.Should().Be(command.Email);
        capturedLawyer.EmailConfirmed.Should().BeTrue();
        capturedLawyer.Specializations.Should().HaveCount(2);
        capturedLawyer.IsActive.Should().BeTrue();

        _userManagerMock.Verify(m =>
            m.AddToRoleAsync(It.IsAny<AppUser>(), DefaultRoles.Lawyer),
            Times.Once);
    }

    // ─────────────────────────────────────────────
    //  Duplicate specialization IDs are deduplicated
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WithDuplicateSpecializationIds_DeduplicatesAndCreatesCorrectly()
    {
        // Arrange — pass [1, 1, 2], expect only [1, 2] to be looked up
        var command = BuildValidCommand(specializationIds: new List<int> { 1, 1, 2 });

        _specializationRepoMock
            .Setup(r => r.GetByIdsAsync(
                It.Is<List<int>>(ids => ids.Distinct().Count() == 2),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Specialization>
            {
                new() { Id = 1, Name = "Criminal Law" },
                new() { Id = 2, Name = "Family Law" }
            });

        _userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<AppUser>(), "Temp@123"))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(m => m.AddToRoleAsync(It.IsAny<AppUser>(), DefaultRoles.Lawyer))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    // ─────────────────────────────────────────────
    //  Helpers
    // ─────────────────────────────────────────────

    private static CreateLaywerCommand BuildValidCommand(
        List<int>? specializationIds = null)
    {
        return new CreateLaywerCommand
        {
            Email = "lawyer@example.com",
            FirstName = "Fatima",
            LastName = "Al-Rashid",
            PhoneNumber = "0501234567",
            LicenseNumber = "LIC-12345",
            YearsOfExperience = 5,
            VerificationStatus = VerificationStatus.Approved,
            SpecializationIds = specializationIds ?? new List<int> { 1, 2 }
        };
    }

    private void SetupSpecializationsFound(List<int> ids)
    {
        var distinctIds = ids.Distinct().ToList();
        var specializations = distinctIds
            .Select(id => new Specialization { Id = id, Name = $"Specialization {id}" })
            .ToList();

        _specializationRepoMock
            .Setup(r => r.GetByIdsAsync(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(specializations);
    }
}
