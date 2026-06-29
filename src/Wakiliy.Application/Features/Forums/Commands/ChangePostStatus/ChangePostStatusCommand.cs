using MediatR;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Forums.Commands.ChangePostStatus;

public record ChangePostStatusCommand(
    string PostId,
    PostStatus Status
) : IRequest<Result<string>>;
