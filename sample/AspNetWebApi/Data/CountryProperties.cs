using System;

namespace AspNetWebApi.Data
{
    public class CountryProperties
    {
        public Country Country { get; set; }
        public int CountryId { get; set; }
        public long Population { get; set; }
        public decimal TaxRate { get; set; }
        public bool HasMonarchy { get; set; }
        public DateTime? LastTimeWonWorldCup { get; set; }
    }
}