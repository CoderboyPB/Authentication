using Basics.AuthorizationRequirements;
using Basics.Controllers;
using Basics.CustomPolicyProvider;
using Basics.Transformer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityExample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("CookieAuth")
                .AddCookie("CookieAuth", config =>
                {
                    config.Cookie.Name = "Grandmas.Cookie";
                    config.LoginPath = "/home/authenticate";
                });

            services.AddAuthorization(config =>
            {
                //var defaultAuthBuilder = new AuthorizationPolicyBuilder();
                //var defaultAuthPolicy = defaultAuthBuilder
                //    .RequireAuthenticatedUser()
                //    .RequireClaim(ClaimTypes.DateOfBirth)
                //    .Build();

                //config.DefaultPolicy = defaultAuthPolicy;

                config.AddPolicy("Claim.DoB", policyBuilder =>
                {
                    policyBuilder.RequireCustomClaim(ClaimTypes.DateOfBirth);
                });

                config.AddPolicy("Age18", policy => policy.AddRequirements(new AgeRequireClaim(18)));
                config.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
            });

            services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, SecurityLevelHandler>();
            services.AddScoped<IAuthorizationHandler, CustomRequireClaimHandler>();
            services.AddScoped<IAuthorizationHandler, CookieJarAuthorizationHandler>();
            services.AddScoped<IClaimsTransformation, ClaimsTransformation>();
            services.AddScoped<IAuthorizationHandler, AgeRequireClaimHandler>();

            services.AddControllersWithViews(config =>
            {
                // global authorization filter
                var defaultAuthBuilder = new AuthorizationPolicyBuilder();
                var defaultAuthPolicy = defaultAuthBuilder
                    .RequireAuthenticatedUser()
                    .RequireClaim(ClaimTypes.DateOfBirth)
                    .Build();

                //config.Filters.Add(new AuthorizeFilter(defaultAuthPolicy));
            });

            services.AddRazorPages()
                .AddRazorPagesOptions(config =>
                {
                    config.Conventions.AuthorizePage("/Razor/Secured");
                    config.Conventions.AuthorizePage("/Razor/Policy", "Admin");
                    config.Conventions.AuthorizeFolder("/RazorSecured");
                    config.Conventions.AllowAnonymousToPage("/RazorSecured/Anon");
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }
    }
}
