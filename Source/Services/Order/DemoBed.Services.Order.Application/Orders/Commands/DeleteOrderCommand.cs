using DemoBed.Services.Order.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoBed.Services.Order.Application.Orders.Commands
{
    public class DeleteOrderCommand: IRequest<bool>
    {
        public int Id { get; set; }

        public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, bool>
        {
            private readonly IOrderDbContext _context;

            public DeleteOrderCommandHandler(IOrderDbContext context)
            {
                _context = context;
            }

            public async Task<bool> Handle(
                DeleteOrderCommand request, CancellationToken cancellationToken)
            {
                var entity = await _context.Orders
                    .Where(o => o.IsDeleted == false && o.Id == request.Id)
                    .SingleAsync(cancellationToken);

                entity.IsDeleted = true;
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
        }
    }
}
