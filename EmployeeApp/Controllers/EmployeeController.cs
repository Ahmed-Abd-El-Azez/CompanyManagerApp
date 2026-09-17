//using EmployeeApp.Data;
using EmployeeApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net.Http.Headers;

namespace EmployeeApp.Client.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EmployeeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient GetApiClient()
        {
            return _httpClientFactory.CreateClient("EmployeeAPI");
        }

        // GET: Employee/Index
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var client = GetApiClient();
            var response = await client.GetAsync("EmployeesApi");

            if (response.IsSuccessStatusCode)
            {
                var employees = await response.Content.ReadFromJsonAsync<List<EmployeeViewModel>>();
                return View(employees ?? new List<EmployeeViewModel>());
            }

            return View(new List<EmployeeViewModel>());
        }

        // GET: Employee/Create
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            return View(new EmployeeViewModel());
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(EmployeeViewModel employee, int[] selectedProjectIds)
        {
            if (ModelState.IsValid)
            {
                employee.SelectedProjectIds = selectedProjectIds?.ToList() ?? new List<int>();

                var client = GetApiClient();
                var response = await client.PostAsJsonAsync("EmployeesApi", employee);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Unable to create employee via Web API.");
            }

            await PopulateDropdownsAsync(employee.DepartmentId);
            return View(employee);
        }

        // GET: Employee/Edit/5
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var client = GetApiClient();
            var response = await client.GetAsync($"EmployeesApi/{id}");

            if (!response.IsSuccessStatusCode) return NotFound();

            var employee = await response.Content.ReadFromJsonAsync<EmployeeViewModel>();
            if (employee == null) return NotFound();

            await PopulateDropdownsAsync(employee.DepartmentId);
            return View(employee);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, EmployeeViewModel employee, int[] selectedProjectIds)
        {
            if (id != employee.EmployeeId) return NotFound();

            if (ModelState.IsValid)
            {
                employee.SelectedProjectIds = selectedProjectIds?.ToList() ?? new List<int>();

                var client = GetApiClient();
                var response = await client.PutAsJsonAsync($"EmployeesApi/{id}", employee);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Unable to update employee via Web API.");
            }

            await PopulateDropdownsAsync(employee.DepartmentId);
            return View(employee);
        }

        // GET: Employee/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null) return NotFound();

            var client = GetApiClient();
            var response = await client.GetAsync($"EmployeesApi/{id}");

            if (!response.IsSuccessStatusCode) return NotFound();

            var employee = await response.Content.ReadFromJsonAsync<EmployeeViewModel>();
            return View(employee);
        }

        // GET: Employee/Delete/5
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var client = GetApiClient();
            var response = await client.GetAsync($"EmployeesApi/{id}");

            if (!response.IsSuccessStatusCode) return NotFound();

            var employee = await response.Content.ReadFromJsonAsync<EmployeeViewModel>();
            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = GetApiClient();
            var response = await client.DeleteAsync($"EmployeesApi/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Unable to delete employee via Web API.");

            // Reload model to redisplay delete confirmation page with error message
            var getResponse = await client.GetAsync($"EmployeesApi/{id}");
            if (getResponse.IsSuccessStatusCode)
            {
                var employee = await getResponse.Content.ReadFromJsonAsync<EmployeeViewModel>();
                return View(employee);
            }

            return RedirectToAction(nameof(Index));
        }

        // Helper method to populate ViewBag data asynchronously from API
        private async Task PopulateDropdownsAsync(int? selectedDepartmentId = null)
        {
            var client = GetApiClient();

            var deptResponse = await client.GetAsync("DepartmentsApi");
            if (deptResponse.IsSuccessStatusCode)
            {
                var departments = await deptResponse.Content.ReadFromJsonAsync<List<DepartmentViewModel>>();
                ViewBag.Departments = new SelectList(departments ?? new List<DepartmentViewModel>(), "DepartmentId", "DepartmentName", selectedDepartmentId);
            }

            var projResponse = await client.GetAsync("ProjectsApi");
            if (projResponse.IsSuccessStatusCode)
            {
                var projects = await projResponse.Content.ReadFromJsonAsync<List<ProjectViewModel>>();
                ViewBag.AllProjects = projects ?? new List<ProjectViewModel>();
            }
        }
    }
}