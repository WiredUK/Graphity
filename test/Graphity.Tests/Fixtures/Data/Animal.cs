using System;

namespace Graphity.Tests.Fixtures.Data
{
    public class Animal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public short NumberOfLegs { get; set; }
        public AnimalType AnimalType { get; set; }

        public Guid HandlerId { get; set; }
        public AnimalType? NullableAnimalType { get; set; }

        public int LivesInId { get; set; }
        public Country LivesIn { get; set; }
    }
}