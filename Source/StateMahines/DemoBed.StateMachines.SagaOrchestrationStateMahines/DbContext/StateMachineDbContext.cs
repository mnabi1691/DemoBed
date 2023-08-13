using DemoBed.StateMachines.SagaOrchestrationStateMahines.StateMaps;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;

namespace DemoBed.StateMachines.SagaOrchestrationStateMahines.DbContext
{
    public class StateMachineDbContext : SagaDbContext
    {
        public StateMachineDbContext(DbContextOptions<StateMachineDbContext> options) : base(options)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new StateMachineMap(); }
        }
    }
}
