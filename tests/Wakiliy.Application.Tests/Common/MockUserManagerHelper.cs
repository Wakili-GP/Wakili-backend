using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Application.Tests.Common;

/// <summary>
/// Factory helpers for mocking ASP.NET Core Identity managers.
/// UserManager and SignInManager have complex constructor dependencies;
/// this helper mocks all required services so tests can focus on handler logic.
/// </summary>
public static class MockUserManagerHelper
{
    public static Mock<UserManager<AppUser>> CreateMockUserManager()
    {
        var store = new Mock<IUserStore<AppUser>>();
        var options = new Mock<IOptions<IdentityOptions>>();
        options.Setup(o => o.Value).Returns(new IdentityOptions());

        var passwordHasher = new Mock<IPasswordHasher<AppUser>>();
        var upperInvariantLookupNormalizer = new Mock<ILookupNormalizer>();
        var errors = new IdentityErrorDescriber();
        var services = new Mock<IServiceProvider>();
        var logger = new Mock<ILogger<UserManager<AppUser>>>();

        var userManager = new Mock<UserManager<AppUser>>(
            store.Object,
            options.Object,
            passwordHasher.Object,
            new List<IUserValidator<AppUser>>(),
            new List<IPasswordValidator<AppUser>>(),
            upperInvariantLookupNormalizer.Object,
            errors,
            services.Object,
            logger.Object);

        return userManager;
    }

    public static Mock<SignInManager<AppUser>> CreateMockSignInManager(
        Mock<UserManager<AppUser>> userManagerMock)
    {
        var contextAccessor = new Mock<IHttpContextAccessor>();
        var claimsPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<AppUser>>();
        var options = new Mock<IOptions<IdentityOptions>>();
        options.Setup(o => o.Value).Returns(new IdentityOptions());
        var logger = new Mock<ILogger<SignInManager<AppUser>>>();
        var schemes = new Mock<IAuthenticationSchemeProvider>();
        var confirmation = new Mock<IUserConfirmation<AppUser>>();

        var signInManager = new Mock<SignInManager<AppUser>>(
            userManagerMock.Object,
            contextAccessor.Object,
            claimsPrincipalFactory.Object,
            options.Object,
            logger.Object,
            schemes.Object,
            confirmation.Object);

        return signInManager;
    }
}
