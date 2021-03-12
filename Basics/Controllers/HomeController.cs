using Basics.CustomPolicyProvider;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthorizationService authorizationService;

        public HomeController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }

        [SecurityLevel(5)]
        public IActionResult SecretLevel()
        {
            return View("Secret");
        }

        [SecurityLevel(10)]
        public IActionResult SecretHigherLevel()
        {
            return View("Secret");
        }

        [Authorize(Policy = "Age18")]
        public IActionResult Adult()
        {
            return View("Secret");
        }

        [Rank("Commander")]
        public IActionResult Commander()
        {
            return View("Secret");
        }

        public IActionResult Authenticate()
        {
            var grandmaClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Bob"),
                new Claim(ClaimTypes.Email, "Bob@fmail.com"),
                new Claim("Grandma.Says", "Very nice boi."),
                new Claim(DynamicPolicies.SecurityLevel, "7"),
                new Claim(DynamicPolicies.Rank,"Commander"),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var licenseClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Bob K Foo"),
                new Claim("DrivingLicense", "A+"),
                new Claim(ClaimTypes.DateOfBirth,"18.09.2005")
            };

            var grandmaIdentity = new ClaimsIdentity(grandmaClaims, "Grandma Identity");
            var licenseIdentity = new ClaimsIdentity(licenseClaims, "Government");

            var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity, licenseIdentity });

            HttpContext.SignInAsync(userPrincipal);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DoStuff()
        {
            // we are doing stuff here

            await authorizationService.AuthorizeAsync(User, "Claim.DoB");

            return View("Index");
        }
    }
}
