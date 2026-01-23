using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Commands.Create
{
    public class CreateLaywerCommand : IRequest<Result<LawyerResponse>>
    {
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
    }
}
