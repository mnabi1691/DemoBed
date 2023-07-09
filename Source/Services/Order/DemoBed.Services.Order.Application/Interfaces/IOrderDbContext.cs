using DemoBed.Base.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoBed.Services.Order.Application.Interfaces
{
    public interface IOrderDbContext
    {
        #region tables
        DbSet<Order.Domain.Entities.Order> Orders { get; set; }
        DbSet<User> Users { get; set;}
        DbSet<Role> Roles { get; set; }
        #endregion

        #region methods
        Task<int> SaveChangesAsync(CancellationToken token = default);
        int SaveChanges();
        #endregion
    }
}
