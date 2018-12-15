namespace AspNetWebApi.Data
{
    public class Animal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public short NumberOfLegs { get; set; }
        public AnimalTypeEnum AnimalType { get; set; }

        public int LivesInId { get; set; }
        public Country LivesIn { get; set; }
    }
}