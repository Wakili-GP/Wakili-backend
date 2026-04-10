using Mapster;
using Wakiliy.Application.Features.Reviews.Commands.Create;
using Wakiliy.Application.Features.Reviews.DTOs;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Application.Features.Reviews.Mapping;

public class ReviewMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Review, ReviewResponseDto>()
            .Map(dest => dest.AiAnalysis, src => src.AiAnalysis);

        config.NewConfig<SystemReview, SystemReviewResponseDto>()
            .Map(dest => dest.AiAnalysis, src => src.AiAnalysis);
        config.NewConfig<AiAnalysis, AiReviewDto>();

        config.NewConfig<CreateReviewRequestDto, CreateReviewCommand>()
            .Map(dest => dest.LawyerReview, src => src.LawyerReview)
            .Map(dest => dest.SystemReview, src => src.SystemReview)
            .Map(dest => dest.AppointmentId, src => src.AppointmentId);
    }
}
