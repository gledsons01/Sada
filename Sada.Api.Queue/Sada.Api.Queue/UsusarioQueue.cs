using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using System.Text.Json;
using System.Reflection.Metadata;

namespace Sada.Api.Queue
{
    public class UsusarioQueue
    {
        private readonly string _queueName = "usuario_queue";
        private readonly string _hostName = "localhost";

        public UsusarioQueue(string hostName, string queueName)
        {
            _hostName = hostName;
            _queueName = queueName;
        }

        public async Task PublishAsync<T>(T message)
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostName
            };

            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties
            {
                Persistent = true
            };

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: _queueName,
                mandatory: false,
                basicProperties: properties,
                body: body
            );
        }
    }
}