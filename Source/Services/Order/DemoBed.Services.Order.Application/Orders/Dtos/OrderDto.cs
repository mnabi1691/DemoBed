using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoBed.Services.Order.Application.Orders.Dtos
{
    public class OrderDto: Profile
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int State { get; set; }

        public OrderDto()
        {
            CreateMap<Order.Domain.Entities.Order, OrderDto>();
        }
    }
}
