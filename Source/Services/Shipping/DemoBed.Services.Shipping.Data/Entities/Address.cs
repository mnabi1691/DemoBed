using DemoBed.Base.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoBed.Services.Shipping.Data.Entities
{
    public class Address: AuditableEntity
    {
        public int ShippingId { get; set; }

        public string AddressLine { get; set; }

        public string City { get; set; }

        public string Country { get; set; }
    }
}
