using System.Collections.Generic;

namespace AspNetWebApi.Data
{
    public class Animal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumberOfLegs { get; set; }

        public ICollection<Country> LivesIn { get; set; }
    }
}