using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Domain.Errors
{
    public static class AuthErrors
    {
        public static Error InvalidOtp =>
            new("Auth.InvalidOtp", "Invalid verification code", StatusCodes.Status400BadRequest);

        public static Error ExpiredOtp =>
            new("Auth.ExpiredOtp", "Verification code expired", StatusCodes.Status400BadRequest);

        public static Error EmailAlreadyVerified =>
            new("Auth.EmailAlreadyVerified", "Email already verified", StatusCodes.Status400BadRequest);
    }
}
