using System.ComponentModel.DataAnnotations;
namespace EmployeeApp.Models.ViewModels
{
        public class EmployeeViewModel
        {
            public int EmployeeId { get; set; }

            [Required(ErrorMessage = "Employee name is required.")]
            [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
            public string Name { get; set; } = string.Empty;

            [Required(ErrorMessage = "Salary is required.")]
            [Range(0, 1000000, ErrorMessage = "Salary must be a positive value.")]
            public float? Salary { get; set; }

            [Display(Name = "Department")]
            public int? DepartmentId { get; set; }

            public string DepartmentName { get; set; } = "N/A";

            // Collects selected checkbox IDs from Create/Edit forms
            public List<int> SelectedProjectIds { get; set; } = new List<int>();

            // Displays project names on Index/Details pages
            public List<ProjectViewModel> Projects { get; set; } = new List<ProjectViewModel>();
        }
    
}
