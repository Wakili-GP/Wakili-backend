using Mapster;
using System.Linq;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using System;

namespace Wakiliy.Application.Features.Lawyers.Mapping
{
    public class LawyerMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Lawyer, LawyerResponse>()
                .Map(dest => dest.ProfileImage, src => src.ProfileImage != null ? src.ProfileImage.SystemFileUrl : null)
                .Map(dest=>dest.JoinedDate, src=>src.ApprovedAt)
                .Map(dest => dest.Specializations, src => src.Specializations.Select(s => s.Name).ToList())
                .Map(dest => dest.SessionTypes, src => src.SessionTypes.Select(st => st == "InOffice" ? 0 : 1).ToList())
                .Map(dest => dest.AverageRating, src => src.Reviews.Where(r => !r.AiAnalysis.IsFlagged).Any() ? src.Reviews.Where(r => !r.AiAnalysis.IsFlagged).Average(r => r.Rating) : 0)
                .Map(dest => dest.NumberOfRatings, src => src.Reviews.Count(r => !r.AiAnalysis.IsFlagged));


            config.NewConfig<Lawyer, PublicLawyerProfileResponseDto>()
                .Map(dest => dest.Profile.Id, src => src.Id)
                .Map(dest => dest.Profile.ProfileImage, src => src.ProfileImage != null ? src.ProfileImage.SystemFileUrl : null)
                .Map(dest => dest.Profile.FirstName, src => src.FirstName)
                .Map(dest => dest.Profile.LastName, src => src.LastName)
                .Map(dest => dest.Profile.Bio, src => src.Bio ?? string.Empty)
                .Map(dest => dest.Profile.Summary, src => src.Summary)
                .Map(dest => dest.Profile.City, src => src.City)
                .Map(dest => dest.Profile.Country, src => src.Country)
                .Map(dest => dest.Profile.PracticeAreas, src => src.Specializations.Select(s => s.Name).ToList())
                .Map(dest => dest.Profile.Stats.NumOfAppointmentsCompleted, src => src.Appointments.Count(a => a.Status == AppointmentStatus.Completed))
                .Map(dest => dest.Profile.Stats.YearsOfExperience, src => src.YearsOfExperience ?? 0)
                .Map(dest => dest.Profile.Stats.ArticlesPublishedCount, src => 0) // Placeholder
                .Map(dest => dest.Profile.Stats.ClientRatingAverage, src => src.Reviews.Where(r => !r.AiAnalysis.IsFlagged).Any() ? src.Reviews.Where(r => !r.AiAnalysis.IsFlagged).Average(r => r.Rating) : 0)
                .Map(dest => dest.Profile.Stats.ReviewsTotal, src => src.Reviews.Count(r => !r.AiAnalysis.IsFlagged))
                .Map(dest => dest.Pricing.PhonePrice, src => src.PhoneSessionPrice ?? 0)
                .Map(dest => dest.Pricing.OfficePrice, src => src.InOfficeSessionPrice ?? 0)
                .Map(dest => dest.Pricing.AvailableSessionTypes, src => src.SessionTypes.Select(st => st == "InOffice" ? 0 : 1).ToList())
                .Map(dest => dest.WorkHistory, src => src.WorkExperiences)
                .Map(dest => dest.Education, src => src.AcademicQualifications)
                .Map(dest => dest.Certifications, src => src.ProfessionalCertifications)
                .Map(dest => dest.TopReviews, src => src.Reviews.Where(r => !r.AiAnalysis.IsFlagged).OrderByDescending(r => r.Rating).ThenByDescending(r => r.CreatedAt).Take(2));

            config.NewConfig<Review, LawyerProfileReviewDto>()
                .Map(dest => dest.Client.FirstName, src => src.User != null ? src.User.FirstName : string.Empty)
                .Map(dest => dest.Client.LastName, src => src.User != null ? src.User.LastName : string.Empty)
                .Map(dest => dest.Client.ProfileImageUrl, src => src.User != null && src.User.ProfileImage != null ? src.User.ProfileImage.SystemFileUrl : null);
        }
    }
}
