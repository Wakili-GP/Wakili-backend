using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wakiliy.API.Extensions;
using Wakiliy.Application.Features.Payrolls.Commands.GeneratePayroll;
using Wakiliy.Application.Features.Payrolls.Commands.MarkPayrollFailed;
using Wakiliy.Application.Features.Payrolls.Commands.MarkPayrollPaid;
using Wakiliy.Application.Features.Payrolls.Queries.GetPayrollDetails;
using Wakiliy.Application.Features.Payrolls.Queries.GetPayrolls;
using Wakiliy.Domain.Constants;

namespace Wakiliy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = DefaultRoles.Admin)]
public class PayrollsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> GeneratePayroll([FromBody] GeneratePayrollCommand command)
    {
        var result = await mediator.Send(command);
        return result.IsSuccess ?  result.ToSuccess() : result.ToProblem();
    }

    [HttpGet]
    public async Task<IActionResult> GetPayrolls([FromQuery] GetPayrollsQuery query)
    {
        var result = await mediator.Send(query);
        return result.IsSuccess ?  result.ToSuccess() : result.ToProblem();
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPayrollDetails(int id)
    {
        var result = await mediator.Send(new GetPayrollDetailsQuery { PayrollId = id });
        return result.IsSuccess ?  result.ToSuccess() : result.ToProblem();
    }

    [HttpPut("{id:int}/mark-paid")]
    public async Task<IActionResult> MarkPaid(int id)
    {
        var result = await mediator.Send(new MarkPayrollPaidCommand { PayrollId = id });
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPut("{id:int}/mark-failed")]
    public async Task<IActionResult> MarkFailed(int id)
    {
        var result = await mediator.Send(new MarkPayrollFailedCommand { PayrollId = id });
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}
