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

        public XiugouDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ticket>().HasKey(e => e.Id);
            modelBuilder.Entity<Ticket>().Property(e => e.TicketType)
                .HasConversion(new EnumToNumberConverter<TicketType, int>());

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();

            UpdateTimestampProperty<Ticket>();

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
