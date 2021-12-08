using AppointmentScheduling.Configuration;
using AppointmentScheduling.DbInitialSeed;
using AppointmentScheduling.Models;
using AppointmentScheduling.Services;
using AppointmentScheduling.Utils;
using ChustaSoft.Tools.SecureConfig;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentScheduling
{
    public class Startup
    {
        private IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }        

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.SetUpSecureConfig<AppSettings>(Configuration, Configuration["SECRET_KEY"]);

            services.AddDbContext<ApplicationDbContext>(options => {
                //options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))

                var connectString = services.SetUpSecureConfig<AppSettings>
                    (Configuration, Configuration["SECRET_KEY"]).ConnectionStrings.Values.First();

                options.UseSqlServer(connectString);
            });

            services.AddControllersWithViews();            

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 3;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

            }).AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddTransient<IAppointmentService, AppointmentService>();
            services.AddTransient<IFinanceService, FinanceService>();

            services.AddScoped<IEmailSender, EmailSender>();

            services.AddScoped<IDbInitializer, DbInitializer>();

            services.Configure<AppSettings>(options =>
            {
                var apiKey = services.SetUpSecureConfig<AppSettings>
                    (Configuration, Configuration["SECRET_KEY"]).apiKey;
                var apiSecret = services.SetUpSecureConfig<AppSettings>
                    (Configuration, Configuration["SECRET_KEY"]).apiSecret;

                options.apiKey = apiKey;
                options.apiSecret = apiSecret;

            });
                

            services.AddDistributedMemoryCache();
            services.AddSession(options => 
            {
                options.IdleTimeout = TimeSpan.FromDays(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;            
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Home/AccessDenied");
            });

            services.AddHttpContextAccessor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            dbInitializer.InitializeDb();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
