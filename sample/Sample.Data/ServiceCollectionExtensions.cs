using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.Data
{
    public static class ServiceCollectionExtensions
    {
        private static SqliteConnection _sqliteConnection;

        public static IServiceCollection AddSampleData(this IServiceCollection services)
        {
            _sqliteConnection = new SqliteConnection("Data Source=:memory:");
            _sqliteConnection.Open();

            services.AddDbContext<AnimalContext>(builder =>
            {
                builder.UseSqlite(_sqliteConnection);
            });

            var sp = services.BuildServiceProvider();

            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AnimalContext>();

                db.Database.EnsureCreated();
            }

            return services;
        }
    }
}