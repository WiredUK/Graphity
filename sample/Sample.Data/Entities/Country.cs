// ReSharper disable UnusedMember.Global

using System.Collections.Generic;

namespace Sample.Data.Entities
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public byte SomeByteValue { get; set; }

        public CountryProperties CountryProperties { get; set; }
        public ICollection<Animal> Animals { get; set; }
    }
}