using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ
{
    public class RabbitBus : IDisposable
    {
        private IConnection _connection;
        private RabbitOptions _options;

        public RabbitBus(RabbitOptions options)
        {
            _options = options;
        }
        public async Task Connect()
        {
            if (_connection != null && _connection.IsOpen) return;
            try
            {
                var factory = new ConnectionFactory
                {
                    UserName = _options.UserName,
                    Password = _options.Password,
                    AutomaticRecoveryEnabled = true,
                    TopologyRecoveryEnabled = true
                };

                // Cluster support
                factory.EndpointResolverFactory = endpoints =>
                    new DefaultEndpointResolver(
                        _options.Hosts.Select(h => new AmqpTcpEndpoint(h)));

                _connection = await factory.CreateConnectionAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("RabbitMQ connection failed", ex);
            }
            try
            {
                using var channel = await _connection.CreateChannelAsync();

                // HOT queue with TTL → COLD DLX
               await channel.QueueDeclareAsync(
                    queue: _options.ColdQueue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);
                Dictionary<string, object> arguments = new()
                {
                    ["x-message-ttl"] = _options.HotQueueTtlMs,
                    ["x-dead-letter-exchange"] = "",
                    ["x-dead-letter-routing-key"] = _options.ColdQueue
                };
                await channel.QueueDeclareAsync(
                    queue: _options.HotQueue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: arguments);

                // COLD queue
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Queue declaration failed", ex);
            }
        }

        public async Task<bool> Publish<T>(T message)
        {
            if (_connection == null || !_connection.IsOpen)
            {
                Console.Error.WriteLine("[PUBLISH] Not connected");
                return false;
            }

            try
            {
                using var channel = await _connection.CreateChannelAsync();

                var envelope = new MessageEnvelope<T> { Payload = message };
                var body = Encoding.UTF8.GetBytes(
                    JsonSerializer.Serialize(envelope));

                var props = new BasicProperties
                {
                    Persistent = true,
                    DeliveryMode = DeliveryModes.Persistent // Alternative way to set persistent (2 = persistent)
                };

                await channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: _options.HotQueue,
                    mandatory:false,
                    basicProperties: props,
                    body: body);

                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[PUBLISH] Error: {ex}");
                return false;
            }
        }

        public async void ConsumeAsync<T>(
       string queue,
       Action<MessageEnvelope<T>, bool> onMessage)
        {
            var channel = await _connection.CreateChannelAsync();
            await channel.BasicQosAsync(0, 1, false);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body.Span);
                var msg = JsonSerializer.Deserialize<MessageEnvelope<T>>(json)!;

                bool isRealtime = queue == _options.HotQueue;
                onMessage(msg, isRealtime);

                // Acknowledge the message
                await channel.BasicAckAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false);
            };

            await channel.BasicConsumeAsync(queue, autoAck: false, consumer);
        }
        public void Dispose() => _connection.Dispose();
    }
}
