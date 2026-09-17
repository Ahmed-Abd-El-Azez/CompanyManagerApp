using Microsoft.AspNetCore.Identity;

namespace EmployeeApp.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleNames = { "Admin", "User" };
            // create roles if they do not exist
            foreach (var roleName in roleNames)
            {
                // Check if the role already exists
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    // Create the role
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            var adminUser =
            new IdentityUser
            {
                UserName = "admin@hotmail.com",
                Email = "admin@hotmail.com",
                EmailConfirmed = true
            };
            string adminPassword = "Admin@123";
            // Check if the users already exist
            var admin = await userManager.FindByEmailAsync(adminUser.Email);
            if (admin == null)
            {
                var createAdmin = await userManager.CreateAsync(adminUser, adminPassword);
                if (createAdmin.Succeeded)
                {
                    // Assign Admin role to the user
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
            var normalUser =
            new IdentityUser
            {
                UserName = "user@hotmail.com",
                Email = "user@hotmail.com",
                EmailConfirmed = true
            };
            string userPassword = "User@123";
            var user = await userManager.FindByEmailAsync(normalUser.Email);
            if (user == null)
            {
                var createUser = await userManager.CreateAsync(normalUser, userPassword);
                if (createUser.Succeeded)
                {
                    // Assign User role to the user
                    await userManager.AddToRoleAsync(normalUser, "User");
                }
            }
        }
    }
}
