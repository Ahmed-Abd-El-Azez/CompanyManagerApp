namespace CompanyWebApplicationAPI.Models.Models.ViewModels
{
    public class CompanyDataViewModel
    {
        public int TotalEmployees { get; set; }
        public int TotalDepartments { get; set; }
        public float? AverageSalary { get; set; }
        public string CompanyName { get; set; } = string.Empty;
    }
}
