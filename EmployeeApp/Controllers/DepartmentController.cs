using EmployeeApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;

namespace EmployeeApp.Controllers
{
    [Authorize]
    public class DepartmentController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DepartmentController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient GetApiClient()
        {
            return _httpClientFactory.CreateClient("EmployeeAPI");
        }

        // GET: Department/Index
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var client = GetApiClient();
            var response = await client.GetAsync("DepartmentsApi");

            if (response.IsSuccessStatusCode)
            {
                var departments = await response.Content.ReadFromJsonAsync<List<DepartmentViewModel>>();
                return View(departments ?? new List<DepartmentViewModel>());
            }

            return View(new List<DepartmentViewModel>());
        }

        // GET: Department/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var client = GetApiClient();
            var response = await client.GetAsync($"DepartmentsApi/{id}");

            if (!response.IsSuccessStatusCode) return NotFound();

            var department = await response.Content.ReadFromJsonAsync<DepartmentViewModel>();
            if (department == null) return NotFound();

            return View(department);
        }

        // GET: Department/Create
        [HttpGet]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Create()
        {

            await PopulateEmployeesDropdownAsync();
            return View(new DepartmentViewModel());
        }

        // POST: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(DepartmentViewModel department)
        {
            
            if (ModelState.IsValid)
            {
                var client = GetApiClient();

                var response = await client.PostAsJsonAsync("DepartmentsApi", department);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "Unable to create department via Web API.");
            }

            await PopulateEmployeesDropdownAsync();
            return View(department);
        }

        // GET: Department/Edit/5
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var client = GetApiClient();
            var response = await client.GetAsync($"DepartmentsApi/{id}");

            if (!response.IsSuccessStatusCode) return NotFound();

            var department = await response.Content.ReadFromJsonAsync<DepartmentViewModel>();
            if (department == null) return NotFound();

            await PopulateEmployeesDropdownAsync();
            return View(department);
        }

        // POST: Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, DepartmentViewModel department)
        {

            if (id != department.DepartmentId) return NotFound();

            if (ModelState.IsValid)
            {
                var client = GetApiClient();
                var response = await client.PutAsJsonAsync($"DepartmentsApi/{id}", department);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "Unable to update department via Web API.");
            }

            await PopulateEmployeesDropdownAsync();
            return View(department);
        }

        // GET: Department/Delete/5
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var client = GetApiClient();
            var response = await client.GetAsync($"DepartmentsApi/{id}");

            if (!response.IsSuccessStatusCode) return NotFound();

            var department = await response.Content.ReadFromJsonAsync<DepartmentViewModel>();
            return View(department);
        }

        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmation(int id)
        {
            var client = GetApiClient();
            var response = await client.DeleteAsync($"DepartmentsApi/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Unable to delete department via Web API.");

            // Reload model to redisplay delete view with error
            var getResponse = await client.GetAsync($"DepartmentsApi/{id}");
            if (getResponse.IsSuccessStatusCode)
            {
                var department = await getResponse.Content.ReadFromJsonAsync<DepartmentViewModel>();
                return View(department);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Department/TestAjax
        [HttpGet]
        public IActionResult TestAjax()
        {
            return View();
        }

        // 2. Proxy endpoint called by AJAX fetch()
        [HttpGet]
        public async Task<IActionResult> GetEmployeesAjax()
        {
            // 2. Use the client name that has JwtTokenHandler attached ("EmployeeAPI")
            var client = _httpClientFactory.CreateClient("EmployeeAPI");

            // 3. Use relative path since BaseAddress is configured in Program.cs
            var response = await client.GetAsync("EmployeesApi");

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Unable to fetch data from API");
            }

            var employees = await response.Content.ReadFromJsonAsync<List<EmployeeViewModel>>();
            return Json(employees);
        }
        private async Task PopulateEmployeesDropdownAsync()
        {
            
            var client = GetApiClient();
            var response = await client.GetAsync("EmployeesApi");

            if (response.IsSuccessStatusCode)
            {
                var employees = await response.Content.ReadFromJsonAsync<List<EmployeeViewModel>>();
                ViewBag.Employees = new SelectList(employees ?? new List<EmployeeViewModel>(), "EmployeeId", "Name");
            }
            else
            {
                ViewBag.Employees = new SelectList(new List<EmployeeViewModel>(), "EmployeeId", "Name");
            }
        }
    }
}