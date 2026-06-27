using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Wakiliy.Domain.Entities;
using Wakiliy.Infrastructure.Data;

namespace Wakiliy.API.IntegrationTests.Infrastructure.Helpers;

public static class TestHelper
{
    public static ByteArrayContent CreateDummyFileContent(string content, string contentType)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        var fileContent = new ByteArrayContent(bytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        return fileContent;
    }

    public static async Task SetupLawyerForReviewAsync(ApplicationDbContext dbContext, string email)
    {
        var user = await dbContext.Users.OfType<Lawyer>().FirstOrDefaultAsync(u => u.Email == email);
        if (user != null)
        {
            user.CurrentOnboardingStep = -1; // Submitted for review
            user.VerificationStatus = Wakiliy.Domain.Enums.VerificationStatus.Pending;
            await dbContext.SaveChangesAsync();
        }
    }
}
