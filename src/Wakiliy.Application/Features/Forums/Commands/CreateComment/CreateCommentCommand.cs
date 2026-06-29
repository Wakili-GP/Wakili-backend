using MediatR;
using Wakiliy.Application.Features.Forums.DTOs;
using Wakiliy.Domain.Responses;

using System.Text.Json.Serialization;

namespace Wakiliy.Application.Features.Forums.Commands.CreateComment;

public record CreateCommentCommand(
    [property: JsonIgnore] string PostId = "",
    string Body = "",
    [property: JsonIgnore] string AuthorId = "",
    string? ParentId = null
) : IRequest<Result<ForumCommentResponse>>;
