using Graphity.Tests.Fixtures.Data;
using System;
using System.Linq;

namespace Graphity.Tests.Fixtures
{
    public class ContextFixture
    {
        private static readonly object Lock = new object();

        public static void SeedTestData(TestContext context)
        {
            lock (Lock)
            {
                if (context.Animals.Any())
                {
                    return;
                }

                SeedCountries(context);
                SeedCountryProperties(context);
                SeedAnimals(context);

                context.SaveChanges();
            }
        }

        private static void SeedAnimals(TestContext context)
        {
            context.Animals.AddRange(
                new Animal { Id = 1, Name = "Dog", NumberOfLegs = 4, NumberOfEyelashes = 50, AnimalType = AnimalType.Mammal, LivesInId = 1, HandlerId = Guid.NewGuid() },
                new Animal { Id = 2, Name = "Cat", NumberOfLegs = 4, NumberOfEyelashes = 60, AnimalType = AnimalType.Mammal, LivesInId = 2, HandlerId = Guid.NewGuid() },
                new Animal { Id = 3, Name = "Sloth", NumberOfLegs = 2, NumberOfEyelashes = 70, AnimalType = AnimalType.Mammal, LivesInId = 3, HandlerId = Guid.NewGuid() },
                new Animal { Id = 4, Name = "Snake", NumberOfLegs = 0, NumberOfEyelashes = 80, AnimalType = AnimalType.Reptile, LivesInId = 4, HandlerId = Guid.NewGuid() }
            );
        }

        private static void SeedCountryProperties(TestContext context)
        {
            context.CountryProperties.AddRange(
                new CountryProperties
                {
                    CountryId = 1, Population = 55_619_400, TaxRate = 0.2M, HasMonarchy = true,
                    LastTimeWonWorldCup = new DateTime(1966, 7, 30)
                },
                new CountryProperties
                {
                    CountryId = 2, Population = 67_348_000, TaxRate = 0.2M, HasMonarchy = false,
                    LastTimeWonWorldCup = new DateTime(2018, 7, 15)
                },
                new CountryProperties
                {
                    CountryId = 3, Population = 82_800_000, TaxRate = 0.19M, HasMonarchy = true,
                    LastTimeWonWorldCup = new DateTime(2014, 7, 13)
                },
                new CountryProperties
                {
                    CountryId = 4, Population = 25_155_300, TaxRate = 0.1M, HasMonarchy = true,
                    LastTimeWonWorldCup = null
                }
            );
        }

        private static void SeedCountries(TestContext context)
        {
            context.Countries.AddRange(
                new Country { Id = 1, Name = "England" },
                new Country { Id = 2, Name = "France" },
                new Country { Id = 3, Name = "Germany" },
                new Country { Id = 4, Name = "Australia" }
            );
        }
    }
}