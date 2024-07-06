using Confluent.Kafka;
using HMSPortal.Application.AppServices.IServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.MessageBrocker.KafkaBus
{
    public class KafkaQueueService 
    {
        private readonly ILogger<KafkaQueueService> _logger;
        private readonly IProducer<Null, string> _producer;
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly string _brokerList;
        private readonly string _groupId;
        private readonly List<string> _topics;
        private readonly IAppointmentServices appointmentServices;

        public KafkaQueueService(ILogger<KafkaQueueService> logger, string brokerList, string groupId, List<string> topics)
        {
            _logger = logger;
            _brokerList = brokerList;
            _groupId = groupId;
            _topics = topics;
            string bootstrapServers = "pkc-4r087.us-west2.gcp.confluent.cloud:9092";  // Replace with your Confluent Cloud bootstrap servers
            string saslUsername = "2PQCGVZ5ZDGDQBTM";          // Replace with your Confluent Cloud API key
            string saslPassword = "7DsaOr2kbdBsnG0hdwYrg0FU9o/1ivFQyGKyoUN76ngt8mfd6AyVJdmSWCpvNeT3";          // Replace with your Confluent Cloud API secret
            string topic = "topic_3";


            var producerConfig = new ProducerConfig
            {
                BootstrapServers = _brokerList,
                SaslUsername = saslUsername,
                SaslPassword = saslPassword,
                SaslMechanism = SaslMechanism.Plain,
                SecurityProtocol = SecurityProtocol.SaslSsl,
            };

            var consumerConfig = new ConsumerConfig
            {
                GroupId = _groupId,
                BootstrapServers = _brokerList,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                SaslUsername = saslUsername,
                SaslPassword = saslPassword,
                SaslMechanism = SaslMechanism.Plain,
                SecurityProtocol = SecurityProtocol.SaslSsl,
            };

            _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
            _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _consumer.Subscribe(_topics);

            Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var cr = _consumer.Consume(cancellationToken);
                        _logger.LogInformation($"Consumed message '{cr.Value}' from '{cr.TopicPartitionOffset}'.");

                        // Handle specific topic logic
                        if (cr.Topic == "topic_3")
                        {
                            HandleOrderTopic(cr.Value);
                        }
                    }
                    catch (ConsumeException e)
                    {
                        _logger.LogError($"Error occurred: {e.Error.Reason}");
                    }
                }
            }, cancellationToken);

            return Task.CompletedTask;
        }

        public async Task PublishMessage(string topic, string message)
        {
            try
            {
                var deliveryResult = await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
                _logger.LogInformation($"Delivered '{deliveryResult.Value}' to '{deliveryResult.TopicPartitionOffset}'");
            }
            catch (ProduceException<Null, string> e)
            {
                _logger.LogError($"Delivery failed: {e.Error.Reason}");
            }
        }

        private void HandleOrderTopic(string message)
        {
            // Add logic to send notification for order completion
            // For example, log the message or send an email/notification
            _logger.LogInformation($"Order completed: {message}");
            // You can add your notification sending logic here
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _consumer.Close();
            return Task.CompletedTask;
        }
    }
}
