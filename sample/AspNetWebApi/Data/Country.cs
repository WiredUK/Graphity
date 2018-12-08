using System.Collections.Generic;

namespace AspNetWebApi.Data
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Animal> Animals { get; set; }
    }
}