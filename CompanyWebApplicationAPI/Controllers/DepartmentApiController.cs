using CompanyWebApplicationAPI.Data;
using CompanyWebApplicationAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CompanyWebApplicationAPI.Controllers
{
    [Route("api/DepartmentsApi")]
    [ApiController]
    [Authorize]
    public class DepartmentsApiController : ControllerBase
    {
        private readonly CompanyContext _context;

        public DepartmentsApiController(CompanyContext context)
        {
            _context = context;
        }

        // GET: api/DepartmentsApi
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
        {
            return await _context.Departments.Include(d=>d.Employees).ToListAsync();
        }

        // GET: api/DepartmentsApi/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            var department = await _context.Departments.Include(d=>d.Employees).FirstOrDefaultAsync(d => d.DepartmentId == id);

            if (department == null)
            {
                return NotFound();
            }

            return department;
        }

        // POST: api/DepartmentsApi
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Department>> PostDepartment(Department department)
        {
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDepartment), new { id = department.DepartmentId }, department);
        }

        // PUT: api/DepartmentsApi/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutDepartment(int id, Department department)
        {
            if (id != department.DepartmentId)
            {
                return BadRequest();
            }

            _context.Entry(department).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/DepartmentsApi/5
        [HttpDelete("{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.DepartmentId == id);
        }
    }
}