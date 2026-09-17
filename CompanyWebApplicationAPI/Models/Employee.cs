using System.ComponentModel.DataAnnotations;

namespace CompanyWebApplicationAPI.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        public int? DepartmentId { get; set; }
        [Required(ErrorMessage = "Salary is required.")]
        public float? Salary { get; set; }
        public Department? Department { get; set; }
        public ICollection<Project>? Projects { get; set; } = new List<Project>();

    }
}
