using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoBed.Base.MassTransit
{
    public class MassTransitService : IMassTransitService
    {
        private readonly ISendEndpointProvider sendEndpointProvider;
        private readonly IPublishEndpoint publishEndpoint;

        public MassTransitService(ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint)
        {
            this.sendEndpointProvider = sendEndpointProvider;
            this.publishEndpoint = publishEndpoint;
        }

        public async Task Send<T>(T message, string queueName) where T : class
        {
            var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{queueName}"));

            await sendEndpoint.Send<T>(message);
        }

        public async Task Publish<T>(T message) where T : class
        {
            await publishEndpoint.Publish<T>(message);
        }
    }
}
