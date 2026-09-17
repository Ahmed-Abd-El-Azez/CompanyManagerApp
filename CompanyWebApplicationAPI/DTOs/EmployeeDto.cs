namespace CompanyWebApplicationAPI.DTOs
{
    public class EmployeeCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public float Salary { get; set; }
        public int? DepartmentId { get; set; }
        public List<int> SelectedProjectIds { get; set; } = new List<int>();
    }

    public class EmployeeUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public float Salary { get; set; }
        public int? DepartmentId { get; set; }
        public List<int> SelectedProjectIds { get; set; } = new List<int>();
    }
}
