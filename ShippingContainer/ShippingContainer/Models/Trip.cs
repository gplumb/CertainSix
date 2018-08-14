using System.ComponentModel.DataAnnotations;

namespace ShippingContainer.Models
{
    public class Trip
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public float SpoilTemperature { get; set; }

        public double SpoilDuration { get; set; }
    }
}
