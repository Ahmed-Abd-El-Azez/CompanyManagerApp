using System.ComponentModel.DataAnnotations;
namespace EmployeeApp.Models.ViewModels
{
    public class DepartmentViewModel
    {
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Department Name is required.")]
        [StringLength(100, ErrorMessage = "Department Name cannot exceed 100 characters.")]
        [Display(Name = "Department Name")]
        public string DepartmentName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters.")]
        [Display(Name = "Location")]
        public string Location { get; set; } = string.Empty;
        public List<EmployeeViewModel> Employees { get; set; } = new();
        // Optional UI helper: Displays total employees on department list/summary views
        public int EmployeeCount => Employees?.Count ?? 0;
    }
}
