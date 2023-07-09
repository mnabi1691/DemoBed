using AutoMapper;
using DemoBed.Services.Order.Application.Interfaces;
using DemoBed.Services.Order.Application.Orders.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoBed.Services.Order.Application.Orders.Commands
{
    public class UpsertOrderCommand: IRequest<OrderDto>
    {
        public int Id { get; set; }

        public UpsertOrderDto Order { get; set; }

        public class UpsertOrderCommandHandler : IRequestHandler<UpsertOrderCommand, OrderDto>
        {
            private readonly IOrderDbContext _context;
            private readonly IMapper _mapper;

            public UpsertOrderCommandHandler(IOrderDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<OrderDto> Handle(
                UpsertOrderCommand request, CancellationToken cancellationToken)
            {
                OrderDto dto = new();
                Domain.Entities.Order entity;

                if (request.Id > 0)
                {
                    entity = await _context.Orders.Where(
                        o => o.IsDeleted == false && o.Id == request.Id)
                        .SingleAsync(cancellationToken);
                }
                else
                {
                    entity = new Domain.Entities.Order();
                    _context.Orders.Add(entity);
                }

                _mapper.Map(request.Order, entity);
                await _context.SaveChangesAsync(cancellationToken);

                _mapper.Map(entity, dto);

                return dto;
            }
        }
    }
}
