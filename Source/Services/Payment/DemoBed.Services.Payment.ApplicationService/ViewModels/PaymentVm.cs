using DemoBed.Base.Models;
using DemoBed.Services.Payment.ApplicationService.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoBed.Services.Payment.ApplicationService.ViewModels
{
    public class PaymentVm
    {
        public PaymentVm()
        {
            Orders = new List<SelectModel>();
        }
        public PaymentDetailsDto Payment { get; set; } = null!;
        public List<SelectModel> Orders { get; set; }
    }
}
