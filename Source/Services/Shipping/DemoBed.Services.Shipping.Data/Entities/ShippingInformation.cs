using DemoBed.Base.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoBed.Services.Shipping.Data.Entities
{
    public class ShippingInformation: AuditableEntity
    {
        public string Title { get; set; }

        public string State { get; set; }

        public int OrderId { get; set; }

        public Address Address { get; set; }
    }
}
