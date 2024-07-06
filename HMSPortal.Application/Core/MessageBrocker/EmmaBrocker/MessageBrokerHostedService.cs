using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DHTMLX.Scheduler;


namespace HMSPortal.Application.Core.MessageBrocker.EmmaBrocker
{
   
    public class MessageBrokerHostedService : IHostedService
    {
        private readonly IMessageBroker _messageBroker;
        private readonly ILogger<MessageBrokerHostedService> _logger;

        public MessageBrokerHostedService(IMessageBroker messageBroker, ILogger<MessageBrokerHostedService> logger)
        {
            _messageBroker = messageBroker;
            _logger = logger;
        }

        public  async Task StartAsync(CancellationToken cancellationToken)
        {
             await   _messageBroker.SubscribeToAppoitmentAsync("Client");
            _logger.LogInformation("MessageBrokerHostedService is starting.");
            // Any initialization code for the message broker can go here.
            //return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("MessageBrokerHostedService is stopping.");
            // Any cleanup code for the message broker can go here.
            return Task.CompletedTask;
        }
    }

}
