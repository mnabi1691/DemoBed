using DemoBed.Base.EventBus.MassTransitEvents;
using DemoBed.StateMachines.SagaOrchestrationStateMahines.StateInstances;
using MassTransit;

namespace DemoBed.StateMachines.SagaOrchestrationStateMahines.OrderStateMachines
{
    public class OrderStateMachine : MassTransitStateMachine<OrderStateInstance>
    {
        #region Events
        private Event<IPaymentCompletedEvent> PaymentCompletedEvent { get; set; }
        private Event<IPaymentFailedEvent> PaymentFailedEvent { get; set; }
        private Event<IShippingCompletedEvent> ShippingCompletedEvent { get; set; }
        private Event<IShippingFailedEevent> ShippingFailedEvent { get; set; }
        #endregion

        #region States
        private State OrderCreated { get; set; }
        private State PaymentCompleted { get; set; }
        private State PaymentFailed { get; set; }
        #endregion

        #region Command
        private Event<ICreateOrderMessage> CreateOrderMessage { get; set; }
        #endregion

        public OrderStateMachine()
        { 
        }
    }
}
