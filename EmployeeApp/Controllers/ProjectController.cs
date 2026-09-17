using EmployeeApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;

namespace EmployeeApp.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProjectController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient GetApiClient()
        {
            return _httpClientFactory.CreateClient("EmployeeAPI");
        }

        // GET: Project/Index
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var client = GetApiClient();
            var response = await client.GetAsync("ProjectsApi");

            if (response.IsSuccessStatusCode)
            {
                var projects = await response.Content.ReadFromJsonAsync<List<ProjectViewModel>>();
                return View(projects ?? new List<ProjectViewModel>());
            }

            return View(new List<ProjectViewModel>());
        }

        // GET: Project/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var client = GetApiClient();
            var response = await client.GetAsync($"ProjectsApi/{id}");

            if (!response.IsSuccessStatusCode) return NotFound();

            var project = await response.Content.ReadFromJsonAsync<ProjectViewModel>();
            if (project == null) return NotFound();

            return View(project);
        }

        // GET: Project/Create
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await PopulateEmployeesDropdownAsync();
            return View(new ProjectViewModel());
        }

        // POST: Project/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ProjectViewModel project)
        {
            if (ModelState.IsValid)
            {
                var client = GetApiClient();
                var response = await client.PostAsJsonAsync("ProjectsApi", project);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "Unable to create project via Web API.");
            }

            await PopulateEmployeesDropdownAsync();
            return View(project);
        }

        // GET: Project/Edit/5
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var client = GetApiClient();
            var response = await client.GetAsync($"ProjectsApi/{id}");

            if (!response.IsSuccessStatusCode) return NotFound();

            var project = await response.Content.ReadFromJsonAsync<ProjectViewModel>();
            if (project == null) return NotFound();

            await PopulateEmployeesDropdownAsync();
            return View(project);
        }

        // POST: Project/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, ProjectViewModel project)
        {
            if (id != project.ProjectId) return NotFound();

            if (ModelState.IsValid)
            {
                var client = GetApiClient();
                var response = await client.PutAsJsonAsync($"ProjectsApi/{id}", project);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "Unable to update project via Web API.");
            }

            await PopulateEmployeesDropdownAsync();
            return View(project);
        }

        // GET: Project/Delete/5
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var client = GetApiClient();
            var response = await client.GetAsync($"ProjectsApi/{id}");

            if (!response.IsSuccessStatusCode) return NotFound();

            var project = await response.Content.ReadFromJsonAsync<ProjectViewModel>();
            return View(project);
        }

        // POST: Project/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmation(int id)
        {
            var client = GetApiClient();
            var response = await client.DeleteAsync($"ProjectsApi/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Unable to delete project via Web API.");

            // Reload project data to redisplay confirmation view with error
            var getResponse = await client.GetAsync($"ProjectsApi/{id}");
            if (getResponse.IsSuccessStatusCode)
            {
                var project = await getResponse.Content.ReadFromJsonAsync<ProjectViewModel>();
                return View(project);
            }

            return RedirectToAction(nameof(Index));
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