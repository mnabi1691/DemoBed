using DemoBed.Base.Entities;

namespace DemoBed.Services.Order.Domain.Entities
{
    public class Order : AuditableEntity
    {
        public string Title { get; set; } = null!;

        public int State { get; set; }
    }
}