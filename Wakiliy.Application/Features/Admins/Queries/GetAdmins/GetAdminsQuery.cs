using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wakiliy.Application.Features.Admins.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Admins.Queries.GetAdmins
{
    public class GetAdminsQuery : IRequest<Result<IEnumerable<AdminDto>>>
    {
    }
}
