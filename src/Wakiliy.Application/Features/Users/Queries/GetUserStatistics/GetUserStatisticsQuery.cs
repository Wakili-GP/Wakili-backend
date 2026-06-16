using MediatR;
using Wakiliy.Application.Features.Users.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Users.Queries.GetUserStatistics;

public class GetUserStatisticsQuery : IRequest<Result<UserStatisticsDto>>;
