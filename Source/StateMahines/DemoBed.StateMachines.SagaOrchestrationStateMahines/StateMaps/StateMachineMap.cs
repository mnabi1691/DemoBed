using DemoBed.StateMachines.SagaOrchestrationStateMahines.StateInstances;
using MassTransit;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace DemoBed.StateMachines.SagaOrchestrationStateMahines.StateMaps
{
    public class StateMachineMap : SagaClassMap<OrderStateInstance>
    {
        protected override void Configure(EntityTypeBuilder<OrderStateInstance> entity, ModelBuilder model)
        {
        }
    }
}
