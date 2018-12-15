using AspNetWebApi.Data;
using Graphity;
using Graphity.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddDbContext<AnimalContext>(builder =>
            {
                builder.UseSqlServer(Configuration.GetConnectionString("GraphQL2EFCore"));
            });

            services.AddGraphity<AnimalContext>(options =>
            {
                options.QueryName("AnimalsQuery");

                options.ConfigureSet(ctx => ctx.Animals)
                    .TypeName("FaunaType")
                    .FieldName("filteredAnimals")
                    .Filter(a => a.Id > 1)
                    .ConfigureProperty(a => a.Id).Exclude()
                    .ConfigureProperty(a => a.LivesInId).Exclude();

                options.ConfigureSet(ctx => ctx.Countries)
                    .ConfigureProperty(c => c.Id).Exclude();

                options.ConfigureSet(ctx => ctx.CountryProperties, null, SetOption.IncludeAsChildOnly)
                    .ConfigureProperty(cp => cp.CountryId).Exclude();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseGraphity();

            app.UseHttpsRedirection();
            app.UseMvcWithDefaultRoute();
        }
    }
}
