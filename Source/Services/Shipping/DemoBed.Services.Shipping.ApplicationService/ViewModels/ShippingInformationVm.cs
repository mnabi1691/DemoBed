using DemoBed.Services.Shipping.ApplicationService.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoBed.Services.Shipping.ApplicationService.ViewModels
{
    public class ShippingInformationVm
    {
        public ShippingInformationDto Shipping { get; set; }
        public AddressDto Address { get; set; }
    }
}
