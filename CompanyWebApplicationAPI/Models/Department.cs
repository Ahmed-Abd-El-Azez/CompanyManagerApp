using Microsoft.AspNetCore.Antiforgery;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.ComponentModel.DataAnnotations;

namespace CompanyWebApplicationAPI.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string DepartmentName { get; set; }

        [Required(ErrorMessage = "Salary is required.")]
        public string? Location { get; set; }
        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}
