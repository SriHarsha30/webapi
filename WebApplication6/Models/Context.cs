﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
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

        public DbSet<Lease> Leases1 { get; set; }
        public DbSet<Notification> notifications1 { get; set; }

    }
}
