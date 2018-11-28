using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AspNetWebApi.Data
{
    public class AnimalContext : DbContext
    {
        public AnimalContext(DbContextOptions<AnimalContext> options)
            : base(options)
        { }

        public DbSet<Animal> Animals { get; set; }
        public DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Animal>(builder =>
            {
                builder.ToTable("Animals");

                builder.HasKey(a => a.Id);

                builder.HasData(new List<Animal>
                {
                    new Animal {Id = 1, Name = "Dog"},
                    new Animal {Id = 2, Name = "Cat"},
                    new Animal {Id = 3, Name = "Sloth"}
                });
            });

            modelBuilder.Entity<Country>(builder =>
            {
                builder.ToTable("Countries");

                builder.HasKey(c => c.Id);

                builder.HasData(new List<Country>
                {
                    new Country {Id = 1, Name = "England"},
                    new Country {Id = 2, Name = "France"},
                    new Country {Id = 3, Name = "Germany"}
                });
            });
        }
    }
}