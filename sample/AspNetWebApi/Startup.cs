using System;
using System.Threading.Tasks;
using AspNetWebApi.Data;
using GraphiQl;
using Graphity;
using Graphity.Authorisation;
using Graphity.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetWebApi
{
    public class Startup
    {
        private SqliteConnection _sqliteConnection;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //We don't need MVC because Graphity runs as middleware
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            ConfigureContext(services);

            //An example (albeit contrived) context configuration
            services.AddGraphity<AnimalContext>(options =>
            {
                options.QueryName("AnimalsQuery"); //Name the query

                options.AddHasRolesAuthorisationPolicy("admin-policy", "admin");
                options.AddFuncAuthorisationPolicy("weekendsOnly", WeekendOnlyPolicy);

                //options.SetGlobalAuthorisationPolicy("admin-policy");

                //Configure the Animals DbSet
                options.ConfigureSet(ctx => ctx.Animals)
                    .TypeName("FaunaType") //Name the type
                    .FieldName("filteredAnimals") //Name the field used for querying
                    .Filter(a => a.Id > 1) //Add a filter to exclude anything with an ID less than 2
                    .DefaultOrderBy(a => a.Name) //Add a default order to sort by name
                    .ConfigureProperty(a => a.Id).Exclude() //Remove Id property from the graph
                    .ConfigureProperty(a => a.LivesInId).Exclude(); //Remove LivesInId property from the graph

                //Configure the Countries DbSet
                options.ConfigureSet(ctx => ctx.Countries)
                    .ConfigureProperty(c => c.Id).Exclude() //Remove Id property from the graph
                    .SetAuthorisationPolicy("admin-policy"); //Restrict this field to admins only

                //Configure the CountryProperties DbSet so it only shows as a child of a country
                options.ConfigureSet(ctx => ctx.CountryProperties)
                    .SetAuthorisationPolicy("weekendsOnly") //We can only call this field on a weekend
                    .ConfigureProperty(cp => cp.CountryId).Exclude(); //Remove CountryId property from the graph
                    
            });
        }

        /// <summary>
        /// A contrived authorisation policy function
        /// </summary>
        /// <returns></returns>
        private static async Task<AuthorisationResult> WeekendOnlyPolicy()
        {
            var isWeekend = DateTime.Today.DayOfWeek == DayOfWeek.Saturday ||
                            DateTime.Today.DayOfWeek == DayOfWeek.Sunday;

            //This isn't really an async method
            await Task.CompletedTask;

            return isWeekend 
                ? AuthorisationResult.Success() 
                : AuthorisationResult.Fail("This query can only be used on a Saturday or Sunday");
        }

        private void ConfigureContext(IServiceCollection services)
        {
            _sqliteConnection = new SqliteConnection("Data Source=:memory:");
            _sqliteConnection.Open();

            services.AddDbContext<AnimalContext>(builder => { builder.UseSqlite(_sqliteConnection); });

            var sp = services.BuildServiceProvider();

            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AnimalContext>();

                db.Database.EnsureCreated();
            }
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

            app.UseGraphiQl("/graphiql", "/api/graph");

            app.UseHttpsRedirection();
        }
    }
}
