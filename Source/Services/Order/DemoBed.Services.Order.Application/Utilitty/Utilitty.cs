using DemoBed.Base.Models;
using DemoBed.Services.Order.Application.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DemoBed.Services.Order.Application.Utilitty
{
    public class Utilitty
    {
        public static List<SelectModel> GetSelectModelListFromOrderState()
        {
            List<SelectModel> list = Enum
                .GetValues(typeof(OrderState)).Cast<OrderState>()
                .Select(e => new SelectModel
                { 
                    Id = ((int)e).ToString(),
                    Value = e.ToString()

                }).ToList();

            return list;
        }
    }
}
