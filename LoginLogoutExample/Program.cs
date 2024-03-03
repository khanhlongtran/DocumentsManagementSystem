using LoginLogoutExample.Filter;
using LoginLogoutExample.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace LoginLogoutExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AuthenticationFilter());
            });
            // IHttpContextAccessor: IHttpContextAccessor is an interface in ASP.NET Core that provides access to the current HttpContext. The HttpContext contains information about the current request, response, and other contextual information.

            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddSession();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });
            // Đã add ở trong class context này rồi nên ngoài này không cần sử dụng options
            builder.Services.AddDbContext<LoginLogoutExampleContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DB"))
);
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            //app.UseCookiePolicy();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            app.Run();
        }
    }
}