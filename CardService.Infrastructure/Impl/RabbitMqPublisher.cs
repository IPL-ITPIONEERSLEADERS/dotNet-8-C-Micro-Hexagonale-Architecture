using CardService.Infrastructure.Contracts;
using Common.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CardService.Infrastructure.Impl
{
    public class RabbitMqPublisher : IRabbitMqPublisher
    {
        private readonly IConnection _connection;
        private readonly string _exchangeName = "card.events";

        public RabbitMqPublisher()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            using var channel = _connection.CreateModel();
            channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Topic, durable: true);
        }

        public async Task PublishEventAsync(IDomainEvent @event)
        {
            using var channel = _connection.CreateModel();
            var eventType = @event.GetType().Name;
            var jsonData = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(jsonData);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: $"card.{eventType}",
                basicProperties: properties,
                body: body
            );
        }
    }
}
