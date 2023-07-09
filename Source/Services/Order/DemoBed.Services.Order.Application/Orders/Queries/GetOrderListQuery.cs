using DemoBed.Services.Order.Application.Constants;
using DemoBed.Services.Order.Application.Interfaces;
using DemoBed.Services.Order.Application.Orders.Dtos;
using DemoBed.Services.Order.Application.Orders.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoBed.Services.Order.Application.Orders.Queries
{
    public class GetOrderListQuery: IRequest<List<OrderListDto>>
    {
        public class GetOrderListQueryHandler : IRequestHandler<GetOrderListQuery, List<OrderListDto>>
        {
            private readonly IOrderDbContext _context;

            public GetOrderListQueryHandler(IOrderDbContext context)
            {
                _context = context;
            }

            public async Task<List<OrderListDto>> Handle(GetOrderListQuery request, CancellationToken cancellationToken)
            {
                List<OrderListDto> list = await _context.Orders.Where(o => o.IsDeleted == false)
                    .Select(o => new OrderListDto
                    {
                        Id = o.Id,
                        Title = o.Title,
                        State = ((OrderState)o.State).ToString()
                    }).ToListAsync(cancellationToken);

                return list;
            }
        }
    }
}
