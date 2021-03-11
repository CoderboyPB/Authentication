using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Basics.AuthorizationRequirements
{
    public class AgeRequireClaim : IAuthorizationRequirement
    {
        public AgeRequireClaim(int Age)
        {
            this.Age = Age;
        }

        public int Age { get; }
    }

    public class AgeRequireClaimHandler : AuthorizationHandler<AgeRequireClaim>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AgeRequireClaim requirement)
        {
            var birthDate = context.User.Claims
                .Where(c => c.Type == ClaimTypes.DateOfBirth)
                .Select(c => c.Value)
                .FirstOrDefault();

            var birthDateParts = birthDate.Split('.');

            var dtDoB = new DateTime(Int32.Parse(birthDateParts[2]), Int32.Parse(birthDateParts[1]), Int32.Parse(birthDateParts[0]));
            var today = DateTime.Now;

            var dateDiffDays = (today - dtDoB).Days;

            int requiredAge = requirement.Age;

            if(dateDiffDays > requiredAge * 365)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
