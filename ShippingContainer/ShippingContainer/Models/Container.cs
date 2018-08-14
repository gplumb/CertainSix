using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShippingContainer.Models
{
    public class Container
    {
        [Key]
        public int Id { get; set; }

        public int TripId { get; set; }

        public string ContainerId { get; set; }

        public double ProductCount { get; set; }

        public float MaxTemperature { get; set; }

        public bool IsSpoiled { get; set; }

        public List<TemperatureRecord> Temperatures { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
