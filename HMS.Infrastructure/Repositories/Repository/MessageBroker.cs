using HMSPortal.Application.Core.MessageBrocker.EmmaBrocker;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HMS.Infrastructure.Persistence.DataContext;
using HMSPortal.Domain.Models;
using HMSPortal.Application.AppServices.IServices;
using DHTMLX.Scheduler;
using Newtonsoft.Json;
using HMSPortal.Application.ViewModels.Appointment;
using HMSPortal.Application.Core;
using Microsoft.Extensions.DependencyInjection;
using HMSPortal.Application.Core.Chat.Bot;

namespace HMS.Infrastructure.Repositories.Repository
{
    public class MessageBroker : IMessageBroker
    {
  
        private readonly ILogger<MessageBroker> _logger;
        private readonly ConcurrentDictionary<string, List<Func<string, Task>>> _handlers = new();
        private readonly IServiceScopeFactory _scopeFactory;
        public MessageBroker( ILogger<MessageBroker> logger,  IServiceScopeFactory scopeFactory)
        {
       
            _logger = logger;

            _scopeFactory=scopeFactory;
        }

        private async Task Pub(string topic, string message)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var newMessage = new BrockerMessage
                {
                    Topic = topic,
                    Content = message,
                    Timestamp = DateTime.UtcNow,
                    Delivered = false
                };

                _context.BrockerMessages.Add(newMessage);
                await _context.SaveChangesAsync();

                if (_handlers.ContainsKey(topic))
                {
                    foreach (var handler in _handlers[topic])
                    {
                        await handler(message);

                    }

                    newMessage.Delivered = true;
                    _context.BrockerMessages.Update(newMessage);
                    await _context.SaveChangesAsync();
                }

                // Use dbContext here
            }
        }
        public async Task PublishAsync(string topic, string message)
        {
             Task.Run(() => Pub(topic, message));
   
        }

        public async Task SubscribeAsync(string topic, string subscriber, Func<string, Task> handler)
        {
            if (!_handlers.ContainsKey(topic))
            {
                _handlers[topic] = new List<Func<string, Task>>();
            }

            _handlers[topic].Add(handler);

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                if (!await _context.BrockerSubscriptions.AnyAsync(s => s.Topic == topic && s.Subscriber == subscriber))
                {
                    _context.BrockerSubscriptions.Add(new BrockerSubscription
                    {
                        Topic = topic,
                        Subscriber = subscriber
                    });

                    await _context.SaveChangesAsync();
                }
            }

        }

        public async Task UnsubscribeAsync(string topic, string subscriber)
        {
            if (_handlers.ContainsKey(topic))
            {
                _handlers[topic].RemoveAll(handler => handler.Method.Name == subscriber);
            }
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var subscription = await _context.BrockerSubscriptions.FirstOrDefaultAsync(s => s.Topic == topic && s.Subscriber == subscriber);
                if (subscription != null)
                {
                    _context.BrockerSubscriptions.Remove(subscription);
                    await _context.SaveChangesAsync();
                }
            }

        }

        public async Task SubscribeToAppoitmentAsync(string subscriber)
        {
            await SubscribeAsync(CoreValiables.BootAppointmentTopic, subscriber, async (msg) =>
            {
                var appointment = JsonConvert.DeserializeObject<AddAppointmentViewModel>(msg);
                using (var scope = _scopeFactory.CreateScope())
                {
                    var _appServices = scope.ServiceProvider.GetRequiredService<IAppointmentServices>();
                    await _appServices.CreateAppointmentByPatient(appointment);

                }

                // Simulate sending notification
            });
        }
    }

}
