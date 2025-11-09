using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Infrastructure.Data;
public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


    // DbSets
    public DbSet<Lawyer> Lawyers { get; set; }
    public DbSet<Client> Clients { get; set; }
}
