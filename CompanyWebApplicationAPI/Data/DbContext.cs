using CompanyWebApplicationAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyWebApplicationAPI.Data
{
    public class CompanyContext : DbContext
    {
        public CompanyContext(DbContextOptions<CompanyContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Project> Projects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Configure One-to-Many Relationship (Employee - Department)
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            // 2. Configure Many-to-Many Relationship & Join Table Structure First
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Projects)
                .WithMany(p => p.Employees)
                .UsingEntity<Dictionary<string, object>>(
                    "EmployeeProject",
                    j => j.HasOne<Project>()
                          .WithMany()
                          .HasForeignKey("ProjectsProjectId")
                          .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<Employee>()
                          .WithMany()
                          .HasForeignKey("EmployeesEmployeeId")
                          .OnDelete(DeleteBehavior.Cascade)
                );

            // 3. Seed Departments
            modelBuilder.Entity<Department>().HasData(
                new Department { DepartmentId = 1, DepartmentName = "Backend", Location = "Cairo" },
                new Department { DepartmentId = 2, DepartmentName = "HR", Location = "Damietta" },
                new Department { DepartmentId = 3, DepartmentName = "PR", Location = "Mansoura" },
                new Department { DepartmentId = 4, DepartmentName = "Frontend", Location = "Alexandria" }
            );

            // 4. Seed Projects
            modelBuilder.Entity<Project>().HasData(
                new Project { ProjectId = 1, ProjectName = "E-Commerce System", StartDate = new DateTime(2026, 1, 1) },
                new Project { ProjectId = 2, ProjectName = "Mobile App", StartDate = new DateTime(2026, 3, 1) },
                new Project { ProjectId = 3, ProjectName = "Calendar App", StartDate = new DateTime(2026, 4, 11) }
            );

            // 5. Seed Employees
            modelBuilder.Entity<Employee>().HasData(
                new Employee { EmployeeId = 1, Name = "Ahmed", Salary = 12000, DepartmentId = 1 },
                new Employee { EmployeeId = 2, Name = "Sara", Salary = 9500, DepartmentId = 2 },
                new Employee { EmployeeId = 3, Name = "Mohamed", Salary = 22000, DepartmentId = 1 },
                new Employee { EmployeeId = 4, Name = "Mona", Salary = 11500, DepartmentId = 2 }
            );

            // 6. Seed Join Table (After relationship configuration)
            modelBuilder.Entity("EmployeeProject").HasData(
                new { EmployeesEmployeeId = 1, ProjectsProjectId = 1 },
                new { EmployeesEmployeeId = 1, ProjectsProjectId = 2 },
                new { EmployeesEmployeeId = 2, ProjectsProjectId = 2 }
            );
        }
    }
}