using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CompanyWebApplicationAPI.Data;
using CompanyWebApplicationAPI.Models;
using CompanyWebApplicationAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
namespace CompanyWebApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeesApiController : ControllerBase
    {
        private readonly CompanyContext _context;

        public EmployeesApiController(CompanyContext context)
        {
            _context = context;
        }

        // GET: api/EmployeesApi
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Projects)
                .Select(e => new
                {
                    employeeId = e.EmployeeId,
                    name = e.Name,
                    salary = e.Salary,
                    departmentId = e.DepartmentId,
                    departmentName = e.Department != null ? e.Department.DepartmentName : "N/A",
                    projects = e.Projects.Select(p => new
                    {
                        projectId = p.ProjectId,
                        projectName = p.ProjectName
                    }).ToList()
                })
                .ToListAsync();

            return Ok(employees);
        }

        // GET: api/EmployeesApi/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Projects)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null) return NotFound();

            // Shape the response anonymously to match the Client's ViewModel properties exactly
            var result = new
            {
                employeeId = employee.EmployeeId,
                name = employee.Name,
                salary = employee.Salary,
                departmentId = employee.DepartmentId,
                departmentName = employee.Department?.DepartmentName,
                projects = employee.Projects?.Select(p => new
                {
                    projectId = p.ProjectId,
                    projectName = p.ProjectName,
                }).ToList()
            };

            return Ok(result);
        }

        // POST: api/EmployeesApi
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var employee = new Employee
            {
                Name = dto.Name,
                Salary = dto.Salary,
                DepartmentId = dto.DepartmentId
            };

            // Attach multi-selected projects
            if (dto.SelectedProjectIds != null && dto.SelectedProjectIds.Any())
            {
                var projects = await _context.Projects
                    .Where(p => dto.SelectedProjectIds.Contains(p.ProjectId))
                    .ToListAsync();

                foreach (var project in projects)
                {
                    employee.Projects.Add(project);
                }
            }

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.EmployeeId }, new { id = employee.EmployeeId });
        }

        // PUT: api/EmployeesApi/5
        [HttpPut("{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeUpdateDto dto)
        {
            var employeeToUpdate = await _context.Employees
                .Include(e => e.Projects)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employeeToUpdate == null) return NotFound();

            // Update scalar values
            employeeToUpdate.Name = dto.Name;
            employeeToUpdate.Salary = dto.Salary;
            employeeToUpdate.DepartmentId = dto.DepartmentId;

            // Clear and re-assign projects
            employeeToUpdate.Projects.Clear();

            if (dto.SelectedProjectIds != null && dto.SelectedProjectIds.Any())
            {
                var selectedProjects = await _context.Projects
                    .Where(p => dto.SelectedProjectIds.Contains(p.ProjectId))
                    .ToListAsync();

                foreach (var proj in selectedProjects)
                {
                    employeeToUpdate.Projects.Add(proj);
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/EmployeesApi/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    // Input DTOs to bind incoming JSON payloads from the client cleanly
    
}