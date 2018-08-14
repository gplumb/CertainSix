using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ShippingContainer.Interfaces;
using ShippingContainer.Models;

namespace ShippingContainer
{
    /// <summary>
    /// Implementation of IShippingRepository using EF Core
    /// </summary>
    public class ShippingRepository : DbContext, IShippingRepository
    {
        /// </inheritdoc />
        public DbSet<Trip> Trips { get; set; }

        /// </inheritdoc />
        public DbSet<Container> Containers { get; set; }

        /// </inheritdoc />
        public DbSet<TemperatureRecord> TemperatureRecords { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        public ShippingRepository(DbContextOptions<ShippingRepository> options)
          : base(options)
        {
        }
    }
}
