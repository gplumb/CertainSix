using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ShippingContainer.Models;

namespace ShippingContainer.Interfaces
{
    public interface IShippingRepository
    {
        DbSet<Trip> Trips { get; set; }

        DatabaseFacade Database { get; }

        int SaveChanges();
    }
}
