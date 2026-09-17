using System.Diagnostics;
using System.Net.Http.Json;
using EmployeeApp.Models;
using EmployeeApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient GetApiClient() => _httpClientFactory.CreateClient("EmployeeAPI");

        // GET: Home/Index
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var client = GetApiClient();

            // 1. Fire both API calls concurrently to optimize loading speed
            var empTask = client.GetAsync("EmployeesApi");
            var deptTask = client.GetAsync("DepartmentsApi");

            await Task.WhenAll(empTask, deptTask);

            var empResponse = await empTask;
            var deptResponse = await deptTask;

            // 2. Read JSON response bodies
            var employees = empResponse.IsSuccessStatusCode
                ? await empResponse.Content.ReadFromJsonAsync<List<EmployeeViewModel>>()
                : new List<EmployeeViewModel>();

            var departments = deptResponse.IsSuccessStatusCode
                ? await deptResponse.Content.ReadFromJsonAsync<List<DepartmentViewModel>>()
                : new List<DepartmentViewModel>();

            // 3. Compute metrics for View
            ViewBag.TotalEmployees = employees?.Count ?? 0;
            ViewBag.TotalDepartments = departments?.Count ?? 0;
            ViewBag.AverageSalary = employees != null && employees.Any()
                ? Math.Round((decimal)employees.Average(e => e.Salary),2)
                : 0;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}