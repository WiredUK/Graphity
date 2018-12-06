namespace AspNetWebApi.Data
{
    public class Animal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumberOfLegs { get; set; }

        public int LivesInId { get; set; }
        public Country LivesIn { get; set; }
    }
}