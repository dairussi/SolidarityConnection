using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Infrastructure.Options;
using System.Text;
using System.Text.Json;

namespace SolidarityConnection.Infrastructure.Messaging;
public sealed class RabbitMqPublisher : IMessagePublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqPublisher(IOptions<RabbitMqOptions> rabbitMqOptions)
    {
        var options = rabbitMqOptions.Value;

        var factory = new ConnectionFactory
        {
            HostName = options.Host,
            Port = options.Port,
            UserName = options.Username,
            Password = options.Password
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public Task PublishAsync<T>(
        T message,
        string queueName,
        CancellationToken cancellationToken)
    {
        _channel.QueueDeclare(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        var props = _channel.CreateBasicProperties();
        props.Persistent = true;

        _channel.BasicPublish(
            exchange: string.Empty,
            routingKey: queueName,
            basicProperties: props,
            body: body);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}
