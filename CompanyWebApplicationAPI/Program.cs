using CompanyWebApplicationAPI.Data;
using EmployeeApp.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CompanyWebApplicationAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Database Context
            builder.Services.AddDbContext<CompanyContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddDbContext<AppIdentityContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppIdentityContext>()
            .AddDefaultTokenProviders();

            // 2. Controllers & JSON Options (Consolidated into single call)
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });

            builder.Services.AddHttpClient("N8nClient", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(60); // Allow up to 60s for LLM processing
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            // 3. Documentation Tools
            builder.Services.AddOpenApi();

            // 4. CORS Setup (Consolidated into single call)
            var origins = builder.Configuration.GetSection("AllowedCorsOrigins").Get<string[]>()
              ?? new[] { builder.Configuration["AllowedCorsOrigins"] ?? "http://companymaster.runasp.net" };
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowMvcClient", policy =>
                {
                    policy.WithOrigins(origins)
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // 5. JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!))
                };
            });

            builder.Services.AddAuthorization();
            builder.Services.AddSwaggerGen();
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                // Relative path ensures it resolves inside /api virtual directory
                options.SwaggerEndpoint("v1/swagger.json", "Company Web API v1");
                options.RoutePrefix = "swagger";
            });
            app.UseHttpsRedirection();

            // Enable CORS before Authentication/Authorization
            app.UseCors("AllowMvcClient");

            // CRITICAL FIX: Middleware order is mandatory
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}