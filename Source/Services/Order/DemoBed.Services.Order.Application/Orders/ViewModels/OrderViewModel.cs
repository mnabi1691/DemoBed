using DemoBed.Base.Models;
using DemoBed.Services.Order.Application.Orders.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoBed.Services.Order.Application.Orders.ViewModels
{
    public class OrderViewModel
    {
        public OrderViewModel()
        {
            OrderStateList = new List<SelectModel>();
        }

        public OrderDto Order { get; set; } = null!;
        public List<SelectModel> OrderStateList { get; set; } 
    }
}
