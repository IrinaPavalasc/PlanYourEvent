using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProiectLicenta.Models;

namespace ProiectLicenta.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> option) : base(option)
        {
        }

        public DbSet<Venue> Venue { get; set; } 
        public DbSet<User> User { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<Calendar> Calendar { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet <Reservation> Reservation { get; set; }


    }
}
