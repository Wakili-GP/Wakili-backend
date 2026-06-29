using MediatR;
using System.Collections.Generic;
using Wakiliy.Application.Features.Forums.DTOs;
using Wakiliy.Domain.Responses;

using System.Text.Json.Serialization;

namespace Wakiliy.Application.Features.Forums.Commands.CreatePost;

public record CreatePostCommand(
    string Title,
    string Body,
    int SpecializationId,
    List<string> Tags,
    [property: JsonIgnore] string AuthorId = ""
) : IRequest<Result<ForumPostResponse>>;
