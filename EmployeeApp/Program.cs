using EmployeeApp.Data;
using EmployeeApp.Handlers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<AppIdentityContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });

            // 1. Session State Services
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // 2. HttpContext & Handlers
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddTransient<JwtTokenHandler>();

            // 3. Named HttpClient for Web API
            builder.Services.AddHttpClient("EmployeeAPI", client =>
            {
                var baseUrl = builder.Configuration["ApiSettings:BaseUrl"];
                if (string.IsNullOrEmpty(baseUrl))
                {
                    throw new InvalidOperationException("ApiSettings:BaseUrl is missing in configuration.");
                }
                client.BaseAddress = new Uri(baseUrl);
            })
            .AddHttpMessageHandler<JwtTokenHandler>();

            // 4. Identity Configuration
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
            })
            .AddEntityFrameworkStores<AppIdentityContext>()
            .AddDefaultTokenProviders();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // Commented out unless Let's Encrypt SSL is enabled on MonsterASP:
                // app.UseHsts();
            }

            // Commented out to prevent 307 loops on plain HTTP hosting:
            // app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseRouting();

            // Session must remain between UseRouting and UseAuthentication
            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Seed Database on App Startup
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await SeedData.InitializeAsync(services);
            }

            app.Run();
        }
    }
}