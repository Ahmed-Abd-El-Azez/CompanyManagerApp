using System.ComponentModel.DataAnnotations;

namespace EmployeeApp.Models.ViewModels
{
    public class ProjectViewModel
    {
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Project name is required.")]
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Start date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Budget is required.")]
        [Range(0, float.MaxValue, ErrorMessage = "Budget must be a positive value.")]
        [Display(Name = "Budget")]
        public float Budget { get; set; }

        // Helper property for Employee Create/Edit checkboxes in the UI
        public bool IsSelected { get; set; }

        // FIXED: Added { get; set; } so the JSON deserializer can populate it
        public List<EmployeeViewModel> Employees { get; set; } = new List<EmployeeViewModel>();

        public int EmployeesCount => Employees?.Count ?? 0;
    }
}