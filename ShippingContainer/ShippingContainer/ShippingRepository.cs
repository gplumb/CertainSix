using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ShippingContainer.Interfaces;
using ShippingContainer.Models;

namespace ShippingContainer
{
    public class ShippingRepository : DbContext, IShippingRepository
    {
        public ShippingRepository(DbContextOptions<ShippingRepository> options)
          : base(options)
        {
        }

        public DbSet<Trip> Trips { get; set; }
    }
}
