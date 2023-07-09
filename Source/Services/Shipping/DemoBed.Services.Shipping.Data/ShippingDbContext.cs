using DemoBed.Base.Entities;
using DemoBed.Base.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoBed.Services.Shipping.Data
{
    public class ShippingDbContext: DbContext
    {
        private readonly ILoggedInUserService _loggedInUserService;

        public ShippingDbContext(
            DbContextOptions<ShippingDbContext> options, 
            ILoggedInUserService loggedInUserService)
           : base(options)
        {
            _loggedInUserService = loggedInUserService;
        }

        #region Tables
        public DbSet<Shipping.Data.Entities.ShippingInformation> Shippings { get; set; }
        public DbSet<Shipping.Data.Entities.Address> Addresses { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        #endregion

        #region Methods
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken)
        {
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
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

            return await base.SaveChangesAsync(cancellationToken);
        }
        #endregion
    }
}
