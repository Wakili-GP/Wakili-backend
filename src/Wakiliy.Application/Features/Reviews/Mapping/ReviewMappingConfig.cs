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
            .Map(dest => dest.Client, src => new ReviewClientDto 
            {
                FirstName = src.User != null ? src.User.FirstName : string.Empty,
                LastName = src.User != null ? src.User.LastName : string.Empty,
                ProfileImageUrl = src.User != null && src.User.ProfileImage != null ? src.User.ProfileImage.SystemFileUrl : null,
                Bio = src.User != null ? src.User.Bio : null
            })
            .Map(dest => dest.Lawyer, src => new ReviewLawyerDto
            {
                FirstName = src.Lawyer != null ? src.Lawyer.FirstName : string.Empty,
                LastName = src.Lawyer != null ? src.Lawyer.LastName : string.Empty,
                ProfileImageUrl = src.Lawyer != null && src.Lawyer.ProfileImage != null ? src.Lawyer.ProfileImage.SystemFileUrl : null
            });

        config.NewConfig<CreateReviewRequestDto, CreateReviewCommand>()
            .Map(dest => dest.LawyerReview, src => src.LawyerReview)
            .Map(dest => dest.AppointmentId, src => src.AppointmentId);
    }
}
