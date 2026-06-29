using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Wakiliy.Application.Features.Forums.DTOs;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Forums.Queries.GetForumStats;

public class GetForumStatsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetForumStatsQuery, Result<ForumStatsResponse>>
{
    public async Task<Result<ForumStatsResponse>> Handle(GetForumStatsQuery request, CancellationToken cancellationToken)
    {
        var totalQuestions = await unitOfWork.Forums.GetTotalQuestionsAsync(cancellationToken);
        var totalAnswers = await unitOfWork.Forums.GetTotalAnswersAsync(cancellationToken);
        var activeUsers = 0; // Ideally count distinct users who posted/commented recently. Placeholder.
        var resolvedQuestions = 0; // Placeholder if you later add an 'IsResolved' flag.

        var stats = new ForumStatsResponse
        {
            TotalQuestions = totalQuestions,
            TotalAnswers = totalAnswers,
            ActiveUsers = activeUsers,
            ResolvedQuestions = resolvedQuestions
        };

        return Result.Success(stats);
    }
}
