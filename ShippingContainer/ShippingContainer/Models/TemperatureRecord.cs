using System;
using System.ComponentModel.DataAnnotations;

namespace ShippingContainer.Models
{
    /// <summary>
    /// Storage representation of a TemperatureRecord entry
    /// </summary>
    public class TemperatureRecord
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        [Key]
        public int Id { get; set; }


        /// <summary>
        /// The trip this measurement is associated with
        /// </summary>
        /// <remarks>
        /// This is stored to make querying simpler later
        /// </remarks>
        public int TripId { get; set; }


        /// <summary>
        /// The parent container for this measurement
        /// </summary>
        public int ContainerId { get; set; }


        /// <summary>
        /// The DateTime this measurement was taken (per spec)
        /// </summary>
        public DateTime Time { get; set; }


        /// <summary>
        /// The Value of this measurement in degrees celcius (per spec)
        /// </summary>
        public float Value { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        public TemperatureRecord()
        {
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>
        /// Concession for testing
        /// </remarks>
        public TemperatureRecord(DateTime time, float value)
        {
            Time = time;
            Value = value;
        }
    }
}
