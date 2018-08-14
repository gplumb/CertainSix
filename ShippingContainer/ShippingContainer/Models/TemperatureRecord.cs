using System;
using System.ComponentModel.DataAnnotations;

namespace ShippingContainer.Models
{
    public class TemperatureRecord
    {
        [Key]
        public int Id { get; set; }

        public int TripId { get; set; }

        public int ContainerId { get; set; }

        public DateTime Time { get; set; }

        public float Value { get; set; }
    }
}
