using Microsoft.EntityFrameworkCore;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Repositories;
using Wakiliy.Infrastructure.Data;

namespace Wakiliy.Infrastructure.Repositories;

public class FavoriteLawyerRepository(ApplicationDbContext dbContext) : IFavoriteLawyerRepository
{
    public async Task<bool> ExistsAsync(string userId, string lawyerId, CancellationToken cancellationToken = default)
    {
        return await dbContext.FavoriteLawyers
            .AnyAsync(fl => fl.UserId == userId && fl.LawyerId == lawyerId, cancellationToken);
    }

    public async Task AddAsync(FavoriteLawyer favoriteLawyer, CancellationToken cancellationToken = default)
    {
        await dbContext.FavoriteLawyers.AddAsync(favoriteLawyer, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(string userId, string lawyerId, CancellationToken cancellationToken = default)
    {
        var favorite = await dbContext.FavoriteLawyers
            .FirstOrDefaultAsync(fl => fl.UserId == userId && fl.LawyerId == lawyerId, cancellationToken);

        if (favorite is not null)
        {
            dbContext.FavoriteLawyers.Remove(favorite);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<List<Lawyer>> GetFavoriteLawyersAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.FavoriteLawyers
            .Where(fl => fl.UserId == userId)
            .Include(fl => fl.Lawyer).ThenInclude(l => l.Specializations)
            .Select(fl => fl.Lawyer)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
