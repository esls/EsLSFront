using EsLSFront.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EsLSFront.Controllers
{
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IOptions<ApiOptions> _config;

        public AuthController(ILogger<AuthController> logger, IOptions<ApiOptions> config)
        {
            _logger = logger;
            _config = config;
        }

        public IActionResult Login()
        {
            if (Request.Cookies.ContainsKey("EsLSAuth"))
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync([Bind("Username,Password")] FrontAuthModel frontAuth)
        {
            if (Request.Cookies.ContainsKey("EsLSAuth"))
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                using var httpClient = new HttpClient();
                StringContent content = new StringContent(JsonConvert.SerializeObject(frontAuth), Encoding.UTF8, "application/json");

                using var response = await httpClient.PostAsync(_config.Value.ApiEndpoint + "auth/login", content);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    Response.Cookies.Append("EsLSAuth", apiResponse, new Microsoft.AspNetCore.Http.CookieOptions() { Expires = DateTime.UtcNow.AddDays(1) });
                    return RedirectToAction("Index", "Licenses");
                }
            }
            return View(frontAuth);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAsync([Bind("Username,Password")] FrontAuthModel frontAuth)
        {
            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    if (Request.Cookies.ContainsKey("EsLSAuth"))
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["EsLSAuth"]);

                    StringContent content = new StringContent(JsonConvert.SerializeObject(frontAuth), Encoding.UTF8, "application/json");

                    using var response = await httpClient.PostAsync(_config.Value.ApiEndpoint + "auth/register", content);
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                        return RedirectToAction("Required");
                    string apiResponse = await response.Content.ReadAsStringAsync();
                }
                return RedirectToAction("Successful");
            }
            return View(frontAuth);
        }

        public IActionResult Logout()
        {
            if (Request.Cookies.ContainsKey("EsLSAuth"))
                Response.Cookies.Delete("EsLSAuth");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Required()
        {
            return View();
        }

        public IActionResult Successful()
        {
            return View();
        }
    }
}
