using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoBed.Services.Order.Application.Orders.Dtos
{
    public class UpsertOrderDto : Profile
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [StringLength(400, ErrorMessage = "Title can not be more than 400 characters long")]
        public string Title { get; set; } = null!;
        public int State { get; set; } = null!;

        public UpsertOrderDto()
        {
            CreateMap<UpsertOrderDto, Order.Domain.Entities.Order>();
        }
    }
}
