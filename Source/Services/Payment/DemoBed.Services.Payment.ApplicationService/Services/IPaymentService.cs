using DemoBed.Services.Payment.ApplicationService.Dtos;
using DemoBed.Services.Payment.ApplicationService.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoBed.Services.Payment.ApplicationService.Services
{
    public interface IPaymentService
    {
        Task<PaymentDetailsDto> AddPaymentAsync(UpsertPaymentDto payment);
        Task<bool> UpdatePaymentAsync(int id, UpsertPaymentDto payment);
        Task<bool> DeletePaymentAsync(int id);
        Task<PaymentVm> GetPaymentAsync(int id);
        Task<List<PaymentListDto>> GetPaymentsAsync();
    }
}
