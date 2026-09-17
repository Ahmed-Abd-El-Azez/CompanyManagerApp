using EmployeeApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace EmployeeApp.Data
{
    public class AppIdentityContext : IdentityDbContext<IdentityUser>
    {
        public AppIdentityContext(DbContextOptions options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // must be called
        }
    }
}
