using System;
using Graphity.Middleware;
using Graphity.Tests.Fixtures.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Graphity.Tests.Fixtures
{
    public class ComplexStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            services.AddDbContext<TestContext>(builder =>
            {
                builder.UseInMemoryDatabase("ComplexTestDatabase");
                builder.UseInternalServiceProvider(serviceProvider);
            });

            services.AddGraphity<TestContext>(options =>
            {
                options.ConfigureSet(ctx => ctx.Animals)
                    .TypeName("FaunaType")
                    .FieldName("filteredAnimals")
                    .Filter(a => a.Id > 1)
                    .ConfigureProperty(a => a.Id).Exclude()
                    .ConfigureProperty(a => a.LivesInId).Exclude();

                options.ConfigureSet(ctx => ctx.Countries, setOption: SetOption.IncludeAsChildOnly);
            });

            var sp = services.BuildServiceProvider();

            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<TestContext>();

                // Ensure the database is created.
                db.Database.EnsureCreated();

                // Seed the database with test data.
                ContextFixture.SeedTestData(db);
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseGraphity();
        }
    }
}