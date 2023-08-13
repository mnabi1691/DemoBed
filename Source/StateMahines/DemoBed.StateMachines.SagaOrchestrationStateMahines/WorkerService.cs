namespace DemoBed.StateMachines.SagaOrchestrationStateMahines
{
    public class WorkerService : BackgroundService
    {
        private readonly ILogger<WorkerService> logger;

        public WorkerService (ILogger<WorkerService> logger)
        {
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // while (!stoppingToken.IsCancellationRequested)
            // {
            //     _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //     await Task.Delay(1000, stoppingToken);
            // }
        }
    }
}
