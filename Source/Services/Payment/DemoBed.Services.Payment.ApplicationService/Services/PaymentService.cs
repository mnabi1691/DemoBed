using AutoMapper;
using DemoBed.Base.Data;
using DemoBed.Services.Payment.ApplicationService.Constants;
using DemoBed.Services.Payment.ApplicationService.Dtos;
using DemoBed.Services.Payment.ApplicationService.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoBed.Services.Payment.ApplicationService.Services
{
    public class PaymentService: IPaymentService
    {
        private readonly IRepository<Payment.Data.Entities.Payment> _repository;
        private readonly IRepository<Payment.Data.Entities.Order> _orderRepository;
        private readonly IMapper _mapper;

        public PaymentService
            (IRepository<Payment.Data.Entities.Payment> repository
            , IRepository<Payment.Data.Entities.Order> orderRepository
            , IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
            _orderRepository = orderRepository;
        }

        public async Task<PaymentDetailsDto> AddPaymentAsync(UpsertPaymentDto payment)
        {
            Payment.Data.Entities.Payment entity =
                _mapper.Map<Payment.Data.Entities.Payment>(payment);

            entity.State = PaymentState.Pending.ToString();

            await _repository.AddAsync(entity);

            return _mapper.Map<PaymentDetailsDto>(entity);
        }

        public async Task<bool> UpdatePaymentAsync(int id, UpsertPaymentDto payment)
        {
            Payment.Data.Entities.Payment? entity =
                await _repository.GetByIdAsync(id, new CancellationToken());

            if (entity == null)
            {
                throw new Exception("Entity not found.");
            }

            _mapper.Map(payment, entity);
            await _repository.UpdateAsync(entity, new CancellationToken());

            return true;
        }

        public async Task<bool> DeletePaymentAsync(int id)
        {
            Payment.Data.Entities.Payment? entity =
                await _repository.GetByIdAsync(id, new CancellationToken());

            if (entity == null)
            {
                throw new Exception("Entity not found.");
            }

            return await _repository.RemoveAsync(entity);
        }

        public async Task<PaymentVm> GetPaymentAsync(int id)
        {
            PaymentVm vm = new PaymentVm();

            Payment.Data.Entities.Payment? entity =
                await _repository.GetByIdAsync(id, new CancellationToken());

            if (entity == null)
            {
                throw new Exception("Entity not found.");
            }

            vm.Payment = _mapper.Map<PaymentDetailsDto>(entity);

            vm.Orders = await _orderRepository.GetAllQueryable()
                .Select(o => new Base.Models.SelectModel
                {
                    Id = o.OrderId.ToString(),
                    Value = o.Title
                }).ToListAsync(new CancellationToken());

            return vm;
        }

        public async Task<List<PaymentListDto>> GetPaymentsAsync()
        {
            List<PaymentListDto> list = await _repository.GetAllQueryable()
                .Select(p => new PaymentListDto
                {
                    Id = p.Id,
                   Title = p.Title,
                   Amount = p.Amount,
                   State = p.State,
                   CreatedAt = p.CreatedAt,
                   UpdatedAt = p.UpdatedAt,
                   CreatedBy = p.CreatedBy,
                   UpdatedBy = p.UpdatedBy
                }).ToListAsync(new CancellationToken());

            return list;
        }
    }
}
