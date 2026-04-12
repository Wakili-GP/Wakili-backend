using Mapster;
using MediatR;
using Wakiliy.Application.Features.Users.DTOs;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Users.Queries.GetUserStatistics;

public class GetUserStatisticsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetUserStatisticsQuery, Result<UserStatisticsDto>>
{
    public async Task<Result<UserStatisticsDto>> Handle(GetUserStatisticsQuery request, CancellationToken cancellationToken)
    {
        var stats = await unitOfWork.Users.GetUserStatisticsAsync(cancellationToken);
        return Result.Success(stats.Adapt<UserStatisticsDto>());
    }
}
