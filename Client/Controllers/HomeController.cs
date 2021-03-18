using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient client;

        public HomeController(IHttpClientFactory factory)
        {
            this.client = factory.CreateClient();
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Secret()
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var serverResponse = await client.GetAsync("https://localhost:44362/secret/index");
            var apiResponse = await client.GetAsync("https://localhost:44395/secret/index");

            return View();
        }
    }
}
