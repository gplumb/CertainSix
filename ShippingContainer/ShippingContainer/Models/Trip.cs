using System;
using System.ComponentModel.DataAnnotations;

namespace ShippingContainer.Models
{
    /// <summary>
    /// Storage representation of a Trip
    /// </summary>
    public class Trip
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        [Key]
        public int Id { get; set; }


        /// <summary>
        /// The name of this trip (per spec)
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// The temperature, in degrees celcius, at spoilage will begin (per spec)
        /// </summary>
        public float SpoilTemperature { get; set; }


        /// <summary>
        /// The consecutive duration, in seconds, at which spoilage will occur (per spec)
        /// </summary>
        public double SpoilDuration { get; set; }


        /// <summary>
        /// DateTime this entry was created
        /// </summary>
        public DateTime Created { get; set; } = DateTime.UtcNow;


        /// <summary>
        /// DateTime this entry was updated
        /// </summary>
        public DateTime Updated { get; set; }
    }
}
