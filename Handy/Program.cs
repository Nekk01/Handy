using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

namespace Handy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //dotNetEnv
            DotNetEnv.Env.Load();
            
            var builder = WebApplication.CreateBuilder(args);

            // Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IADService, ADService>();

            var connectionString = builder.Configuration.GetConnectionString("SQLConnection");

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddAuthentication("Cookie").AddCookie("Cookie", options =>
            {
                options.LoginPath = "/Login";
                options.AccessDeniedPath = "/Login";
                options.LogoutPath = "/Logout";
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
