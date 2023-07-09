using AutoMapper;
using DemoBed.Services.Order.Application.Interfaces;
using DemoBed.Services.Order.Application.Orders.Dtos;
using DemoBed.Services.Order.Application.Orders.ViewModels;
using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoBed.Services.Order.Application.Orders.Queries
{
    public class GetOrderDetailsQuery: IRequest<OrderViewModel>
    {
        public int Id { get; set; }

        public class GetOrderDetailsQueryHandler :
            IRequestHandler<GetOrderDetailsQuery, OrderViewModel>
        {
            private readonly IOrderDbContext _context;
            private readonly IMapper _mapper;

            public GetOrderDetailsQueryHandler(
                IOrderDbContext context, 
                IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<OrderViewModel> Handle(
                GetOrderDetailsQuery request,
                CancellationToken cancellationToken)
            {
                OrderViewModel vm = new();

                if (request.Id > 0)
                {
                    var entity = await _context.Orders
                        .Where(o => o.IsDeleted == false && o.Id == request.Id)
                        .SingleAsync(cancellationToken);

                    vm.Order = _mapper.Map<OrderDto>(entity);
                }
                else
                {
                    vm.Order = new Dtos.OrderDto()
                    {
                        Id = 0,
                        Title = ""
                    };
                }

                vm.OrderStateList = Utilitty.Utilitty
                    .GetSelectModelListFromOrderState();

                return vm;
            }
        }
    }
}

