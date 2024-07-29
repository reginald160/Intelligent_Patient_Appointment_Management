using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.MessageBrocker.EmmaBrocker
{
    public  interface IMessageBroker
    {
        Task PublishAsync(string topic, string message);
        Task SubscribeAsync(string topic, string subscriber, Func<string, Task> handler);
        Task SubscribeDoctorSignUp(string subscriber);
        Task SubscribeToAppoitmentAsync(string subscriber);
        Task UnsubscribeAsync(string topic, string subscriber);
    }
}
