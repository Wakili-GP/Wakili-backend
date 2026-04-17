using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wakiliy.Domain.Common.Models;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Repositories;
using Wakiliy.Infrastructure.Data;

namespace Wakiliy.Infrastructure.Repositories
{
    public class LawyerRepository(ApplicationDbContext dbContext) : ILawyerRepository
    {
        public async Task CreateAsync(Lawyer lawyer, CancellationToken cancellationToken)
        {
            await dbContext.Lawyers.AddAsync(lawyer, cancellationToken);
        }

        public async Task UpdateAsync(Lawyer lawyer, CancellationToken cancellationToken)
        {
            dbContext.Lawyers.Update(lawyer);
        }

        public async Task<Lawyer?> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            return await dbContext.Lawyers
                .Include(l => l.ProfileImage)
                .Include(l => l.Specializations)
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        }

        public async Task<Lawyer?> GetByIdWithQualificationsAndCertificationsAsync(string id, CancellationToken cancellationToken = default)
        {
            return await dbContext.Lawyers
                .Include(l => l.AcademicQualifications).ThenInclude(q => q.Document)
                .Include(l => l.ProfessionalCertifications).ThenInclude(pc => pc.Document)
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        }

        public async Task<Lawyer?> GetByIdWithExperiencesAsync(string id, CancellationToken cancellationToken = default)
        {
            return await dbContext.Lawyers
                .Include(l => l.WorkExperiences)
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        }

        public async Task<Lawyer?> GetLawyerWithVerificationAsync(string id, CancellationToken cancellationToken = default)
        {
            return await dbContext.Lawyers
                .Include(l => l.VerificationDocuments)
                //.ThenInclude(v => v.EducationalCertificates)
                .Include(l => l.VerificationDocuments)
                //.ThenInclude(v => v.ProfessionalCertificates)
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        }

        public async Task<Lawyer?> GetByIdWithAllOnboardingDataAsync(string id)
        {
            return await dbContext.Lawyers
                .Include(l => l.ProfileImage)
                .Include(l => l.Specializations)
                .Include(l => l.AcademicQualifications).ThenInclude(q => q.Document)
                .Include(l => l.ProfessionalCertifications).ThenInclude(pc => pc.Document)
                .Include(l => l.WorkExperiences)
                .Include(l => l.VerificationDocuments!).ThenInclude(vd => vd.File)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public IQueryable<Lawyer> GetVerificationRequestsQueryable()
        {
            return dbContext.Lawyers.AsNoTracking();
        }

        public async Task<List<LawyerVerificationModel>> GetVerificationRequestsAsync(
            VerificationStatus? status,
            CancellationToken cancellationToken = default)
        {
            var query = dbContext.Lawyers
                .Include(l => l.Specializations)
                .Include(l => l.ApprovedBy)
                .Include(l => l.RejectedBy)
                .AsNoTracking();

            if (status.HasValue)
            {
                query = query.Where(l => l.VerificationStatus == status.Value);
            }

            var result = await query
                .Select(l => new LawyerVerificationModel
                {
                    Id = l.Id,
                    FirstName = l.FirstName,
                    LastName = l.LastName,
                    Email = l.Email,
                    Specializations = string.Join(", ", l.Specializations.Select(s => s.Name)),
                    CreatedAt = l.CreatedAt,
                    SubmittedAt = l.LastOnboardingUpdate,
                    ProfileImageUrl = l.ProfileImage != null ? l.ProfileImage.SystemFileUrl : "",
                    Status = l.VerificationStatus.ToString(),
                    ApprovedAt = l.ApprovedAt,
                    ApprovedBy = l.ApprovedBy != null ? $"{l.ApprovedBy.FirstName} {l.ApprovedBy.LastName}" : "",
                    RejectedAt = l.RejectedAt,
                    RejectedBy = l.RejectedBy != null ? $"{l.RejectedBy.FirstName} {l.RejectedBy.LastName}" : ""
                })
                .ToListAsync(cancellationToken);

            return result;
        }

        public async Task<(IEnumerable<LawyerVerificationModel> Requests, int TotalCount, LawyerVerificationStats Stats)> GetVerificationRequestsPagedAsync(
            int page,
            int pageSize,
            string? search,
            VerificationStatus? status,
            bool sortDescending,
            DateFilterType? dateFilter,
            CancellationToken cancellationToken = default)
        {
            var query = dbContext.Lawyers
                .AsNoTracking()
                .Select(l => new
                {
                    l.Id,
                    l.FirstName,
                    l.LastName,
                    l.Email,
                    l.CreatedAt,
                    l.LastOnboardingUpdate,
                    l.VerificationStatus,
                    l.ApprovedAt,
                    l.RejectedAt,
                    ProfileImageUrl = l.ProfileImage != null ? l.ProfileImage.SystemFileUrl : "",
                    ApprovedByName = l.ApprovedBy != null
                        ? l.ApprovedBy.FirstName + " " + l.ApprovedBy.LastName
                        : "",

                                RejectedByName = l.RejectedBy != null
                        ? l.RejectedBy.FirstName + " " + l.RejectedBy.LastName
                        : "",
                    Specializations = l.Specializations.Select(s => s.Name)
                });

            if (status.HasValue)
            {
                query = query.Where(l => l.VerificationStatus == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.ToLower();
                query = query.Where(l =>
                    l.FirstName.ToLower().Contains(lowerSearch) ||
                    l.LastName.ToLower().Contains(lowerSearch) ||
                    l.Email.ToLower().Contains(lowerSearch));
            }



            var now = DateTime.UtcNow;

            if (dateFilter.HasValue)
            {
                query = dateFilter switch
                {
                    DateFilterType.Today =>
                        query.Where(l => l.CreatedAt >= now.Date && l.CreatedAt < now.Date.AddDays(1)),

                    DateFilterType.Last7Days =>
                        query.Where(l => l.CreatedAt >= now.AddDays(-7)),

                    DateFilterType.Last30Days =>
                        query.Where(l => l.CreatedAt >= now.AddDays(-30)),

                    DateFilterType.ThisYear =>
                        query.Where(l => l.CreatedAt >= new DateTime(now.Year, 1, 1)
                                      && l.CreatedAt < new DateTime(now.Year + 1, 1, 1)),

                    _ => query
                };
            }

            var totalCount = await query.CountAsync(cancellationToken);


            query = sortDescending
                ? query.OrderByDescending(l => l.CreatedAt)
                : query.OrderBy(l => l.CreatedAt);



            var result = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new LawyerVerificationModel
                {
                    Id = l.Id,
                    FirstName = l.FirstName,
                    LastName = l.LastName,
                    Email = l.Email,
                    Specializations = string.Join(", ", l.Specializations),
                    CreatedAt = l.CreatedAt,
                    SubmittedAt = l.LastOnboardingUpdate,
                    ProfileImageUrl = l.ProfileImageUrl,
                    Status = l.VerificationStatus.ToString(),
                    ApprovedAt = l.ApprovedAt,
                    ApprovedBy = l.ApprovedByName,
                    RejectedAt = l.RejectedAt,
                    RejectedBy = l.RejectedByName
                })
                .ToListAsync(cancellationToken);

            var stats = await GetVerificationStatsAsync(cancellationToken);

            return (result, totalCount, stats);
        }

        public async Task<LawyerVerificationStats> GetVerificationStatsAsync(CancellationToken cancellationToken = default)
        {
            var stats = await dbContext.Lawyers
                .GroupBy(l => 1)
                .Select(g => new LawyerVerificationStats
                {
                    Total = g.Count(),
                    Pending = g.Count(l => l.VerificationStatus == VerificationStatus.Pending),
                    UnderReview = g.Count(l => l.VerificationStatus == VerificationStatus.UnderReview),
                    Approved = g.Count(l => l.VerificationStatus == VerificationStatus.Approved),
                    Rejected = g.Count(l => l.VerificationStatus == VerificationStatus.Rejected)
                })
                .FirstOrDefaultAsync(cancellationToken);

            return stats ?? new LawyerVerificationStats();
        }
        public async Task DeleteExperiencesByLawyerIdAsync(string lawyerId, CancellationToken cancellationToken = default)
        {
            var experiences = await dbContext.WorkExperiences
                .Where(e => e.LawyerId == lawyerId)
                .ToListAsync(cancellationToken);

            if (experiences.Any())
            {
                dbContext.WorkExperiences.RemoveRange(experiences);
            }
        }
        public IQueryable<Lawyer> GetApprovedLawyersQuery(
            string? searchQuery,
            int? specializationId,
            string? city,
            decimal? minPrice,
            decimal? maxPrice,
            double? minRating,
            List<int>? sessionTypes,
            string? sortBy,
            string? sortOrder)
        {
            IQueryable<Lawyer> query = dbContext.Lawyers
                .Where(l => l.VerificationStatus == VerificationStatus.Approved && l.IsActive)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var search = searchQuery.ToLower();
                query = query.Where(l => l.FirstName.ToLower().Contains(search) || 
                                         l.LastName.ToLower().Contains(search) ||
                                         l.Specializations.Any(s => s.Name.ToLower().Contains(search)));
            }

            if (specializationId.HasValue)
                query = query.Where(l => l.Specializations.Any(s => s.Id == specializationId.Value));

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(l => l.City != null && l.City.ToLower() == city.ToLower());

            if (minPrice.HasValue)
            {
                query = query.Where(l => 
                    (l.PhoneSessionPrice != null && l.PhoneSessionPrice >= minPrice.Value) ||
                    (l.InOfficeSessionPrice != null && l.InOfficeSessionPrice >= minPrice.Value));
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(l => 
                    (l.PhoneSessionPrice != null && l.PhoneSessionPrice <= maxPrice.Value) ||
                    (l.InOfficeSessionPrice != null && l.InOfficeSessionPrice <= maxPrice.Value));
            }

            if (minRating.HasValue)
            {
                query = query.Where(l => l.Reviews.Any() 
                    ? l.Reviews.Where(r => !r.AiAnalysis.IsFlagged).Select(r => r.Rating).Average() >= minRating.Value 
                    : false);
            }

            if (sessionTypes != null && sessionTypes.Any())
            {
                var stringSessionTypes = sessionTypes.Select(st => st == 0 ? "InOffice" : "Phone").ToList();
                query = query.Where(l => l.SessionTypes.Any(st => stringSessionTypes.Contains(st)));
            }

            var isAsc = sortOrder?.ToLower() == "asc";
            
            if (sortBy?.ToLower() == "rating")
            {
                query = isAsc 
                    ? query.OrderBy(l => l.Reviews.Any() ? l.Reviews.Where(r => !r.AiAnalysis.IsFlagged).Select(r => r.Rating).Average() : 0)
                    : query.OrderByDescending(l => l.Reviews.Any() ? l.Reviews.Where(r => !r.AiAnalysis.IsFlagged).Select(r => r.Rating).Average() : 0);
            }
            else if (sortBy?.ToLower() == "price")
            {
                query = isAsc
                    ? query.OrderBy(l => l.PhoneSessionPrice).ThenBy(l => l.InOfficeSessionPrice)
                    : query.OrderByDescending(l => l.PhoneSessionPrice).ThenByDescending(l => l.InOfficeSessionPrice);
            }
            else 
            {
                query = query.OrderBy(l => l.JoinedDate);
            }
            
            return query;
        }

    }
}
