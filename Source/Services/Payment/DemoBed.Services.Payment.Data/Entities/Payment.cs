using DemoBed.Base.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoBed.Services.Payment.Data.Entities
{
    public class Payment: AuditableEntity
    {
        public string Title { get; set; } = null!;

        public decimal Amount { get; set; }

        public string State { get; set; } = null!;

        public int OrderId { get; set; }

        public Order Order { get; set; } = null!;

    }
}
