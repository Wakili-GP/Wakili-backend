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
        }
    }
}
