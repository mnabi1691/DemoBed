using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoBed.Services.Payment.ApplicationService.Dtos
{
    public class PaymentDetailsDto : Profile
    {
        public string Title { get; set; }

        public decimal Amount { get; set; }

        public string State { get; set; }

        public int OrderId { get; set; }

        #region Tracking
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

        #endregion

        public PaymentDetailsDto()
        {
            CreateMap<Payment.Data.Entities.Payment, PaymentDetailsDto>();
        }
    }
}
