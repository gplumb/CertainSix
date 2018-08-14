using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ShippingContainer.Models;

namespace ShippingContainer.Interfaces
{
    /// <summary>
    /// EF storage abstraction for shipping
    /// </summary>
    public interface IShippingRepository
    {
        /// <summary>
        /// Maps to 'Trip' table
        /// </summary>
        DbSet<Trip> Trips { get; set; }


        /// <summary>
        /// Maps to 'Container' table
        /// </summary>
        DbSet<Container> Containers { get; set; }


        /// <summary>
        /// Maps to 'TemperatureRecord' table
        /// </summary>
        DbSet<TemperatureRecord> TemperatureRecords { get; set; }


        /// <summary>
        /// Expose the underlying Database for direct manipulation
        /// </summary>
        DatabaseFacade Database { get; }


        /// <summary>
        /// Save any changes made to the current EF object graph
        /// </summary>
        int SaveChanges();
    }
}
