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

        public List<TemperatureRecord> Temperatures { get; set; }


        // TODO: Store mean temperature or calculate via query?

        // TODO: MaxTemperature

        // TODO: IsSpoiled
    }
}
