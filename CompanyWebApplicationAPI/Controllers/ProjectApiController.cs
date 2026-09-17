using CompanyWebApplicationAPI.Data;
using CompanyWebApplicationAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
// Ensure you include your API's DbContext and Entity namespaces
// using CompanyWebApplicationAPI.Data;
// using CompanyWebApplicationAPI.Models;

namespace CompanyWebApplicationAPI.Controllers
{
    [Route("api/ProjectsApi")]
    [ApiController]
    [Authorize]
    public class ProjectsApiController : ControllerBase
    {
        private readonly CompanyContext _context;

        public ProjectsApiController(CompanyContext context)
        {
            _context = context;
        }

        // GET: api/ProjectsApi
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            return await _context.Projects
                .Include(p=>p.Employees)
                .ToListAsync();
        }

        // GET: api/ProjectsApi/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await _context.Projects.Include(p=>p.Employees).FirstOrDefaultAsync(p => p.ProjectId == id);

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }

        // POST: api/ProjectsApi
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Project>> PostProject(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProject), new { id = project.ProjectId }, project);
        }

        // PUT: api/ProjectsApi/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutProject(int id, Project project)
        {
            if (id != project.ProjectId)
            {
                return BadRequest();
            }

            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
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

        // DELETE: api/ProjectsApi/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
        }
    }
}