using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DemoBed.Services.Order.Persistence.Configuration
{
    public class OrderConfiguration: IEntityTypeConfiguration<Order.Domain.Entities.Order>
    {
        public void Configure(EntityTypeBuilder<Order.Domain.Entities.Order> builder)
        {
            builder.Property(e => e.CreatedBy)
                .IsRequired();

            builder.Property(e => e.CreatedAt)
                .IsRequired();

            builder.Property(e => e.UpdatedBy)
                .IsRequired();

            builder.Property(e => e.UpdatedAt)
                .IsRequired();

            builder.Property(e => e.DeletedAt)
                .IsRequired(false);

            builder.Property(e => e.DeletedBy)
                .IsRequired(false);

            builder.Property(e => e.State)
                .IsRequired();

            builder.Property(e => e.Title)
                .IsRequired();
        }
    }
}
