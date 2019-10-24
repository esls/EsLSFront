using EsLSFront.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EsLSFront.Controllers
{
    public class LicensesController : Controller
    {
        private readonly ILogger<LicensesController> _logger;
        private readonly IOptions<ApiOptions> _config;

        public LicensesController(ILogger<LicensesController> logger, IOptions<ApiOptions> config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task<IActionResult> IndexAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string mailFilter = null, [FromQuery] string keyFilter = null)
        {
            // Don't do anything meaningful if not signed in
            if (!Request.Cookies.ContainsKey("EsLSAuth"))
                return RedirectToAction("Required", "Auth");

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["EsLSAuth"]);

            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            if (page > 1)
                queryParams["from"] = ((page - 1) * pageSize).ToString();
            if (pageSize != 10)
                queryParams["count"] = pageSize.ToString();
            if (!string.IsNullOrEmpty(mailFilter))
                queryParams["mailFilter"] = mailFilter;
            if (!string.IsNullOrEmpty(keyFilter))
                queryParams["keyFilter"] = keyFilter;
            string queryString = queryParams.Count > 0 ? "?" + queryParams.ToString() : string.Empty;

            using var response = await httpClient.GetAsync(_config.Value.ApiEndpoint + "licenses" + queryString );
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var apiResponse = JsonConvert.DeserializeObject<LicenseSearchResults>(await response.Content.ReadAsStringAsync());
                var pageModel = new PaginatedLicensesModel()
                {
                    Data = apiResponse.Licenses.ToList(),
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling((double)apiResponse.TotalCount / pageSize),
                    MailFiter = mailFilter,
                    KeyFilter = keyFilter
                };
                return View(pageModel);
            }
            else if (response.StatusCode == HttpStatusCode.Forbidden)
                Response.Cookies.Delete("EsLSAuth");
            return RedirectToAction("Required", "Auth");
        }

        public async Task<IActionResult> DetailsAsync(int id)
        {
            using var httpClient = new HttpClient();
            if (Request.Cookies.ContainsKey("EsLSAuth"))
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["EsLSAuth"]);

            using var response = await httpClient.GetAsync(_config.Value.ApiEndpoint + "licenses/" + id.ToString());
            string apiResponse = await response.Content.ReadAsStringAsync();
            var license = JsonConvert.DeserializeObject<FrontLicenseModel>(apiResponse);
            return View(license);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details([Bind("Id","CreationDate","FullName","Email","LicenseKey","Price")] FrontLicenseModel license)
        {
            if (ModelState.IsValid)
            {
                using var httpClient = new HttpClient();
                if (Request.Cookies.ContainsKey("EsLSAuth"))
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["EsLSAuth"]);

                StringContent content = new StringContent(JsonConvert.SerializeObject(license), Encoding.UTF8, "application/json");

                using var response = await httpClient.PostAsync(_config.Value.ApiEndpoint + "licenses/update", content);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return RedirectToAction("Required", "Auth");
                if (response.StatusCode == HttpStatusCode.OK)
                    license.UpdatedSuccessfully = true;
                else
                    license.UpdatedSuccessfully = false;
            }
            else
                license.UpdatedSuccessfully = false;
            return View(license);
        }
    }
}
