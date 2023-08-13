using MassTransit;

namespace DemoBed.StateMachines.SagaOrchestrationStateMahines.StateInstances
{
    public class OrderStateInstance : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; } = String.Empty;
        public int OrderId { get; set; }
        public string CustomerId { get; set; } = String.Empty;
        public string? PaymentId { get; set; }
        public string? ShippingId { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
