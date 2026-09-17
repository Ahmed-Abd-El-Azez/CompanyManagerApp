namespace CompanyWebApplicationAPI.DTOs
{
    public class ProjectDto
    {
        public int ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public int AssignedEmployeeCount { get; set; }
    }

    public class CreateProjectDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Budget { get; set; }
    }
}
