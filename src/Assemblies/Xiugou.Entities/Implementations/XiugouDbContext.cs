using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Xiugou.Entities.Entities;
using Xiugou.Entities.Enums;

namespace Xiugou.Entities.Implementations
{
    public class XiugouDbContext: DbContext, IXiugouDbContext
    {
        public DbSet<Ticket> Tickets { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Session> Sessions { get; set; }

        public DbSet<H5Profile> H5Profiles { get; set; }

        public XiugouDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ticket
            modelBuilder.Entity<Ticket>().HasKey(e => e.Id);
            modelBuilder.Entity<Ticket>().Property(e => e.TicketType)
                .HasConversion(new EnumToNumberConverter<TicketType, int>());
            modelBuilder.Entity<Ticket>().Property(e => e.Platform)
                .HasConversion(new EnumToNumberConverter<Platform, int>());
            modelBuilder.Entity<Ticket>().Property(e => e.Event)
                .HasConversion(new EnumToNumberConverter<Event, int>());

            // User
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<User>().Property(u => u.Platform)
                .HasConversion(new EnumToNumberConverter<Platform, int>());

            // Session
            modelBuilder.Entity<Session>().HasKey(s => s.Id);

            // H5Profile
            modelBuilder.Entity<H5Profile>().HasKey(s => s.Id);
            modelBuilder.Entity<H5Profile>().Property(e => e.Platform)
                .HasConversion(new EnumToNumberConverter<Platform, int>());

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();

            UpdateTimestampProperty<Ticket>();
            UpdateTimestampProperty<User>();

            return base.SaveChanges();
        }

        private void UpdateTimestampProperty<T>() where T : class
        {
            var modifiedSourceInfo =
                ChangeTracker.Entries<T>()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in modifiedSourceInfo)
            {
                var currentUtc = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    entry.Property("CreatedUtc").CurrentValue = currentUtc;
                }

                entry.Property("UpdatedUtc").CurrentValue = currentUtc;
            }
        }
    }
}
