using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Infrastructure.Data.Seed;
public class DbSeeder
{
    public static async Task MigrateAndSeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var dbContext = services.GetRequiredService<ApplicationDbContext>();

        // Automatically apply pending migrations
        await dbContext.Database.MigrateAsync();

        // Seeding logic for roles and users
        var config = services.GetRequiredService<IConfiguration>();
        await SeedRolesAndAdminAsync(services, config);
    }

    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();



        #region Seed Roles

        string[] roleNames = { DefaultRoles.Admin, DefaultRoles.Client, DefaultRoles.Lawyer };

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
        #endregion

        // Ensure admin and lawyer users exists
        var adminEmail = configuration["AdminUser:Email"];
        var adminUserName = configuration["AdminUser:UserName"];
        var adminPassword = configuration["AdminUser:Password"];
        var lawyerEmail = configuration["LawyerUser:Email"];
        var lawyerUserName = configuration["LawyerUser:UserName"];
        var lawyerPassword = configuration["LawyerUser:Password"];



        #region Seed Admin User

        if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword) || string.IsNullOrEmpty(adminUserName))
        {
            throw new Exception("Admin email , password , username must be set in appsettings.json.");
        }

        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new AppUser { UserName = adminUserName, Email = adminEmail };
            try
            {
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, DefaultRoles.Admin);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("look hereeeeeeeeeeeeeeeeeeeeeee!!!!!!!!!!!!!!!!!!!", ex);
            }

        }
        else
        {
            // Ensure admin is always in the "Admin" role
            if (!await userManager.IsInRoleAsync(adminUser, DefaultRoles.Admin))
            {
                await userManager.AddToRoleAsync(adminUser, DefaultRoles.Admin);
            }
        }

        #endregion

        #region Seed Lawyer User

        if (string.IsNullOrEmpty(lawyerEmail) || string.IsNullOrEmpty(lawyerPassword) || string.IsNullOrEmpty(lawyerUserName))
        {
            throw new Exception("Admin email , password , username must be set in appsettings.json.");
        }

        var lawyerUser = await userManager.FindByEmailAsync(lawyerEmail);

        if (lawyerUser == null)
        {
            lawyerUser = new AppUser { UserName = lawyerUserName, Email = lawyerEmail };
            try
            {
                var result = await userManager.CreateAsync(lawyerUser, lawyerPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(lawyerUser, DefaultRoles.Lawyer);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("look hereeeeeeeeeeeeeeeeeeeeeee!!!!!!!!!!!!!!!!!!!", ex);
            }

        }
        else
        {
            // Ensure lawyer is always in the "Lawyer" role
            if (!await userManager.IsInRoleAsync(adminUser, DefaultRoles.Lawyer))
            {
                await userManager.AddToRoleAsync(adminUser, DefaultRoles.Lawyer);
            }
        }

        #endregion


    }
}
