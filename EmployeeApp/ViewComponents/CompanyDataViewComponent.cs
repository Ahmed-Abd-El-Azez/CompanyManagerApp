using EmployeeApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace EmployeeApp.ViewComponents
{
    public class CompanyDataViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CompanyDataViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient("EmployeeAPI");

            var model = new CompanyDataViewModel();

            try
            {
                // Corrected routes to match API Controller [Route] attributes
                var employees = await client.GetFromJsonAsync<List<EmployeeViewModel>>("EmployeesApi");
                var departments = await client.GetFromJsonAsync<List<DepartmentViewModel>>("DepartmentsApi");

                if (employees != null)
                {
                    model.TotalEmployees = employees.Count;
                    model.AverageSalary = employees.Any()
                        ? employees.Where(e => e.Salary.HasValue).Average(e => e.Salary!.Value)
                        : 0;
                }

                if (departments != null)
                {
                    model.TotalDepartments = departments.Count;
                }
            }
            catch
            {
                // Fallback graceful handling if API service is unreachable
                model.TotalEmployees = 0;
                model.TotalDepartments = 0;
                model.AverageSalary = 0;
            }

            return View(model);
        }
    }
}