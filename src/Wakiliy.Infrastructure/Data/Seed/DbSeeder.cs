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

        string[] roleNames = { DefaultRoles.SuperAdmin, DefaultRoles.Admin, DefaultRoles.Client, DefaultRoles.Lawyer };

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
        #endregion

        // Ensure admin, lawyer, client and superadmin users exists
        var superAdminEmail = configuration["SuperAdminUser:Email"];
        var superAdminUserName = configuration["SuperAdminUser:UserName"] ?? superAdminEmail;
        var superAdminPassword = configuration["SuperAdminUser:Password"];

        var adminEmail = configuration["AdminUser:Email"];
        var adminUserName = configuration["AdminUser:UserName"] ?? adminEmail;
        var adminPassword = configuration["AdminUser:Password"];

        var lawyerEmail = configuration["LawyerUser:Email"];
        var lawyerUserName = configuration["LawyerUser:UserName"] ?? lawyerEmail;
        var lawyerPassword = configuration["LawyerUser:Password"];

        var clientEmail = configuration["ClientUser:Email"];
        var clientUserName = configuration["ClientUser:UserName"] ?? clientEmail;
        var clientPassword = configuration["ClientUser:Password"];

        #region Seed SuperAdmin User
        if (!string.IsNullOrEmpty(superAdminEmail) && !string.IsNullOrEmpty(superAdminPassword) && !string.IsNullOrEmpty(superAdminUserName))
        {
            var superAdminUser = await userManager.FindByEmailAsync(superAdminEmail);

            if (superAdminUser == null)
            {
                superAdminUser = new AppUser { 
                    UserName = superAdminUserName,
                    NormalizedUserName = superAdminUserName.ToUpper(),
                    Email = superAdminEmail,
                    EmailConfirmed = true,
                    FirstName="SuperAdmin",
                    LastName="User" 
                 };
                try
                {
                    var result = await userManager.CreateAsync(superAdminUser, superAdminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(superAdminUser, DefaultRoles.SuperAdmin);
                    }
                    else
                    {
                        throw new Exception($"Error seeding SuperAdmin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error seeding SuperAdmin:", ex);
                }
            }
            else
            {
                // Ensure superadmin is always in the "SuperAdmin" role
                if (!await userManager.IsInRoleAsync(superAdminUser, DefaultRoles.SuperAdmin))
                {
                    await userManager.AddToRoleAsync(superAdminUser, DefaultRoles.SuperAdmin);
                }
            }
        }
        #endregion

        #region Seed Admin User
        if (!string.IsNullOrEmpty(adminEmail) && !string.IsNullOrEmpty(adminPassword) && !string.IsNullOrEmpty(adminUserName))
        {
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new AppUser { UserName = adminUserName, NormalizedUserName = adminUserName.ToUpper(), Email = adminEmail, EmailConfirmed = true, FirstName="Admin", LastName="User" };
                try
                {
                    var result = await userManager.CreateAsync(adminUser, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, DefaultRoles.Admin);
                    }
                    else
                    {
                        throw new Exception($"Error seeding Admin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error seeding Admin:", ex);
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
        }
        #endregion

        #region Seed Lawyer User
        if (!string.IsNullOrEmpty(lawyerEmail) && !string.IsNullOrEmpty(lawyerPassword) && !string.IsNullOrEmpty(lawyerUserName))
        {
            var lawyerUser = await userManager.FindByEmailAsync(lawyerEmail);

            if (lawyerUser == null)
            {
                // Use Lawyer entity type here
                lawyerUser = new Lawyer { UserName = lawyerUserName, NormalizedUserName = lawyerUserName.ToUpper(), Email = lawyerEmail ,EmailConfirmed = true,FirstName="Lawyer",LastName="User" };
                try
                {
                    var result = await userManager.CreateAsync(lawyerUser, lawyerPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(lawyerUser, DefaultRoles.Lawyer);
                    }
                    else
                    {
                        throw new Exception($"Error seeding Lawyer: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error seeding Lawyer:", ex);
                }

            }
            else
            {
                // Ensure lawyer is always in the "Lawyer" role
                if (!await userManager.IsInRoleAsync(lawyerUser, DefaultRoles.Lawyer))
                {
                    await userManager.AddToRoleAsync(lawyerUser, DefaultRoles.Lawyer);
                }
            }
        }
        #endregion

        #region Seed Client User
        if (!string.IsNullOrEmpty(clientEmail) && !string.IsNullOrEmpty(clientPassword) && !string.IsNullOrEmpty(clientUserName))
        {
            var clientUser = await userManager.FindByEmailAsync(clientEmail);

            if (clientUser == null)
            {
                // Use Client entity type here
                clientUser = new Client { UserName = clientUserName, NormalizedUserName = clientUserName.ToUpper(), Email = clientEmail, EmailConfirmed = true, FirstName="Client", LastName="User" };
                try
                {
                    var result = await userManager.CreateAsync(clientUser, clientPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(clientUser, DefaultRoles.Client);
                    }
                    else
                    {
                        throw new Exception($"Error seeding Client: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error seeding Client:", ex);
                }
            }
            else
            {
                if (!await userManager.IsInRoleAsync(clientUser, DefaultRoles.Client))
                {
                    await userManager.AddToRoleAsync(clientUser, DefaultRoles.Client);
                }
            }
        }
        #endregion

    }
}
