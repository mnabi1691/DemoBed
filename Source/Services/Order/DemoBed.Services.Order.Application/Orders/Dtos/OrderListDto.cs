using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoBed.Services.Order.Application.Orders.Dtos
{
    public class OrderListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string State { get; set; } = null!;
    }
}
