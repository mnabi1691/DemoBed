using DemoBed.Base.Entities;
using DemoBed.Base.Services;
using DemoBed.Services.Order.Application.Interfaces;
using DemoBed.Services.Order.Domain.Entities;
using DemoBed.Services.Order.Persistence.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DemoBed.Services.Order.Persistence
{
    public class OrderDbContext: DbContext, IOrderDbContext
    {
        private readonly ILoggedInUserService _loggedInUserService;
        public OrderDbContext(
            ILoggedInUserService loggedInUserService
            , DbContextOptions<OrderDbContext> options) : base(options)
        { 
        }


        #region tables
        public DbSet<Order.Domain.Entities.Order> Orders { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        #endregion

        #region methods

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(OrderConfiguration).Assembly);
        }

        public void ChangeTracking(IEnumerable<EntityEntry<AuditableEntity>> entries)
        {
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedBy = _loggedInUserService?.User?.UserId;
                    entry.Entity.UpdatedBy = _loggedInUserService?.User?.UserId;
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedBy = _loggedInUserService?.User?.UserId;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken token = default)
        {
            ChangeTracking(ChangeTracker.Entries<AuditableEntity>());
            return await base.SaveChangesAsync(token);
        }
        public override int SaveChanges()
        {
            ChangeTracking(ChangeTracker.Entries<AuditableEntity>());
            return base.SaveChanges();
        }
        #endregion
    }
}
