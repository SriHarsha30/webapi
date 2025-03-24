using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;

namespace WebApplication6.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Registration> Registrationss { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Maintainance> Maintainances { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Lease> Leases1 { get; set; }
        public DbSet<Notification> notifications1 { get; set; }

        public DbSet<History> Histories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Lease>().ToTable(Tb => Tb.HasTrigger("InsertHistoryOnLeaseUpdate"));
            modelBuilder.Entity<History>().HasNoKey();
        }

       



    }

}
