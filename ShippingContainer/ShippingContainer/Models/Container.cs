using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShippingContainer.Models
{
    /// <summary>
    /// Storage representation of a Container
    /// </summary>
    public class Container
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        [Key]
        public int Id { get; set; }


        /// <summary>
        /// The trip this container belongs to
        /// </summary>
        public int TripId { get; set; }


        /// <summary>
        /// The id of this container (per spec)
        /// </summary>
        public string ContainerId { get; set; }


        /// <summary>
        /// The amount of product in this container (per spec)
        /// </summary>
        public double ProductCount { get; set; }


        /// <summary>
        /// The maximum temperature this container has reached
        /// </summary>
        public float MaxTemperature { get; set; }


        /// <summary>
        /// Determines if the contents of this container are spoiled
        /// </summary>
        /// <remarks>
        /// This is determined by the parent Trip's configuration
        /// </remarks>
        public bool IsSpoiled { get; set; }


        /// <summary>
        /// The temperature data for this container (per spec)
        /// </summary>
        public List<TemperatureRecord> Temperatures { get; set; }


        /// <summary>
        /// The DeteTime this entry was created
        /// </summary>
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
