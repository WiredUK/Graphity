using System.Collections.Generic;

namespace Graphity.Tests.Fixtures.Data
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public CountryProperties CountryProperties { get; set; }
        public ICollection<Animal> Animals { get; set; }
    }
}