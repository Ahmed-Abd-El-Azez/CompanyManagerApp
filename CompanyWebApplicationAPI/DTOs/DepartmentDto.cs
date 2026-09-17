namespace CompanyWebApplicationAPI.DTOs
{
    public class DepartmentDto
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
    }

    public class CreateDepartmentDto
    {
        public string DepartmentName { get; set; } = string.Empty;
    }
}
