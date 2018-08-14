using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ShippingContainer.Models;

namespace ShippingContainer.Interfaces
{
    public interface IShippingRepository
    {
        DbSet<Trip> Trips { get; set; }

        DbSet<Container> Containers { get; set; }

        DbSet<TemperatureRecord> TemperatureRecords { get; set; }

        DatabaseFacade Database { get; }

        int SaveChanges();
    }
}
