using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Api.AuthRequirement
{
    public class JwtRequirement : IAuthorizationRequirement
    {
        
    }

    public class JwtRequirementHandler : AuthorizationHandler<JwtRequirement>
    {
        private readonly HttpClient httpClient;
        private readonly HttpContext httpContext;

        public JwtRequirementHandler(IHttpClientFactory httpFactory, IHttpContextAccessor contextAccessor)
        {
            httpClient = httpFactory.CreateClient();
            httpContext = contextAccessor.HttpContext;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, JwtRequirement requirement)
        {
            if (httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var accessToken = authHeader.ToString().Split(' ')[1];

                var response = await httpClient.GetAsync($"https://localhost:44362/oauth/validate?access_token={accessToken}");

                if(response.StatusCode == HttpStatusCode.OK)
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}
