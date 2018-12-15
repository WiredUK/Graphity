using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace AspNetWebApi.Data
{
    public class AnimalContext : DbContext
    {
        public AnimalContext(DbContextOptions<AnimalContext> options)
            : base(options)
        { }

        public DbSet<Animal> Animals { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<CountryProperties> CountryProperties { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>(builder =>
            {
                builder.ToTable("Countries");

                builder.HasKey(c => c.Id);

                builder.HasData(new List<Country>
                {
                    new Country {Id = 1, Name = "England"},
                    new Country {Id = 2, Name = "France"},
                    new Country {Id = 3, Name = "Germany"},
                    new Country {Id = 4, Name = "Australia"}
                });
            });

            modelBuilder.Entity<CountryProperties>(builder =>
            {
                builder.ToTable("CountryProperties");

                builder.HasKey(c => c.CountryId);

                builder.HasData(new List<CountryProperties>
                {
                    new CountryProperties {CountryId = 1, Population = 55_619_400, TaxRate = 0.2M, HasMonarchy = true, LastTimeWonWorldCup = new DateTime(1966, 7, 30)},
                    new CountryProperties {CountryId = 2, Population = 67_348_000, TaxRate = 0.2M, HasMonarchy = false, LastTimeWonWorldCup = new DateTime(2018, 7, 15)},
                    new CountryProperties {CountryId = 3, Population = 82_800_000, TaxRate = 0.19M, HasMonarchy = true, LastTimeWonWorldCup = new DateTime(2014, 7, 13)},
                    new CountryProperties {CountryId = 4, Population = 25_155_300, TaxRate = 0.1M, HasMonarchy = true, LastTimeWonWorldCup = null}
                });
            });

            modelBuilder.Entity<Animal>(builder =>
            {
                builder.ToTable("Animals");

                builder.HasKey(a => a.Id);

                builder.HasData(new List<Animal>
                {
                    new Animal {Id = 1, Name = "Dog", NumberOfLegs = 4, AnimalType = AnimalTypeEnum.Mammal, LivesInId = 1},
                    new Animal {Id = 2, Name = "Cat", NumberOfLegs = 4, AnimalType = AnimalTypeEnum.Mammal, LivesInId = 2},
                    new Animal {Id = 3, Name = "Sloth", NumberOfLegs = 2, AnimalType = AnimalTypeEnum.Mammal, LivesInId = 3},
                    new Animal {Id = 4, Name = "Snake", NumberOfLegs = 0, AnimalType = AnimalTypeEnum.Reptile, LivesInId = 4}
                });
            });


        }
    }
}