namespace CompanyWebApplicationAPI.Models
{
    public class Project
    {
        public int ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public ICollection<Employee>? Employees { get; set; }
        public DateTime StartDate { get; set; }
        public float Budget {  get; set; }

    }
}
