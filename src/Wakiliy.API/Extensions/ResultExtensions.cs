using Microsoft.AspNetCore.Mvc;
using Wakiliy.API.Common;
using Wakiliy.Domain.Responses;

namespace Wakiliy.API.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToSuccess<T>(this Result<T> result)
    {
        if (result.IsFailure)
        {
            throw new InvalidOperationException("Cannot convert failure result to a success response.");
        }

        return new OkObjectResult(new SuccessResponse<T>(result.Value));
    }
    public static IActionResult ToSuccess(this Result result, string message)
    {
        if (result.IsFailure)
        {
            throw new InvalidOperationException("Cannot convert failure result to a success response.");
        }

        var data = new { message };
        return new OkObjectResult(new SuccessResponse<object>(data));
    }

    public static ObjectResult ToProblem(this Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Cannot convert success result to a problem");

        var errorResponse = new
        {
            success = false,
            error = result.Error.Description,
            result.Error.statusCode
        };


        return new ObjectResult(errorResponse);
    }
}