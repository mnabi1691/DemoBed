using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoBed.Services.Payment.ApplicationService.Dtos
{
    public class UpsertPaymentDto: Profile
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public string State { get; set; }

        [Required]
        public int OrderId { get; set; }

        public UpsertPaymentDto()
        {
            CreateMap<UpsertPaymentDto, Payment.Data.Entities.Payment>();
        }
    }
}
