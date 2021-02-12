using System;
using System.Threading.Tasks;
using Graphity.Authorisation;
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
                options
                    .QueryName("RandomName")
                    .SetDefaultTake(5)
                    .AddHasScopeAuthorisationPolicy("scopeCheck", "scope1")
                    .AddFuncAuthorisationPolicy("fail-on-first-call", FailOnFirstCall);

                options.ConfigureSet(ctx => ctx.Animals)
                    .TypeName("FaunaType")
                    .FieldName("filteredAnimals")
                    .Filter(a => a.Id > 1)
                    .DefaultOrderBy(a => a.Name)
                    .ConfigureProperty(a => a.Id).Exclude()
                    .ConfigureProperty(a => a.LivesInId).Exclude();

                options.ConfigureSet(ctx => ctx.Countries)
                    .SetAuthorisationPolicy("fail-on-first-call");

                options.ConfigureSet(ctx => ctx.CountryProperties)
                    .SetAuthorisationPolicy("scopeCheck");
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

        private static bool _hasBeenCalled;
        private static async Task<AuthorisationResult> FailOnFirstCall()
        {
            await Task.CompletedTask;

            var result = _hasBeenCalled
                ? AuthorisationResult.Success()
                : AuthorisationResult.Fail("This query will fail the first time you call it, try again.");

            _hasBeenCalled = true;

            return result;
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseGraphity();
        }
    }
}