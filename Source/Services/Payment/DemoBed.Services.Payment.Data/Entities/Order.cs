using DemoBed.Base.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoBed.Services.Payment.Data.Entities
{
    public class Order: AuditableEntity
    {
        public int OrderId { get; set; }
        public string Title { get; set; } = null!;
    }
}
