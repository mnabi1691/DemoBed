using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoBed.Services.Payment.ApplicationService.Dtos
{
    public class PaymentListDto
    {
        public string Title { get; set; }

        public decimal Amount { get; set; }

        public string State { get; set; }

        public int OrderId { get; set; }

        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

    }
}
