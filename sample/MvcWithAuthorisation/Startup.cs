using System;
using System.IO;
using GraphiQl;
using Graphity;
using Graphity.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MvcWithAuthorisation.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sample.Data;

namespace MvcWithAuthorisation
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSampleData();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var builder = new SqliteConnectionStringBuilder(connectionString);
            builder.DataSource = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, builder.DataSource));
            connectionString = builder.ToString();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(connectionString)
            );

            services.AddDefaultIdentity<IdentityUser>()
                .AddDefaultUI()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddGraphity<AnimalContext>(options =>
            {
                options.AddHasRolesAuthorisationPolicy("admin-policy", "admin");
                options.AddHasRolesAuthorisationPolicy("user-policy", "");

                options.ConfigureSet(ctx => ctx.Animals)
                    .SetAuthorisationPolicy("admin-policy");

                options.ConfigureSet(ctx => ctx.Countries)
                    .SetAuthorisationPolicy("user-policy");
            });

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
            IWebHostEnvironment env,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            DataSeeder.Seed(userManager, roleManager);

            app.UseGraphity();

            app.UseGraphiQl("/graphiql", "/api/graph");

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});
        }
    }
}
